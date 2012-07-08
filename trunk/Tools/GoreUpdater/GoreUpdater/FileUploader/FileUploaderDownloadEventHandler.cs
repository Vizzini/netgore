﻿using System.Linq;

namespace GoreUpdater
{
    /// <summary>
    /// Delegate for handling download events from the <see cref="IFileUploader"/>.
    /// </summary>
    /// <param name="sender">The <see cref="IFileUploader"/> that the event came from.</param>
    /// <param name="localFile">The local file for the job that finished.</param>
    /// <param name="remoteFile">The remote file for the job that finished.</param>
    public delegate void FileUploaderDownloadEventHandler(IFileUploader sender, string localFile, string remoteFile);
}