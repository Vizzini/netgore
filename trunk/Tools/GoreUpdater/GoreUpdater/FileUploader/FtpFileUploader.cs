﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Threading;
using log4net;

namespace GoreUpdater
{
    /// <summary>
    /// Implementation of the <see cref="IFileUploader"/> for FTP.
    /// </summary>
    public class FtpFileUploader : IFileUploader
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// For how long each thread will time out when the job queue is empty.
        /// </summary>
        const int _emptyJobQueueTimeout = 500;

        /// <summary>
        /// For how long to wait after failing a job. This prevents failed jobs from overloading the system or network by constantly
        /// failing over and over, along with gives failures due to issues that can be resolved automaticaly over time a bit of time before
        /// trying again.
        /// </summary>
        const int _jobFailedTimeout = 1000;

        /// <summary>
        /// The number of FTP connections that may be open at once.
        /// </summary>
        const int _numConnections = 4;

        /// <summary>
        /// The number of uploading threads for each <see cref="FtpFileUploader"/> instance.
        /// </summary>
        const int _numThreads = 3;

        /// <summary>
        /// A shared sync root for creating <see cref="WebRequest"/>s for all <see cref="FtpFileUploader"/>s.
        /// </summary>
        static readonly object _webRequestSync = new object();

        /// <summary>
        /// The credentials to use for the Ftp requests.
        /// </summary>
        readonly NetworkCredential _credentials;

        /// <summary>
        /// The root address to the Ftp server. Guarenteed to end with a /.
        /// </summary>
        readonly string _hostRoot;

        /// <summary>
        /// Jobs that are currently being executed. These will not be in the <see cref="_jobsQueue"/>. Instead, when a job
        /// is started, it is moved from the <see cref="_jobsQueue"/> into this list. When it finishes, it is removed from
        /// this list. If it finished unsuccessfully, it is added back to the <see cref="_jobsQueue"/>.
        /// This list must also be synchronized using the <see cref="_jobsSync"/>.
        /// </summary>
        readonly List<IFtpFileUploaderJob> _jobsActive = new List<IFtpFileUploaderJob>();

        /// <summary>
        /// The queue of files to upload, where the key is the local file path and the value is the relative remote path.
        /// Items are grabbed from this queue when started, and put back if they fail.
        /// </summary>
        readonly Queue<IFtpFileUploaderJob> _jobsQueue = new Queue<IFtpFileUploaderJob>();

        /// <summary>
        /// Sync root for <see cref="_jobsQueue"/> and <see cref="_jobsActive"/>.
        /// </summary>
        readonly object _jobsSync = new object();

        readonly RequestCachePolicy _requestCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);

        /// <summary>
        /// The list of worker threads.
        /// </summary>
        readonly List<Thread> _threads = new List<Thread>();

        /// <summary>
        /// Sync root for <see cref="_threads"/>.
        /// </summary>
        readonly object _threadsSync = new object();

        /// <summary>
        /// If this object has been disposed.
        /// </summary>
        bool _isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpFileUploader"/> class.
        /// </summary>
        /// <param name="hostRoot">The root address of the Ftp server to upload to, including the host. Must be prefixed with
        /// ftp://. For example: ftp://www.netgore.com/my_dir/</param>
        /// <param name="username">The Ftp account username.</param>
        /// <param name="password">The Ftp account password.</param>
        public FtpFileUploader(string hostRoot, string username, string password)
        {
            SkipIfExists = true;
            UsePassive = false;

            if (!hostRoot.EndsWith("/"))
                hostRoot += "/";

            _hostRoot = hostRoot;

            _credentials = new NetworkCredential(username, password);

            lock (_threadsSync)
            {
                for (var i = 0; i < _numThreads; i++)
                {
                    var t = new Thread(WorkerThreadLoop) { IsBackground = true };
                    try
                    {
                        t.Name = "FtpFileUploader worker " + i + " for: " + hostRoot;
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail(ex.ToString());
                    }

                    t.Start();

                    _threads.Add(t);
                }
            }
        }

        /// <summary>
        /// Gets the path to the Ftp server and root directory that files will be uploaded to.
        /// </summary>
        public string FtpRoot
        {
            get { return _hostRoot; }
        }

        /// <summary>
        /// Gets or sets if passive FTP will be used. You may have to change this value to get the FTP connection to work, depending
        /// on your system configuration. Default value is false.
        /// </summary>
        public bool UsePassive { get; set; }

        /// <summary>
        /// Creates a <see cref="FtpWebRequest"/> that can be used and sets it up with the default values.
        /// </summary>
        /// <param name="fullRemotePath">The full remote path for the web request being created.</param>
        /// <returns>The <see cref="FtpWebRequest"/> to use.</returns>
        FtpWebRequest CreateFtpWebRequest(string fullRemotePath)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("Creating FtpWebRequest for path: {0}", fullRemotePath);

            // I am not sure if the WebRequest.Create() method is thread-safe already, so might as well just ensure
            // that we minimize any threading issues within it by making every FtpFileUploader lock (our sync object is static)
            FtpWebRequest req;
            lock (_webRequestSync)
            {
                req = (FtpWebRequest)WebRequest.Create(fullRemotePath);
            }

            // Set the common properties
            req.UseBinary = true;
            req.UsePassive = UsePassive;
            req.KeepAlive = true;
            req.CachePolicy = _requestCachePolicy;
            req.ConnectionGroupName = _credentials.UserName + "@" + _hostRoot;
            req.ServicePoint.ConnectionLimit = _numConnections;
            req.Credentials = _credentials;

            return req;
        }

        /// <summary>
        /// Enqueues the specified job.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <returns>True if successfully enqueued; otherwise false.</returns>
        bool EnqueueJob(IFtpFileUploaderJob job)
        {
            lock (_jobsSync)
            {
                // Check if this job already exists
                if (_jobsQueue.Any(x => x.AreJobsSame(job)) || _jobsActive.Any(x => x.AreJobsSame(job)))
                {
                    if (log.IsDebugEnabled)
                        log.DebugFormat("Enqueueing job `{0}` failed: job already in queue.", job);

                    return false;
                }

                // Add the job
                _jobsQueue.Enqueue(job);

                if (log.IsDebugEnabled)
                    log.DebugFormat("Enqueueing job `{0}` successful.", job);
            }

            return true;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="FtpFileUploader"/> is reclaimed by garbage collection.
        /// </summary>
        ~FtpFileUploader()
        {
            HandleDispose(false);
        }

        /// <summary>
        /// Gets the error messages from an <see cref="Exception"/>.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/>.</param>
        /// <returns>The error messages from the <paramref name="ex"/>.</returns>
        static string FormatExceptionMessage(Exception ex)
        {
            var s = string.Empty;
            while (ex != null)
            {
                if (s.Length > 0)
                    s += Environment.NewLine + "-------------------" + Environment.NewLine;
                s += ex.Message;
                ex = ex.InnerException;
            }

            return s;
        }

        /// <summary>
        /// Ensures that the path for a FTP directory exists.
        /// </summary>
        /// <param name="filePath">The complete path for the remote file path that will be used. If you want to pass a directory
        /// instead of a file, make sure that the directory has a trailing slash.</param>
        void FtpCreateDirectoryIfNotExists(string filePath)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("FtpCreateDirectoryIfNotExists(filePath: {0})", filePath);

            // Find the index of the last path separator
            var lastSlash = filePath.LastIndexOf('/');

            // If the path separator index is less than the host root length, then assume we are at the end
            // since we only want to create stuff under the host root
            if (lastSlash < _hostRoot.Length)
                return;

            // Get the directory name
            var dir = filePath.Substring(0, lastSlash);

            // Recursively call this method to ensure the parent directories are created
            FtpCreateDirectoryIfNotExists(dir);

            // Create and execute the request
            var req = CreateFtpWebRequest(dir);
            req.Method = WebRequestMethods.Ftp.MakeDirectory;

            try
            {
                var res = (FtpWebResponse)req.GetResponse();
                res.Close();
            }
            catch (WebException ex)
            {
                // Ignore errors creating the folder, which will happen if the folder already exists
                var res = (FtpWebResponse)ex.Response;
                try
                {
                    switch (res.StatusCode)
                    {
                        case FtpStatusCode.ActionNotTakenFileUnavailable:
                        case FtpStatusCode.ActionNotTakenFileUnavailableOrBusy:
                            break;

                        default:
                            const string errmsg = "Error trying to create directory `{0}`: {1}";
                            if (log.IsErrorEnabled)
                                log.ErrorFormat(errmsg, dir, ex);
                            Debug.Fail(string.Format(errmsg, dir, ex));
                            throw;
                    }
                }
                finally
                {
                    res.Close();
                }
            }
        }

        /// <summary>
        /// Creates a file on the Ftp server. Required sub-directories are created automatically when needed.
        /// </summary>
        /// <param name="localFile">The full path to the local file to upload.</param>
        /// <param name="remoteFile">The path on the server to upload the file to. Can be a relative or fully qualified path.</param>
        void FtpCreateFile(string localFile, string remoteFile)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("FtpCreateFile(localFile: {0}, remoteFile: {1})", localFile, remoteFile);

            remoteFile = ResolveRemotePath(remoteFile);

            var remoteTempFile = remoteFile + ".ftptmp";

            // Check if the remote file already exists
            var fileExists = FtpFileExists(remoteFile);

            // Make sure the temporary file does not exist
            try
            {
                FtpDeleteFile(remoteTempFile);
            }
            catch (Exception ex)
            {
                const string errmsg = "FtpDeleteFile on `{0}` failed (likely not an issue): {1}";
                if (log.IsWarnEnabled)
                    log.WarnFormat(errmsg, remoteTempFile, ex);
                Debug.Fail(string.Format(errmsg, remoteTempFile, ex));
            }

            // If the we are skipping files that exist, then return if it exists
            if (fileExists && SkipIfExists)
            {
                if (log.IsDebugEnabled)
                    log.DebugFormat("Skipping creating remote file `{0}` - file already exists and SkipIfExists is set.",
                        remoteFile);
                return;
            }

            if (fileExists)
            {
                // Delete the existing file
                FtpDeleteFile(remoteFile);
            }
            else
            {
                // Ensure that the directory exists
                FtpCreateDirectoryIfNotExists(remoteFile);
            }

            // Get the name of just the file
            var fileName = remoteFile.Substring(remoteFile.LastIndexOf('/') + 1);

            Debug.Assert(!fileName.StartsWith("/"));

            // Read the file to upload into memory
            var data = File.ReadAllBytes(localFile);

            // Set up the request
            var req = CreateFtpWebRequest(remoteTempFile);
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.ContentLength = data.Length;
            req.ContentOffset = 0;

            // Writes the data to the request stream
            var reqStream = req.GetRequestStream();
            try
            {
                reqStream.Write(data, 0, data.Length);
            }
            finally
            {
                reqStream.Close();
            }

            // Rename the file from the temporary name to the correct name
            FtpRenameFile(remoteTempFile, fileName);
        }

        /// <summary>
        /// Deletes a directory, including all files and sub-directories inside it.
        /// </summary>
        /// <param name="dirPath">The remote path to the directory to delete.  Can be a relative or fully qualified path.</param>
        /// <param name="requireExists">If false and the <paramref name="dirPath"/> does not exist, this method will fail silently
        /// instead of throwing a <see cref="WebException"/>.</param>
        void FtpDeleteDir(string dirPath, bool requireExists = false)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("FtpDeleteDir(dirPath: {0}, requireExists: {1})", dirPath, requireExists);

            dirPath = ResolveRemotePath(dirPath);

            // Make sure the path always ends with a /, so that we get listings without the parent directory's name and that
            // files will return a null list
            if (!dirPath.EndsWith("/"))
                dirPath += "/";

            // Get the directory listing so we can delete the subdirectories and files recursively first before trying
            // to delete this directory. The following return states let us know what type of file it is:
            //  null: A file
            //  empty list: An empty directory
            //  non-empty list: A non-empty directory
            var listing = FtpNListDir(dirPath);

            // Null means it was a file (or an invalid directory..., in which case no deletion is needed anyways), so delete
            // it as a file then return - no more recursion needed
            if (listing == null)
            {
                // Remember that we have to remove the trailing slash for files
                var filePath = dirPath.Substring(0, dirPath.Length - 1);
                FtpDeleteFile(filePath);
                return;
            }

            // Recursively call FtpDeleteDir on every listing (some of which will be files, some directories... doesn't matter)
            foreach (var f in listing)
            {
                FtpDeleteDir(dirPath + f);
            }

            // Do not allow deletion of the root directory. Instead, just empty it and abort.
            if (dirPath.Length <= _hostRoot.Length)
                return;

            // By now, we should know we have a directory (since a file would call FtpDeleteFile then return), and thanks to the recursion,
            // it should be empty. So delete this directory.
            var req = CreateFtpWebRequest(dirPath);
            req.UseBinary = true;
            req.Method = WebRequestMethods.Ftp.RemoveDirectory;

            try
            {
                var res = (FtpWebResponse)req.GetResponse();

                if (res != null)
                    res.Close();
            }
            catch (WebException ex)
            {
                var res = (FtpWebResponse)ex.Response;
                try
                {
                    switch (res.StatusCode)
                    {
                        case FtpStatusCode.ActionNotTakenFileUnavailable:
                            if (requireExists)
                            {
                                const string errmsg =
                                    "Failed to delete remote directory `{0}` when requireExists was set. Exception: {1}";
                                if (log.IsErrorEnabled)
                                    log.ErrorFormat(errmsg, dirPath, ex);
                                Debug.Fail(string.Format(errmsg, dirPath, ex));
                                throw;
                            }
                            break;

                        default:
                            const string errmsg2 = "Failed to delete remote directory `{0}` due to unexpected error: {1}";
                            if (log.IsErrorEnabled)
                                log.ErrorFormat(errmsg2, dirPath, ex);
                            Debug.Fail(string.Format(errmsg2, dirPath, ex));
                            throw;
                    }
                }
                finally
                {
                    res.Close();
                }
            }
        }

        /// <summary>
        /// Deletes a file from the Ftp server.
        /// </summary>
        /// <param name="filePath">The path to the remote file to delete. Can be a relative or fully qualified path.</param>
        /// <param name="requireExists">If true and the <paramref name="filePath"/> does not exist, a <see cref="WebException"/>
        /// will be thrown. If false and the file does not exist, it will just silently fail.</param>
        void FtpDeleteFile(string filePath, bool requireExists = false)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("FtpDeleteFile(filePath: {0}, requireExists: {1})", filePath, requireExists);

            filePath = ResolveRemotePath(filePath);

            // Create and execute the request
            var req = CreateFtpWebRequest(filePath);
            req.UseBinary = true;
            req.Method = WebRequestMethods.Ftp.DeleteFile;

            try
            {
                var res = (FtpWebResponse)req.GetResponse();

                if (res != null)
                    res.Close();
            }
            catch (WebException ex)
            {
                // Ignore errors creating the folder, which will happen if the folder already exists
                var res = (FtpWebResponse)ex.Response;
                try
                {
                    switch (res.StatusCode)
                    {
                        case FtpStatusCode.ActionNotTakenFileUnavailable:
                        case FtpStatusCode.ActionNotTakenFileUnavailableOrBusy:
                            if (requireExists)
                            {
                                const string errmsg =
                                    "Failed to delete remote file `{0}` when requireExists was set. Exception: {1}";
                                if (log.IsErrorEnabled)
                                    log.ErrorFormat(errmsg, filePath, ex);
                                Debug.Fail(string.Format(errmsg, filePath, ex));
                                throw;
                            }
                            break;

                        default:
                            const string errmsg2 = "Failed to delete remote file `{0}` due to unexpected error: {1}";
                            if (log.IsErrorEnabled)
                                log.ErrorFormat(errmsg2, filePath, ex);
                            Debug.Fail(string.Format(errmsg2, filePath, ex));
                            throw;
                    }
                }
                finally
                {
                    res.Close();
                }
            }
        }

        /// <summary>
        /// Downloads a remote file from the Ftp server.
        /// </summary>
        /// <param name="remoteFile">The remote file to download.</param>
        /// <param name="requireExists">If false, and the remote file does not exist, a null will be returned instead.</param>
        /// <returns>The contents of the <paramref name="remoteFile"/>.</returns>
        byte[] FtpDownload(string remoteFile, bool requireExists)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("FtpDownload(remoteFile: {0}, requireExists: {1})", remoteFile, requireExists);

            var fullRemotePath = ResolveRemotePath(remoteFile);
            var req = CreateFtpWebRequest(fullRemotePath);
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.UseBinary = true;

            try
            {
                using (var res = req.GetResponse())
                {
                    if (res == null)
                        throw new WebException("Failed to get response while trying to download remote file `" + remoteFile + "`.");

                    using (var resStream = res.GetResponseStream())
                    {
                        if (resStream == null)
                            throw new WebException("Could not open stream to remote file `" + remoteFile + "`.");

                        using (var ms = new MemoryStream())
                        {
                            var b = new byte[8192];
                            int r;
                            while ((r = resStream.Read(b, 0, b.Length)) > 0)
                            {
                                ms.Write(b, 0, r);
                            }

                            return ms.ToArray();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                var res = (FtpWebResponse)ex.Response;
                try
                {
                    switch (res.StatusCode)
                    {
                        case FtpStatusCode.ActionNotTakenFileUnavailable:
                        case FtpStatusCode.ActionNotTakenFileUnavailableOrBusy:
                            if (!requireExists)
                                return null;
                            else
                                throw;

                        default:
                            const string errmsg = "Failed to download remote file `{0}`. Exception: {1}";
                            if (log.IsErrorEnabled)
                                log.ErrorFormat(errmsg, fullRemotePath, ex);
                            Debug.Fail(string.Format(errmsg, fullRemotePath, ex));
                            throw;
                    }
                }
                finally
                {
                    res.Close();
                }
            }
        }

        /// <summary>
        /// Downloads a remote file from the Ftp server as a string.
        /// </summary>
        /// <param name="remoteFile">The remote file to download.</param>
        /// <param name="requireExists">If false, and the remote file does not exist, a null will be returned instead.</param>
        /// <returns>The contents of the <paramref name="remoteFile"/>.</returns>
        string FtpDownloadAsString(string remoteFile, bool requireExists)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("FtpDownloadAsString(remoteFile: {0}, requireExists: {1})", remoteFile, requireExists);

            var fullRemotePath = ResolveRemotePath(remoteFile);
            var req = CreateFtpWebRequest(fullRemotePath);
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.UseBinary = false;

            try
            {
                using (var res = req.GetResponse())
                {
                    if (res == null)
                        throw new WebException("Failed to get response while trying to download remote file `" + remoteFile + "`.");

                    using (var resStream = res.GetResponseStream())
                    {
                        if (resStream == null)
                            throw new WebException("Could not open stream to remote file `" + remoteFile + "`.");

                        using (var sr = new StreamReader(resStream))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                var res = (FtpWebResponse)ex.Response;
                try
                {
                    switch (res.StatusCode)
                    {
                        case FtpStatusCode.ActionNotTakenFileUnavailable:
                        case FtpStatusCode.ActionNotTakenFileUnavailableOrBusy:
                            if (!requireExists)
                                return null;
                            else
                                throw;

                        default:
                            const string errmsg = "Failed to download remote file `{0}` as string. Exception: {1}";
                            if (log.IsErrorEnabled)
                                log.ErrorFormat(errmsg, fullRemotePath, ex);
                            Debug.Fail(string.Format(errmsg, fullRemotePath, ex));
                            throw;
                    }
                }
                finally
                {
                    res.Close();
                }
            }
        }

        /// <summary>
        /// Gets if a file exists on the server.
        /// </summary>
        /// <param name="filePath">The remote path to the file to check if exists. Can be a relative or fully qualified path.</param>
        /// <returns>True if the file exists; false if the file does not exist or you do not have the credentials to access the file.</returns>
        bool FtpFileExists(string filePath)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("FtpFileExists(filePath: {0})", filePath);

            filePath = ResolveRemotePath(filePath);

            // Create and execute the request
            var req = CreateFtpWebRequest(filePath);
            req.UseBinary = true;
            req.Method = WebRequestMethods.Ftp.GetDateTimestamp;

            try
            {
                var res = (FtpWebResponse)req.GetResponse();

                if (res != null)
                    res.Close();

                return true;
            }
            catch (WebException ex)
            {
                // Ignore errors creating the folder, which will happen if the folder already exists
                var res = (FtpWebResponse)ex.Response;
                try
                {
                    switch (res.StatusCode)
                    {
                        case FtpStatusCode.ActionNotTakenFileUnavailable:
                        case FtpStatusCode.ActionNotTakenFileUnavailableOrBusy:
                            return false;

                        default:
                            const string errmsg = "FtpFileExist for path `{0}` failed. Exception: {1}";
                            if (log.IsErrorEnabled)
                                log.ErrorFormat(errmsg, filePath, ex);
                            Debug.Fail(string.Format(errmsg, filePath, ex));
                            throw;
                    }
                }
                finally
                {
                    res.Close();
                }
            }
        }

        /// <summary>
        /// Gets the Ftp directory listing containing only file names (FTP NLST).
        /// </summary>
        /// <param name="remotePath">The path to get the listing for.</param>
        /// <param name="requireExists">When false, this method will silently fail when the <paramref name="remotePath"/>
        /// does not exist.</param>
        /// <returns>The directory listing for the <paramref name="remotePath"/>, or null if <paramref name="requireExists"/> is
        /// false and the directory does not exist.</returns>
        IEnumerable<string> FtpNListDir(string remotePath, bool requireExists = false)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("FtpNListDir(remotePath: {0}, requireExists: {1})", remotePath, requireExists);

            remotePath = ResolveRemotePath(remotePath);

            var req = CreateFtpWebRequest(remotePath);
            req.Method = WebRequestMethods.Ftp.ListDirectory;

            try
            {
                using (var res = req.GetResponse())
                {
                    if (res == null)
                    {
                        const string errmsgResNull = "Received null WebResponse for remote path `{0}`. Returning null.";
                        if (log.IsWarnEnabled)
                            log.WarnFormat(errmsgResNull, remotePath);
                        Debug.Fail(string.Format(errmsgResNull, remotePath));
                        return null;
                    }

                    using (var resStream = res.GetResponseStream())
                    {
                        if (resStream == null)
                        {
                            const string errmsgResStreamNull =
                                "Received null WebResponseStream for remote path `{0}`. Returning null.";
                            if (log.IsWarnEnabled)
                                log.WarnFormat(errmsgResStreamNull, remotePath);
                            Debug.Fail(string.Format(errmsgResStreamNull, remotePath));
                            return null;
                        }

                        using (var sr = new StreamReader(resStream))
                        {
                            var ret = new List<string>();

                            if (log.IsDebugEnabled)
                            {
                                log.DebugFormat(
                                    "Starting read of WebResponseStream `{0}` for remote path `{1}` using StreamReader `{2}`.",
                                    resStream, remotePath, sr);
                            }

                            while (!sr.EndOfStream)
                            {
                                var s = sr.ReadLine();

                                if (log.IsDebugEnabled)
                                    log.DebugFormat("Read WebResponseStream line for remote path `{0}`: {1}", remotePath,
                                        s ?? "[NULL]");

                                // Skip null lines
                                if (s == null)
                                    continue;

                                // Trim
                                s = s.Trim();

                                // Skip empty lines
                                if (s.Length == 0)
                                    continue;

                                // Add the line
                                ret.Add(s);
                            }

                            // Return the file listing
                            return ret;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                var res = (FtpWebResponse)ex.Response;
                try
                {
                    switch (res.StatusCode)
                    {
                        case FtpStatusCode.ActionNotTakenFileUnavailable:
                            if (requireExists)
                            {
                                const string errmsg =
                                    "FtpNListDir for path `{0}` failed when requireExists was set. Exception: {1}";
                                if (log.IsErrorEnabled)
                                    log.ErrorFormat(errmsg, remotePath, ex);
                                Debug.Fail(string.Format(errmsg, remotePath, ex));
                                throw;
                            }
                            break;

                        default:
                            const string errmsg2 = "FtpNListDir for path `{0}` failed. Exception: {1}";
                            if (log.IsErrorEnabled)
                                log.ErrorFormat(errmsg2, remotePath, ex);
                            Debug.Fail(string.Format(errmsg2, remotePath, ex));
                            throw;
                    }
                }
                finally
                {
                    res.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Renames a file on the Ftp server.
        /// </summary>
        /// <param name="filePath">The path to the file to rename. Can be a relative or fully qualified path.</param>
        /// <param name="newName">The new name of the file.</param>
        void FtpRenameFile(string filePath, string newName)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("FtpRenameFile(filePath: {0}, newName: {1})", filePath, newName);

            filePath = ResolveRemotePath(filePath);

            // Create and execute the request
            var req = CreateFtpWebRequest(filePath);
            req.UseBinary = true;
            req.Method = WebRequestMethods.Ftp.Rename;
            req.RenameTo = newName;

            var res = (FtpWebResponse)req.GetResponse();

            if (res != null)
                res.Close();
        }

        /// <summary>
        /// Handles disposing this object.
        /// </summary>
        /// <param name="disposeManaged">If managed resources should be disposed.</param>
        protected virtual void HandleDispose(bool disposeManaged)
        {
            if (!disposeManaged)
                return;

            lock (_threadsSync)
            {
                foreach (var t in _threads)
                {
                    // ReSharper disable EmptyGeneralCatchClause
                    try
                    {
                        if (log.IsDebugEnabled)
                            log.DebugFormat("Disposing `{0}`'s worker thread `{1}`.", this, t);

                        t.Abort();
                    }
                    catch (Exception ex)
                    {
                        const string errmsg = "Disposing `{0}`'s worker thread `{1}` failed. Exception: {2}";
                        if (log.IsWarnEnabled)
                            log.WarnFormat(errmsg, this, t, ex);
                        Debug.Fail(string.Format(errmsg, this, t, ex));
                    }
                    // ReSharper restore EmptyGeneralCatchClause
                }

                _threads.Clear();
            }
        }

        /// <summary>
        /// Gets the fully resolved path for a relative remote path.
        /// </summary>
        /// <param name="relativePath">The relative remote path.</param>
        /// <returns>The fully resolved remote path.</returns>
        string ResolveRemotePath(string relativePath)
        {
            // Check if the path is already resolved
            if (relativePath.Length >= _hostRoot.Length && relativePath.StartsWith(_hostRoot))
                return relativePath;

            if (relativePath.Length == 0)
                return _hostRoot;

            if (relativePath.StartsWith("/"))
                return _hostRoot + relativePath.Substring(1);
            else
                return _hostRoot + relativePath;
        }

        /// <summary>
        /// Sanitizes the relative remote path for a file to upload.
        /// </summary>
        /// <param name="p">The relative path.</param>
        /// <returns>The sanitized <paramref name="p"/>.</returns>
        static string SanitizeFtpTargetPath(string p)
        {
            // Trim
            var ret = p.Trim();

            // Make sure to always use / instead of \
            ret = ret.Replace('\\', '/');

            // Remove a prefixed path separator
            if (ret.StartsWith("/"))
                ret = ret.Substring(1);

            if (log.IsDebugEnabled)
                log.DebugFormat("Path `{0}` sanitized to `{1}`.", p, ret);

            return ret;
        }

        /// <summary>
        /// The method for worker threads.
        /// </summary>
        void WorkerThreadLoop()
        {
            while (!IsDisposed)
            {
                IFtpFileUploaderJob job = null;

                // Get the next job
                lock (_jobsSync)
                {
                    if (_jobsQueue.Count > 0)
                    {
                        job = _jobsQueue.Dequeue();
                        Debug.Assert(!_jobsActive.Contains(job), "Why was the job already in the active list? This is NOT good...");
                        _jobsActive.Add(job);
                    }
                }

                // Sleep if there was no jobs free
                if (job == null)
                {
                    Thread.Sleep(_emptyJobQueueTimeout);
                    continue;
                }

                if (log.IsInfoEnabled)
                    log.InfoFormat("Processing job: {0}", job);

                try
                {
                    // Execute the job
                    job.Execute(this);
                }
                catch (Exception ex)
                {
                    if (log.IsInfoEnabled)
                        log.InfoFormat("Processing job `{0}` failed. Exception: {1}", job, ex);

                    // Throw the job back into the queue
                    lock (_jobsSync)
                    {
                        // Remove the job from the active list and throw it back into the queue
                        var removed = _jobsActive.Remove(job);
                        Debug.Assert(removed,
                            "Why was this job not in the _jobsActive list? It should be there since it was just active.");

                        Debug.Assert(!_jobsQueue.Contains(job),
                            "Why was this job already in the job queue? Now we're going to get stuck running the job multiple times...");
                        _jobsQueue.Enqueue(job);
                    }

                    // Present the error
                    if (job is JobUploadFile)
                    {
                        var asJobCreateFile = (JobUploadFile)job;

                        try
                        {
                            if (UploadError != null)
                                UploadError(this, asJobCreateFile.LocalFile, asJobCreateFile.RemoteFile,
                                    FormatExceptionMessage(ex), job.Attempts);
                        }
                        catch (NullReferenceException ex2)
                        {
                            Debug.Fail(ex2.ToString());
                        }
                    }
                    else if (job is JobDownloadFile)
                    {
                        var asJobDownloadFile = (JobDownloadFile)job;

                        try
                        {
                            if (DownloadError != null)
                                DownloadError(this, asJobDownloadFile.LocalFile, asJobDownloadFile.RemoteFile,
                                    FormatExceptionMessage(ex), job.Attempts);
                        }
                        catch (NullReferenceException ex2)
                        {
                            Debug.Fail(ex2.ToString());
                        }
                    }
                    else if (job is JobDeleteDir)
                    {
                        var asJobDeleteDir = (JobDeleteDir)job;

                        try
                        {
                            if (DeleteDirectoryError != null)
                                DeleteDirectoryError(this, asJobDeleteDir.RemotePath, FormatExceptionMessage(ex), job.Attempts);
                        }
                        catch (NullReferenceException ex2)
                        {
                            Debug.Fail(ex2.ToString());
                        }
                    }
                    else
                    {
                        const string errmsg =
                            "No error notification event available for job type `{0}`." +
                            " Usually best to allow people to view the job errors.";
                        Debug.Fail(string.Format(errmsg, job.GetType()));
                    }

                    // Sleep for a bit
                    Thread.Sleep(_jobFailedTimeout);
                    continue;
                }

                // If we made it this far, the file job was successful!
                if (log.IsInfoEnabled)
                    log.InfoFormat("Job `{0}` successfully processed.", job);

                // Remove the job from the active list
                lock (_jobsSync)
                {
                    var removed = _jobsActive.Remove(job);
                    Debug.Assert(removed,
                        "Why was this job not in the _jobsActive list? It should be there since it was just active.");
                }

                // Notify listeners
                if (job is JobUploadFile)
                {
                    var asJobCreateFile = (JobUploadFile)job;

                    try
                    {
                        if (UploadComplete != null)
                            UploadComplete(this, asJobCreateFile.LocalFile, asJobCreateFile.RemoteFile);
                    }
                    catch (NullReferenceException ex)
                    {
                        Debug.Fail(ex.ToString());
                    }
                }
                else if (job is JobDownloadFile)
                {
                    var asJobDownloadFile = (JobDownloadFile)job;

                    try
                    {
                        if (DownloadComplete != null)
                            DownloadComplete(this, asJobDownloadFile.LocalFile, asJobDownloadFile.RemoteFile);
                    }
                    catch (NullReferenceException ex)
                    {
                        Debug.Fail(ex.ToString());
                    }
                }
                else if (job is JobDeleteDir)
                {
                    var asJobDeleteDir = (JobDeleteDir)job;

                    try
                    {
                        if (DeleteDirectoryComplete != null)
                            DeleteDirectoryComplete(this, asJobDeleteDir.RemotePath);
                    }
                    catch (NullReferenceException ex)
                    {
                        Debug.Fail(ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Invokes the <see cref="TestConnectionMessage"/> event.
        /// </summary>
        /// <param name="userState">State of the user.</param>
        /// <param name="msg">The message.</param>
        void WriteTestConnectionMessage(object userState, string msg)
        {
            if (TestConnectionMessage != null)
            {
                try
                {
                    TestConnectionMessage(this, msg, userState);
                }
                catch (NullReferenceException ex)
                {
                    Debug.Fail(ex.ToString());
                }
            }

            if (log.IsInfoEnabled)
                log.InfoFormat("TestConnectionMessage (userState: {0}): {1}", userState != null ? userState.ToString() : "[NULL]",
                    msg);
        }

        #region IFileUploader Members

        /// <summary>
        /// Notifies listeners when an asynchronous request to delete a directory has been completed.
        /// </summary>
        public event FileUploaderDeleteDirEventHandler DeleteDirectoryComplete;

        /// <summary>
        /// Notifies listeners when an asynchronous request to delete a directory has encountered an error.
        /// </summary>
        public event FileUploaderDeleteDirErrorEventHandler DeleteDirectoryError;

        /// <summary>
        /// Notifies listeners when an asynchronous download has been completed.
        /// </summary>
        public event FileUploaderDownloadEventHandler DownloadComplete;

        /// <summary>
        /// Notifies listeners when there has been an error related to one of the asynchronous download jobs.
        /// The job in question will still be re-attempted by default.
        /// </summary>
        public event FileUploaderDownloadErrorEventHandler DownloadError;

        /// <summary>
        /// Notifies listeners when the <see cref="IFileUploader.TestConnection"/> method has produced a message
        /// related to the status of the connection testing. This only contains status update messages, not error
        /// messages.
        /// </summary>
        public event FileUploaderTestConnectionMessageEventHandler TestConnectionMessage;

        /// <summary>
        /// Notifies listeners when an asynchronous upload has been completed.
        /// </summary>
        public event FileUploaderUploadEventHandler UploadComplete;

        /// <summary>
        /// Notifies listeners when there has been an error related to one of the asynchronous upload jobs.
        /// The job in question will still be re-attempted by default.
        /// </summary>
        public event FileUploaderUploadErrorEventHandler UploadError;

        /// <summary>
        /// Gets if the <see cref="IFileUploader"/> is currently busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                lock (_jobsSync)
                {
                    if (_jobsActive.Count > 0 || _jobsQueue.Count > 0)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets if this object has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        /// <summary>
        /// Gets the number of jobs remaining. Includes both queued and in-progress jobs.
        /// </summary>
        public int JobsRemaining
        {
            get
            {
                lock (_jobsSync)
                {
                    return _jobsActive.Count + _jobsQueue.Count;
                }
            }
        }

        /// <summary>
        /// Gets or sets if files that already exist on the destination will be skipped. Default value is true.
        /// </summary>
        public bool SkipIfExists { get; set; }

        /// <summary>
        /// Removes a file from the asynchronous download queue and aborts it.
        /// </summary>
        /// <param name="localPath">The fully qualified local path of the download to cancel.</param>
        /// <returns>True if the job was removed; otherwise false.</returns>
        public bool CancelAsyncDownload(string localPath)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("this");

            if (log.IsInfoEnabled)
                log.InfoFormat("Canceling async download for `{0}`.", localPath);

            lock (_jobsSync)
            {
                var match =
                    _jobsQueue.OfType<JobDownloadFile>().FirstOrDefault(x => StringComparer.Ordinal.Equals(x.LocalFile, localPath));
                if (match == null)
                    return false;

                // Unfortunately, the only way to really remove the item from the queue (while preserving the ordering) is to clear it,
                // then add everything back except for the one job. Brutal, eh?
                var jobArray = _jobsQueue.ToArray();
                _jobsQueue.Clear();

                foreach (var job in jobArray)
                {
                    if (job != match)
                        _jobsQueue.Enqueue(job);
                }

                return true;
            }
        }

        /// <summary>
        /// Removes a file from the asynchronous upload queue and aborts it.
        /// </summary>
        /// <param name="remotePath">The remote path for the upload to cancel.</param>
        /// <returns>True if the job was removed; otherwise false.</returns>
        public bool CancelAsyncUpload(string remotePath)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("this");

            if (log.IsInfoEnabled)
                log.InfoFormat("Canceling async upload for `{0}`.", remotePath);

            var p = SanitizeFtpTargetPath(remotePath);

            lock (_jobsSync)
            {
                var match = _jobsQueue.OfType<JobUploadFile>().FirstOrDefault(x => StringComparer.Ordinal.Equals(x.RemoteFile, p));
                if (match == null)
                    return false;

                // Unfortunately, the only way to really remove the item from the queue (while preserving the ordering) is to clear it,
                // then add everything back except for the one job. Brutal, eh?
                var jobArray = _jobsQueue.ToArray();
                _jobsQueue.Clear();

                foreach (var job in jobArray)
                {
                    if (job != match)
                        _jobsQueue.Enqueue(job);
                }

                return true;
            }
        }

        /// <summary>
        /// Deletes a directory synchronously. If the root directory is specified, then all files and folders in the root
        /// directory will be deleted, but the root directory itself will not be deleted. Otherwise, the specified directory
        /// will be deleted along with all files and folders under it.
        /// </summary>
        /// <param name="targetPath">The relative path of the directory to delete.</param>
        /// <param name="requireExists">If false, and the <paramref name="targetPath"/> does not exist, then the deletion
        /// will fail silently.</param>
        public void DeleteDirectory(string targetPath, bool requireExists)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("this");

            if (log.IsInfoEnabled)
                log.InfoFormat("Deleting directory `{0}`.", targetPath);

            FtpDeleteDir(targetPath, requireExists);
        }

        /// <summary>
        /// Deletes a directory asynchronously. If the root directory is specified, then all files and folders in the root
        /// directory will be deleted, but the root directory itself will not be deleted. Otherwise, the specified directory
        /// will be deleted along with all files and folders under it.
        /// </summary>
        /// <param name="targetPath">The relative path of the directory to delete.</param>
        /// <returns>True if the directory deletion task was enqueued; false if the <paramref name="targetPath"/> is already
        /// queued for deletion, or if the <paramref name="targetPath"/> is invalid.</returns>
        public bool DeleteDirectoryAsync(string targetPath)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("this");

            if (string.IsNullOrEmpty(targetPath))
                return false;

            if (log.IsInfoEnabled)
                log.InfoFormat("Asynchronously deleting directory `{0}`.", targetPath);

            var job = new JobDeleteDir(targetPath);

            return EnqueueJob(job);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            HandleDispose(true);
            _isDisposed = true;
        }

        /// <summary>
        /// Synchronously downloads a remote file and returns the contents of the downloaded file as an array of bytes.
        /// </summary>
        /// <param name="remoteFile">The remote file to download.</param>
        /// <param name="requireExists">If false, and the remote file does not exist, a null will be returned instead.</param>
        /// <returns>The downloaded file's contents.</returns>
        public byte[] Download(string remoteFile, bool requireExists = false)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("this");

            if (log.IsInfoEnabled)
                log.InfoFormat("Downloading file `{0}`.", remoteFile);

            return FtpDownload(remoteFile, requireExists);
        }

        /// <summary>
        /// Synchronously downloads a remote file and returns the contents of the downloaded file as a string.
        /// </summary>
        /// <param name="remoteFile">The remote file to download.</param>
        /// <param name="requireExists">If false, and the remote file does not exist, a null will be returned instead.</param>
        /// <returns>The downloaded file's contents.</returns>
        public string DownloadAsString(string remoteFile, bool requireExists = false)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("this");

            if (log.IsInfoEnabled)
                log.InfoFormat("Downloading file `{0}` as string.", remoteFile);

            return FtpDownloadAsString(remoteFile, requireExists);
        }

        /// <summary>
        /// Enqueues a file for asynchronous downloading.
        /// </summary>
        /// <param name="remotePath">The path to the file to download on the destination.</param>
        /// <param name="sourcePath">The fully qualified path to download the file to.</param>
        /// <returns>True if the file was enqueued; false if either of the arguments were invalid, or the file already
        /// exists in the queue.</returns>
        public bool DownloadAsync(string remotePath, string sourcePath)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("this");

            if (string.IsNullOrEmpty(remotePath) || string.IsNullOrEmpty(sourcePath))
                return false;

            if (log.IsInfoEnabled)
                log.InfoFormat("Asynchronously Downloading file `{0}`.", remotePath);

            var job = new JobDownloadFile(remotePath, sourcePath);

            return EnqueueJob(job);
        }

        /// <summary>
        /// Enqueues multiple files for asynchronous downloading.
        /// </summary>
        /// <param name="files">The files to download, where the key is the remote file path, and the value is the
        /// fully qualified local path to download the file to.</param>
        public void DownloadAsync(IEnumerable<KeyValuePair<string, string>> files)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("this");

            foreach (var f in files)
            {
                DownloadAsync(f.Key, f.Value);
            }
        }

        /// <summary>
        /// Tests the connection of the <see cref="IFileUploader"/> and ensures that the needed operations can be performed.
        /// The test runs synchronously.
        /// </summary>
        /// <param name="userState">An optional object that can be used. When the <see cref="TestConnectionMessage"/> event is raised,
        /// this object is passed back through the event, allowing you to differentiate between multiple connection tests.</param>
        /// <param name="error">When this method returns false, contains a string describing the error encountered during testing.</param>
        /// <returns>
        /// True if the test was successful; otherwise false.
        /// </returns>
        public bool TestConnection(object userState, out string error)
        {
            const string remoteTestFile = "___connection_test.tmp";
            const string remoteTestDirRoot = "/__connection_test/";
            const string remoteTestDir = remoteTestDirRoot + "/__connection_test/__test/__a/";
            const string remoteTestFileContents = "Test file. Please delete.";

            WriteTestConnectionMessage(userState, "Connection information:");
            WriteTestConnectionMessage(userState, " * Class: " + GetType());
            WriteTestConnectionMessage(userState, " * Host: " + FtpRoot);
            WriteTestConnectionMessage(userState, " * User: " + _credentials.UserName);
            WriteTestConnectionMessage(userState, " * Domain: " + _credentials.Domain);
            WriteTestConnectionMessage(userState, " * Passive FTP?: " + UsePassive);

            WriteTestConnectionMessage(userState, "Setting up tests...");
            WriteTestConnectionMessage(userState, "Getting the path for a temporary local file...");

            var tmpFile = Path.GetTempFileName();

            WriteTestConnectionMessage(userState, "Writing content to the temporary local file...");

            File.WriteAllText(tmpFile, "Test file. Please delete.");

            WriteTestConnectionMessage(userState, "Test setup complete");
            WriteTestConnectionMessage(userState, "Starting tests on the server");

            try
            {
                // *** File creation test ***
                WriteTestConnectionMessage(userState, "Uploading a test file to root directory...");

                try
                {
                    FtpCreateFile(tmpFile, remoteTestFile);
                }
                catch (Exception ex)
                {
                    const string errmsg =
                        "Failed to create file on remote server. Check your file creation permissions. Details:{0}{1}";
                    error = string.Format(errmsg, Environment.NewLine, ex);
                    return false;
                }

                WriteTestConnectionMessage(userState, "File upload complete");

                // *** File integrity test ***
                WriteTestConnectionMessage(userState, "Downloading the file just uploaded...");

                try
                {
                    var contents = FtpDownloadAsString(remoteTestFile, true);

                    WriteTestConnectionMessage(userState, "File download complete");
                    WriteTestConnectionMessage(userState, "Checking the contents of the file to ensure integrity was preserved...");

                    if (!StringComparer.Ordinal.Equals(contents.Trim(), remoteTestFileContents))
                    {
                        const string errmsg =
                            "The contents of the downloaded file did not match the expected file contents." +
                            "{0}Expected: {1}{0}Actual: {2}";
                        throw new Exception(string.Format(errmsg, Environment.NewLine, remoteTestFileContents, contents));
                    }
                }
                catch (Exception ex)
                {
                    const string errmsg =
                        "Failed to download file from the remote server. Check your file download permissions. Details:{0}{1}";
                    error = string.Format(errmsg, Environment.NewLine, ex);
                    return false;
                }

                WriteTestConnectionMessage(userState, "File integrity test passed");

                // *** File exists test ***
                WriteTestConnectionMessage(userState, "Testing the ability to detect if the file exists...");

                try
                {
                    var exists = FtpFileExists(remoteTestFile);
                    if (!exists)
                        throw new Exception("File was expected to exist, but FtpFileExists returned false.");
                }
                catch (Exception ex)
                {
                    const string errmsg = "Failed to check file existance on remote server. Details:{0}{1}";
                    error = string.Format(errmsg, Environment.NewLine, ex);
                    return false;
                }

                WriteTestConnectionMessage(userState, "File successfully determined to exist");

                // *** File deletion test ***
                WriteTestConnectionMessage(userState, "Deleting the test file...");

                try
                {
                    FtpDeleteFile(remoteTestFile);
                }
                catch (Exception ex)
                {
                    const string errmsg =
                        "Failed to delete file on remote server. Check your file deletion permissions. Details:{0}{1}";
                    error = string.Format(errmsg, Environment.NewLine, ex);
                    return false;
                }

                WriteTestConnectionMessage(userState, "Test file deleted");

                // *** Directory create test ***
                WriteTestConnectionMessage(userState,
                    "Creating sub-directories and creating a file inside those sub-directories...");

                try
                {
                    FtpCreateFile(tmpFile, remoteTestDir + remoteTestFile);
                }
                catch (Exception ex)
                {
                    const string errmsg =
                        "Failed to create a file inside directory on remote server. Check your directory creation permissions. Details:{0}{1}";
                    error = string.Format(errmsg, Environment.NewLine, ex);
                    return false;
                }

                WriteTestConnectionMessage(userState, "Sub-directories test file successfully created");

                // *** Directory exists test ***
                WriteTestConnectionMessage(userState, "Checking ability to determine if the test file exists...");

                try
                {
                    var exists = FtpFileExists(remoteTestDir + remoteTestFile);
                    if (!exists)
                        throw new Exception("File was expected to exist, but FtpFileExists returned false.");
                }
                catch (Exception ex)
                {
                    const string errmsg =
                        "Failed to check file existance on remote server when inside a subdirectory. Details:{0}{1}";
                    error = string.Format(errmsg, Environment.NewLine, ex);
                    return false;
                }

                WriteTestConnectionMessage(userState, "File successfully determined to exist");

                // *** Directory deletion test ***
                WriteTestConnectionMessage(userState, "Deleting the test sub-directory and all of its contents...");

                try
                {
                    FtpDeleteDir(remoteTestDirRoot);
                }
                catch (Exception ex)
                {
                    const string errmsg =
                        "Failed to delete directory on remote server. Check your directory deletion permissions. Details:{0}{1}";
                    error = string.Format(errmsg, Environment.NewLine, ex);
                    return false;
                }

                WriteTestConnectionMessage(userState, "Test sub-directory successfully deleted");

                // *** File doesn't exists test ***
                WriteTestConnectionMessage(userState, "Checking ability to determine that a file does not exist...");

                try
                {
                    var exists = FtpFileExists(remoteTestFile);
                    if (exists)
                        throw new Exception("File was expected to not exist, but FtpFileExists returned true.");
                }
                catch (Exception ex)
                {
                    const string errmsg = "Failed to check file existance on remote server. Details:{0}{1}";
                    error = string.Format(errmsg, Environment.NewLine, ex);
                    return false;
                }

                WriteTestConnectionMessage(userState, "File successfully determined to not exist");
            }
            catch (Exception ex)
            {
                // Catch unexpected error at any phase during the testing
                const string errmsg = "Unexpected error encountered. Details:{0}{1}";
                error = string.Format(errmsg, Environment.NewLine, ex);
                return false;
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(tmpFile) && File.Exists(tmpFile))
                        File.Delete(tmpFile);
                }
                catch (IOException)
                {
                    // Ignore errors deleting the temp file
                }
            }

            error = null;
            return true;
        }

        /// <summary>
        /// Enqueues a file for asynchronous uploading.
        /// </summary>
        /// <param name="sourcePath">The path to the local file to upload.</param>
        /// <param name="remotePath">The path to upload the file to on the destination.</param>
        /// <returns>True if the file was enqueued; false if either of the arguments were invalid, or the file already
        /// exists in the queue.</returns>
        public bool UploadAsync(string sourcePath, string remotePath)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("this");

            if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(remotePath))
                return false;

            if (log.IsInfoEnabled)
                log.InfoFormat("Asynchronously uploading file `{0}` to `{1}`.", sourcePath, remotePath);

            var job = new JobUploadFile(sourcePath, remotePath);

            return EnqueueJob(job);
        }

        /// <summary>
        /// Enqueues multiple files for uploading.
        /// </summary>
        /// <param name="files">The files to upload, where the key is the source path, and the value is the
        /// path to upload the file on the destination.</param>
        public void UploadAsync(IEnumerable<KeyValuePair<string, string>> files)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("this");

            foreach (var f in files)
            {
                UploadAsync(f.Key, f.Value);
            }
        }

        #endregion

        /// <summary>
        /// Interface for an individual type of job in the <see cref="FtpFileUploader"/>. Jobs are placed in a queue
        /// and are executed by the <see cref="FtpFileUploader"/>'s worker threads in the <see cref="WorkerThreadLoop"/>.
        /// </summary>
        interface IFtpFileUploaderJob
        {
            /// <summary>
            /// Gets the number of times this job has been attempted.
            /// </summary>
            byte Attempts { get; }

            /// <summary>
            /// Gets if this <see cref="IFtpFileUploaderJob"/> is considered the same as another <see cref="IFtpFileUploaderJob"/>.
            /// The implementation depends on the context, but generally it means that they are of the same type and that the
            /// <paramref name="otherJob"/> operates on the same remote path as this job.
            /// </summary>
            /// <param name="otherJob">The <see cref="IFtpFileUploaderJob"/> to compare against.</param>
            /// <returns>True if the jobs are the same; otherwise false.</returns>
            bool AreJobsSame(IFtpFileUploaderJob otherJob);

            /// <summary>
            /// Executes the job.
            /// </summary>
            /// <param name="parent">The <see cref="FtpFileUploader"/> the job is being executed on.</param>
            void Execute(FtpFileUploader parent);
        }

        /// <summary>
        /// A directory deletion job. Deletes a directory from the Ftp server.
        /// </summary>
        class JobDeleteDir : IFtpFileUploaderJob
        {
            readonly string _remotePath;

            byte _attempts = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="JobDeleteDir"/> class.
            /// </summary>
            /// <param name="remotePath">The remote path to delete.</param>
            public JobDeleteDir(string remotePath)
            {
                _remotePath = SanitizeFtpTargetPath(remotePath);
            }

            /// <summary>
            /// Gets the remote path to delete.
            /// </summary>
            public string RemotePath
            {
                get { return _remotePath; }
            }

            #region IFtpFileUploaderJob Members

            /// <summary>
            /// Gets the number of times this job has been attempted.
            /// </summary>
            public byte Attempts
            {
                get { return _attempts; }
            }

            /// <summary>
            /// Gets if this <see cref="IFtpFileUploaderJob"/> is considered the same as another <see cref="IFtpFileUploaderJob"/>.
            /// The implementation depends on the context, but generally it means that they are of the same type and that the
            /// <paramref name="otherJob"/> operates on the same remote path as this job.
            /// </summary>
            /// <param name="otherJob">The <see cref="IFtpFileUploaderJob"/> to compare against.</param>
            /// <returns>True if the jobs are the same; otherwise false.</returns>
            public bool AreJobsSame(IFtpFileUploaderJob otherJob)
            {
                var o = otherJob as JobDeleteDir;
                if (o == null)
                    return false;

                return StringComparer.Ordinal.Equals(RemotePath, o.RemotePath);
            }

            /// <summary>
            /// Executes the job.
            /// </summary>
            /// <param name="parent">The <see cref="FtpFileUploader"/> the job is being executed on.</param>
            public void Execute(FtpFileUploader parent)
            {
                if (_attempts < byte.MaxValue)
                    _attempts++;

                parent.FtpDeleteDir(RemotePath);
            }

            #endregion
        }

        /// <summary>
        /// A file download job. Downloads a single file from the Ftp server to the local file system.
        /// </summary>
        class JobDownloadFile : IFtpFileUploaderJob
        {
            readonly string _localFile;
            readonly string _remoteFile;

            byte _attempts = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="JobDownloadFile"/> class.
            /// </summary>
            /// <param name="remoteFile">The relative path on the server to the file to download.</param>
            /// <param name="localFile">The full local path to download the file to.</param>
            public JobDownloadFile(string remoteFile, string localFile)
            {
                _remoteFile = SanitizeFtpTargetPath(remoteFile);
                _localFile = localFile;
            }

            /// <summary>
            /// Gets the full local path to download the file to.
            /// </summary>
            public string LocalFile
            {
                get { return _localFile; }
            }

            /// <summary>
            /// Gets the relative path on the server to the file to download.
            /// </summary>
            public string RemoteFile
            {
                get { return _remoteFile; }
            }

            #region IFtpFileUploaderJob Members

            /// <summary>
            /// Gets the number of times this job has been attempted.
            /// </summary>
            public byte Attempts
            {
                get { return _attempts; }
            }

            /// <summary>
            /// Gets if this <see cref="IFtpFileUploaderJob"/> is considered the same as another <see cref="IFtpFileUploaderJob"/>.
            /// The implementation depends on the context, but generally it means that they are of the same type and that the
            /// <paramref name="otherJob"/> operates on the same remote path as this job.
            /// </summary>
            /// <param name="otherJob">The <see cref="IFtpFileUploaderJob"/> to compare against.</param>
            /// <returns>True if the jobs are the same; otherwise false.</returns>
            public bool AreJobsSame(IFtpFileUploaderJob otherJob)
            {
                var o = otherJob as JobDownloadFile;
                if (o == null)
                    return false;

                return StringComparer.Ordinal.Equals(LocalFile, o.LocalFile);
            }

            /// <summary>
            /// Executes the job.
            /// </summary>
            /// <param name="parent">The <see cref="FtpFileUploader"/> the job is being executed on.</param>
            public void Execute(FtpFileUploader parent)
            {
                if (_attempts < byte.MaxValue)
                    _attempts++;

                // Ensure the local directory exists
                var p = Path.GetDirectoryName(LocalFile);
                if (p != null)
                {
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                }

                // Attempt to create the file before downloading anything
                if (File.Exists(LocalFile))
                    File.Delete(LocalFile);

                using (File.Create(p))
                {
                }

                // Start the download now that we know we are able to create the file
                var data = parent.FtpDownload(RemoteFile, true);

                // Write to the file
                if (File.Exists(LocalFile))
                    File.Delete(LocalFile);

                File.WriteAllBytes(LocalFile, data);
            }

            #endregion
        }

        /// <summary>
        /// A file upload job. Uploads a single file from the local machine to the Ftp server.
        /// </summary>
        class JobUploadFile : IFtpFileUploaderJob
        {
            readonly string _localFile;
            readonly string _remoteFile;

            byte _attempts = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="JobUploadFile"/> class.
            /// </summary>
            /// <param name="localFile">The full path to the local file to upload.</param>
            /// <param name="remoteFile">The relative path on the server to upload the file to.</param>
            public JobUploadFile(string localFile, string remoteFile)
            {
                _localFile = localFile;
                _remoteFile = SanitizeFtpTargetPath(remoteFile);
            }

            /// <summary>
            /// Gets the full path to the local file to upload.
            /// </summary>
            public string LocalFile
            {
                get { return _localFile; }
            }

            /// <summary>
            /// Gets the relative path on the server to upload the file to.
            /// </summary>
            public string RemoteFile
            {
                get { return _remoteFile; }
            }

            #region IFtpFileUploaderJob Members

            /// <summary>
            /// Gets the number of times this job has been attempted.
            /// </summary>
            public byte Attempts
            {
                get { return _attempts; }
            }

            /// <summary>
            /// Gets if this <see cref="IFtpFileUploaderJob"/> is considered the same as another <see cref="IFtpFileUploaderJob"/>.
            /// The implementation depends on the context, but generally it means that they are of the same type and that the
            /// <paramref name="otherJob"/> operates on the same remote path as this job.
            /// </summary>
            /// <param name="otherJob">The <see cref="IFtpFileUploaderJob"/> to compare against.</param>
            /// <returns>True if the jobs are the same; otherwise false.</returns>
            public bool AreJobsSame(IFtpFileUploaderJob otherJob)
            {
                var o = otherJob as JobUploadFile;
                if (o == null)
                    return false;

                return StringComparer.Ordinal.Equals(RemoteFile, o.RemoteFile);
            }

            /// <summary>
            /// Executes the job.
            /// </summary>
            /// <param name="parent">The <see cref="FtpFileUploader"/> the job is being executed on.</param>
            public void Execute(FtpFileUploader parent)
            {
                if (_attempts < byte.MaxValue)
                    _attempts++;

                parent.FtpCreateFile(LocalFile, RemoteFile);
            }

            #endregion
        }
    }
}