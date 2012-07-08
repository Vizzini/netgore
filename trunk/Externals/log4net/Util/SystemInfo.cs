#region Copyright & License

//
// Copyright 2001-2006 The Apache Software Foundation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Threading;

namespace log4net.Util
{
    /// <summary>
    /// Utility class for system specific information.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Utility class of static methods for system specific information.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    /// <author>Alexey Solofnenko</author>
    public sealed class SystemInfo
    {
        #region Private Constants

        const string DEFAULT_NOT_AVAILABLE_TEXT = "NOT AVAILABLE";
        const string DEFAULT_NULL_TEXT = "(null)";

        #endregion

        #region Private Instance Constructors

        /// <summary>
        /// Private constructor to prevent instances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Only static methods are exposed from this type.
        /// </para>
        /// </remarks>
        SystemInfo()
        {
        }

        #endregion Private Instance Constructors

        #region Public Static Constructor

        /// <summary>
        /// Initialize default values for private static fields.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Only static methods are exposed from this type.
        /// </para>
        /// </remarks>
        static SystemInfo()
        {
            var nullText = DEFAULT_NULL_TEXT;
            var notAvailableText = DEFAULT_NOT_AVAILABLE_TEXT;

            // Look for log4net.NullText in AppSettings
            var nullTextAppSettingsKey = GetAppSetting("log4net.NullText");
            if (nullTextAppSettingsKey != null && nullTextAppSettingsKey.Length > 0)
            {
                LogLog.Debug("SystemInfo: Initializing NullText value to [" + nullTextAppSettingsKey + "].");
                nullText = nullTextAppSettingsKey;
            }

            // Look for log4net.NotAvailableText in AppSettings
            var notAvailableTextAppSettingsKey = GetAppSetting("log4net.NotAvailableText");
            if (notAvailableTextAppSettingsKey != null && notAvailableTextAppSettingsKey.Length > 0)
            {
                LogLog.Debug("SystemInfo: Initializing NotAvailableText value to [" + notAvailableTextAppSettingsKey + "].");
                notAvailableText = notAvailableTextAppSettingsKey;
            }

            s_notAvailableText = notAvailableText;
            s_nullText = nullText;
        }

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Gets the base directory for this <see cref="AppDomain"/>.
        /// </summary>
        /// <value>The base directory path for the current <see cref="AppDomain"/>.</value>
        /// <remarks>
        /// <para>
        /// Gets the base directory for this <see cref="AppDomain"/>.
        /// </para>
        /// <para>
        /// The value returned may be either a local file path or a URI.
        /// </para>
        /// </remarks>
        public static string ApplicationBaseDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        /// <summary>
        /// Get this application's friendly name
        /// </summary>
        /// <value>
        /// The friendly name of this application as a string
        /// </value>
        /// <remarks>
        /// <para>
        /// If available the name of the application is retrieved from
        /// the <c>AppDomain</c> using <c>AppDomain.CurrentDomain.FriendlyName</c>.
        /// </para>
        /// <para>
        /// Otherwise the file name of the entry assembly is used.
        /// </para>
        /// </remarks>
        public static string ApplicationFriendlyName
        {
            get
            {
                if (s_appFriendlyName == null)
                {
                    try
                    {
                        s_appFriendlyName = AppDomain.CurrentDomain.FriendlyName;
                    }
                    catch (SecurityException)
                    {
                        // This security exception will occur if the caller does not have 
                        // some undefined set of SecurityPermission flags.
                        LogLog.Debug(
                            "SystemInfo: Security exception while trying to get current domain friendly name. Error Ignored.");
                    }

                    if (s_appFriendlyName == null || s_appFriendlyName.Length == 0)
                    {
                        try
                        {
                            var assemblyLocation = EntryAssemblyLocation;
                            s_appFriendlyName = Path.GetFileName(assemblyLocation);
                        }
                        catch (SecurityException)
                        {
                            // Caller needs path discovery permission
                        }
                    }

                    if (s_appFriendlyName == null || s_appFriendlyName.Length == 0)
                        s_appFriendlyName = s_notAvailableText;
                }
                return s_appFriendlyName;
            }
        }

        /// <summary>
        /// Gets the path to the configuration file for the current <see cref="AppDomain"/>.
        /// </summary>
        /// <value>The path to the configuration file for the current <see cref="AppDomain"/>.</value>
        /// <remarks>
        /// <para>
        /// The .NET Compact Framework 1.0 does not have a concept of a configuration
        /// file. For this runtime, we use the entry assembly location as the root for
        /// the configuration file name.
        /// </para>
        /// <para>
        /// The value returned may be either a local file path or a URI.
        /// </para>
        /// </remarks>
        public static string ConfigurationFileLocation
        {
            get { return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile; }
        }

        /// <summary>
        /// Gets the ID of the current thread.
        /// </summary>
        /// <value>The ID of the current thread.</value>
        /// <remarks>
        /// <para>
        /// On the .NET framework, the <c>AppDomain.GetCurrentThreadId</c> method
        /// is used to obtain the thread ID for the current thread. This is the 
        /// operating system ID for the thread.
        /// </para>
        /// <para>
        /// On the .NET Compact Framework 1.0 it is not possible to get the 
        /// operating system thread ID for the current thread. The native method 
        /// <c>GetCurrentThreadId</c> is implemented inline in a header file
        /// and cannot be called.
        /// </para>
        /// <para>
        /// On the .NET Framework 2.0 the <c>Thread.ManagedThreadId</c> is used as this
        /// gives a stable id unrelated to the operating system thread ID which may 
        /// change if the runtime is using fibers.
        /// </para>
        /// </remarks>
        public static int CurrentThreadId
        {
            get { return Thread.CurrentThread.ManagedThreadId; }
        }

        /// <summary>
        /// Gets the path to the file that first executed in the current <see cref="AppDomain"/>.
        /// </summary>
        /// <value>The path to the entry assembly.</value>
        /// <remarks>
        /// <para>
        /// Gets the path to the file that first executed in the current <see cref="AppDomain"/>.
        /// </para>
        /// </remarks>
        public static string EntryAssemblyLocation
        {
            get { return Assembly.GetEntryAssembly().Location; }
        }

        /// <summary>
        /// Get the host name or machine name for the current machine
        /// </summary>
        /// <value>
        /// The hostname or machine name
        /// </value>
        /// <remarks>
        /// <para>
        /// Get the host name or machine name for the current machine
        /// </para>
        /// <para>
        /// The host name (<see cref="System.Net.Dns.GetHostName"/>) or
        /// the machine name (<c>Environment.MachineName</c>) for
        /// the current machine, or if neither of these are available
        /// then <c>NOT AVAILABLE</c> is returned.
        /// </para>
        /// </remarks>
        public static string HostName
        {
            get
            {
                if (s_hostName == null)
                {
                    // Get the DNS host name of the current machine
                    try
                    {
                        // Lookup the host name
                        s_hostName = Dns.GetHostName();
                    }
                    catch (SocketException)
                    {
                    }
                    catch (SecurityException)
                    {
                        // We may get a security exception looking up the hostname
                        // You must have Unrestricted DnsPermission to access resource
                    }

                    // Get the NETBIOS machine name of the current machine
                    if (s_hostName == null || s_hostName.Length == 0)
                    {
                        try
                        {
                            s_hostName = Environment.MachineName;
                        }
                        catch (InvalidOperationException)
                        {
                        }
                        catch (SecurityException)
                        {
                            // We may get a security exception looking up the machine name
                            // You must have Unrestricted EnvironmentPermission to access resource
                        }
                    }

                    // Couldn't find a value
                    if (s_hostName == null || s_hostName.Length == 0)
                        s_hostName = s_notAvailableText;
                }
                return s_hostName;
            }
        }

        /// <summary>
        /// Gets the system dependent line terminator.
        /// </summary>
        /// <value>
        /// The system dependent line terminator.
        /// </value>
        /// <remarks>
        /// <para>
        /// Gets the system dependent line terminator.
        /// </para>
        /// </remarks>
        public static string NewLine
        {
            get { return Environment.NewLine; }
        }

        /// <summary>
        /// Text to output when an unsupported feature is requested.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Use this value when an unsupported feature is requested.
        /// </para>
        /// <para>
        /// The default value is <c>NOT AVAILABLE</c>. This value can be overridden by specifying
        /// a value for the <c>log4net.NotAvailableText</c> appSetting in the application's
        /// .config file.
        /// </para>
        /// </remarks>
        public static string NotAvailableText
        {
            get { return s_notAvailableText; }
            set { s_notAvailableText = value; }
        }

        /// <summary>
        /// Text to output when a <c>null</c> is encountered.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Use this value to indicate a <c>null</c> has been encountered while
        /// outputting a string representation of an item.
        /// </para>
        /// <para>
        /// The default value is <c>(null)</c>. This value can be overridden by specifying
        /// a value for the <c>log4net.NullText</c> appSetting in the application's
        /// .config file.
        /// </para>
        /// </remarks>
        public static string NullText
        {
            get { return s_nullText; }
            set { s_nullText = value; }
        }

        /// <summary>
        /// Get the start time for the current process.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the time at which the log4net library was loaded into the
        /// AppDomain. Due to reports of a hang in the call to <c>System.Diagnostics.Process.StartTime</c>
        /// this is not the start time for the current process.
        /// </para>
        /// <para>
        /// The log4net library should be loaded by an application early during its
        /// startup, therefore this start time should be a good approximation for
        /// the actual start time.
        /// </para>
        /// <para>
        /// Note that AppDomains may be loaded and unloaded within the
        /// same process without the process terminating, however this start time
        /// will be set per AppDomain.
        /// </para>
        /// </remarks>
        public static DateTime ProcessStartTime
        {
            get { return s_processStartTime; }
        }

        #endregion Public Static Properties

        #region Public Static Methods

        /// <summary>
        /// Gets the file name portion of the <see cref="Assembly" />, including the extension.
        /// </summary>
        /// <param name="myAssembly">The <see cref="Assembly" /> to get the file name for.</param>
        /// <returns>The file name of the assembly.</returns>
        /// <remarks>
        /// <para>
        /// Gets the file name portion of the <see cref="Assembly" />, including the extension.
        /// </para>
        /// </remarks>
        public static string AssemblyFileName(Assembly myAssembly)
        {
            return Path.GetFileName(myAssembly.Location);
        }

        /// <summary>
        /// Gets the assembly location path for the specified assembly.
        /// </summary>
        /// <param name="myAssembly">The assembly to get the location for.</param>
        /// <returns>The location of the assembly.</returns>
        /// <remarks>
        /// <para>
        /// This method does not guarantee to return the correct path
        /// to the assembly. If only tries to give an indication as to
        /// where the assembly was loaded from.
        /// </para>
        /// </remarks>
        public static string AssemblyLocationInfo(Assembly myAssembly)
        {
            if (myAssembly.GlobalAssemblyCache)
                return "Global Assembly Cache";
            else
            {
                try
                {
                    // This call requires FileIOPermission for access to the path
                    // if we don't have permission then we just ignore it and
                    // carry on.
                    return myAssembly.Location;
                }
                catch (SecurityException)
                {
                    return "Location Permission Denied";
                }
            }
        }

        /// <summary>
        /// Gets the fully qualified name of the <see cref="Type" />, including 
        /// the name of the assembly from which the <see cref="Type" /> was 
        /// loaded.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to get the fully qualified name for.</param>
        /// <returns>The fully qualified name for the <see cref="Type" />.</returns>
        /// <remarks>
        /// <para>
        /// This is equivalent to the <c>Type.AssemblyQualifiedName</c> property,
        /// but this method works on the .NET Compact Framework 1.0 as well as
        /// the full .NET runtime.
        /// </para>
        /// </remarks>
        public static string AssemblyQualifiedName(Type type)
        {
            return type.FullName + ", " + type.Assembly.FullName;
        }

        /// <summary>
        /// Gets the short name of the <see cref="Assembly" />.
        /// </summary>
        /// <param name="myAssembly">The <see cref="Assembly" /> to get the name for.</param>
        /// <returns>The short name of the <see cref="Assembly" />.</returns>
        /// <remarks>
        /// <para>
        /// The short name of the assembly is the <see cref="Assembly.FullName" /> 
        /// without the version, culture, or public key. i.e. it is just the 
        /// assembly's file name without the extension.
        /// </para>
        /// <para>
        /// Use this rather than <c>Assembly.GetName().Name</c> because that
        /// is not available on the Compact Framework.
        /// </para>
        /// <para>
        /// Because of a FileIOPermission security demand we cannot do
        /// the obvious Assembly.GetName().Name. We are allowed to get
        /// the <see cref="Assembly.FullName" /> of the assembly so we 
        /// start from there and strip out just the assembly name.
        /// </para>
        /// </remarks>
        public static string AssemblyShortName(Assembly myAssembly)
        {
            var name = myAssembly.FullName;
            var offset = name.IndexOf(',');
            if (offset > 0)
                name = name.Substring(0, offset);
            return name.Trim();

            // TODO: Do we need to unescape the assembly name string? 
            // Doc says '\' is an escape char but has this already been 
            // done by the string loader?
        }

        /// <summary>
        /// Convert a path into a fully qualified local file path.
        /// </summary>
        /// <param name="path">The path to convert.</param>
        /// <returns>The fully qualified path.</returns>
        /// <remarks>
        /// <para>
        /// Converts the path specified to a fully
        /// qualified path. If the path is relative it is
        /// taken as relative from the application base 
        /// directory.
        /// </para>
        /// <para>
        /// The path specified must be a local file path, a URI is not supported.
        /// </para>
        /// </remarks>
        public static string ConvertToFullPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            var baseDirectory = "";
            try
            {
                var applicationBaseDirectory = ApplicationBaseDirectory;
                if (applicationBaseDirectory != null)
                {
                    // applicationBaseDirectory may be a URI not a local file path
                    var applicationBaseDirectoryUri = new Uri(applicationBaseDirectory);
                    if (applicationBaseDirectoryUri.IsFile)
                        baseDirectory = applicationBaseDirectoryUri.LocalPath;
                }
            }
            catch
            {
                // Ignore URI exceptions & SecurityExceptions from SystemInfo.ApplicationBaseDirectory
            }

            if (baseDirectory != null && baseDirectory.Length > 0)
            {
                // Note that Path.Combine will return the second path if it is rooted
                return Path.GetFullPath(Path.Combine(baseDirectory, path));
            }
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Create an <see cref="ArgumentOutOfRangeException"/>
        /// </summary>
        /// <param name="parameterName">The name of the parameter that caused the exception</param>
        /// <param name="actualValue">The value of the argument that causes this exception</param>
        /// <param name="message">The message that describes the error</param>
        /// <returns>the ArgumentOutOfRangeException object</returns>
        /// <remarks>
        /// <para>
        /// Create a new instance of the <see cref="ArgumentOutOfRangeException"/> class 
        /// with a specified error message, the parameter name, and the value 
        /// of the argument.
        /// </para>
        /// <para>
        /// The Compact Framework does not support the 3 parameter constructor for the
        /// <see cref="ArgumentOutOfRangeException"/> type. This method provides an
        /// implementation that works for all platforms.
        /// </para>
        /// </remarks>
        public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string parameterName, object actualValue,
                                                                                    string message)
        {
            return new ArgumentOutOfRangeException(parameterName, actualValue, message);
        }

        /// <summary>
        /// Creates a new case-insensitive instance of the <see cref="Hashtable"/> class with the default initial capacity. 
        /// </summary>
        /// <returns>A new case-insensitive instance of the <see cref="Hashtable"/> class with the default initial capacity</returns>
        /// <remarks>
        /// <para>
        /// The new Hashtable instance uses the default load factor, the CaseInsensitiveHashCodeProvider, and the CaseInsensitiveComparer.
        /// </para>
        /// </remarks>
        public static Hashtable CreateCaseInsensitiveHashtable()
        {
            return CollectionsUtil.CreateCaseInsensitiveHashtable();
        }

        /// <summary>
        /// Lookup an application setting
        /// </summary>
        /// <param name="key">the application settings key to lookup</param>
        /// <returns>the value for the key, or <c>null</c></returns>
        /// <remarks>
        /// <para>
        /// Configuration APIs are not supported under the Compact Framework
        /// </para>
        /// </remarks>
        public static string GetAppSetting(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception ex)
            {
                // If an exception is thrown here then it looks like the config file does not parse correctly.
                LogLog.Error(
                    "DefaultRepositorySelector: Exception while reading ConfigurationSettings. Check your .config file is well formed XML.",
                    ex);
            }
            return null;
        }

        /// <summary>
        /// Loads the type specified in the type string.
        /// </summary>
        /// <param name="relativeType">A sibling type to use to load the type.</param>
        /// <param name="typeName">The name of the type to load.</param>
        /// <param name="throwOnError">Flag set to <c>true</c> to throw an exception if the type cannot be loaded.</param>
        /// <param name="ignoreCase"><c>true</c> to ignore the case of the type name; otherwise, <c>false</c></param>
        /// <returns>The type loaded or <c>null</c> if it could not be loaded.</returns>
        /// <remarks>
        /// <para>
        /// If the type name is fully qualified, i.e. if contains an assembly name in 
        /// the type name, the type will be loaded from the system using 
        /// <see cref="Type.GetType(string,bool)"/>.
        /// </para>
        /// <para>
        /// If the type name is not fully qualified, it will be loaded from the assembly
        /// containing the specified relative type. If the type is not found in the assembly 
        /// then all the loaded assemblies will be searched for the type.
        /// </para>
        /// </remarks>
        public static Type GetTypeFromString(Type relativeType, string typeName, bool throwOnError, bool ignoreCase)
        {
            return GetTypeFromString(relativeType.Assembly, typeName, throwOnError, ignoreCase);
        }

        /// <summary>
        /// Loads the type specified in the type string.
        /// </summary>
        /// <param name="typeName">The name of the type to load.</param>
        /// <param name="throwOnError">Flag set to <c>true</c> to throw an exception if the type cannot be loaded.</param>
        /// <param name="ignoreCase"><c>true</c> to ignore the case of the type name; otherwise, <c>false</c></param>
        /// <returns>The type loaded or <c>null</c> if it could not be loaded.</returns>		
        /// <remarks>
        /// <para>
        /// If the type name is fully qualified, i.e. if contains an assembly name in 
        /// the type name, the type will be loaded from the system using 
        /// <see cref="Type.GetType(string,bool)"/>.
        /// </para>
        /// <para>
        /// If the type name is not fully qualified it will be loaded from the
        /// assembly that is directly calling this method. If the type is not found 
        /// in the assembly then all the loaded assemblies will be searched for the type.
        /// </para>
        /// </remarks>
        public static Type GetTypeFromString(string typeName, bool throwOnError, bool ignoreCase)
        {
            return GetTypeFromString(Assembly.GetCallingAssembly(), typeName, throwOnError, ignoreCase);
        }

        /// <summary>
        /// Loads the type specified in the type string.
        /// </summary>
        /// <param name="relativeAssembly">An assembly to load the type from.</param>
        /// <param name="typeName">The name of the type to load.</param>
        /// <param name="throwOnError">Flag set to <c>true</c> to throw an exception if the type cannot be loaded.</param>
        /// <param name="ignoreCase"><c>true</c> to ignore the case of the type name; otherwise, <c>false</c></param>
        /// <returns>The type loaded or <c>null</c> if it could not be loaded.</returns>
        /// <remarks>
        /// <para>
        /// If the type name is fully qualified, i.e. if contains an assembly name in 
        /// the type name, the type will be loaded from the system using 
        /// <see cref="Type.GetType(string,bool)"/>.
        /// </para>
        /// <para>
        /// If the type name is not fully qualified it will be loaded from the specified
        /// assembly. If the type is not found in the assembly then all the loaded assemblies 
        /// will be searched for the type.
        /// </para>
        /// </remarks>
        public static Type GetTypeFromString(Assembly relativeAssembly, string typeName, bool throwOnError, bool ignoreCase)
        {
            // Check if the type name specifies the assembly name
            if (typeName.IndexOf(',') == -1)
            {
                //LogLog.Debug("SystemInfo: Loading type ["+typeName+"] from assembly ["+relativeAssembly.FullName+"]");

                // Attempt to lookup the type from the relativeAssembly
                var type = relativeAssembly.GetType(typeName, false, ignoreCase);
                if (type != null)
                {
                    // Found type in relative assembly
                    //LogLog.Debug("SystemInfo: Loaded type ["+typeName+"] from assembly ["+relativeAssembly.FullName+"]");
                    return type;
                }

                Assembly[] loadedAssemblies = null;
                try
                {
                    loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                }
                catch (SecurityException)
                {
                    // Insufficient permissions to get the list of loaded assemblies
                }

                if (loadedAssemblies != null)
                {
                    // Search the loaded assemblies for the type
                    foreach (var assembly in loadedAssemblies)
                    {
                        type = assembly.GetType(typeName, false, ignoreCase);
                        if (type != null)
                        {
                            // Found type in loaded assembly
                            LogLog.Debug("SystemInfo: Loaded type [" + typeName + "] from assembly [" + assembly.FullName +
                                         "] by searching loaded assemblies.");
                            return type;
                        }
                    }
                }

                // Didn't find the type
                if (throwOnError)
                {
                    throw new TypeLoadException("Could not load type [" + typeName + "]. Tried assembly [" +
                                                relativeAssembly.FullName + "] and all loaded assemblies");
                }
                return null;
            }
            else
            {
                // Includes explicit assembly name
                //LogLog.Debug("SystemInfo: Loading type ["+typeName+"] from global Type");
                return Type.GetType(typeName, throwOnError, ignoreCase);
            }
        }

        /// <summary>
        /// Generate a new guid
        /// </summary>
        /// <returns>A new Guid</returns>
        /// <remarks>
        /// <para>
        /// Generate a new guid
        /// </para>
        /// </remarks>
        public static Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Parse a string into an <see cref="Int32"/> value
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="val">out param where the parsed value is placed</param>
        /// <returns><c>true</c> if the string was able to be parsed into an integer</returns>
        /// <remarks>
        /// <para>
        /// Attempts to parse the string into an integer. If the string cannot
        /// be parsed then this method returns <c>false</c>. The method does not throw an exception.
        /// </para>
        /// </remarks>
        public static bool TryParse(string s, out int val)
        {
            // Initialise out param
            val = 0;

            try
            {
                double doubleVal;
                if (Double.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out doubleVal))
                {
                    val = Convert.ToInt32(doubleVal);
                    return true;
                }
            }
            catch
            {
                // Ignore exception, just return false
            }

            return false;
        }

        /// <summary>
        /// Parse a string into an <see cref="Int64"/> value
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <param name="val">out param where the parsed value is placed</param>
        /// <returns><c>true</c> if the string was able to be parsed into an integer</returns>
        /// <remarks>
        /// <para>
        /// Attempts to parse the string into an integer. If the string cannot
        /// be parsed then this method returns <c>false</c>. The method does not throw an exception.
        /// </para>
        /// </remarks>
        public static bool TryParse(string s, out long val)
        {
            // Initialise out param
            val = 0;

            try
            {
                double doubleVal;
                if (Double.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out doubleVal))
                {
                    val = Convert.ToInt64(doubleVal);
                    return true;
                }
            }
            catch
            {
                // Ignore exception, just return false
            }

            return false;
        }

        #endregion Public Static Methods

        #region Private Static Methods

        #endregion Private Static Methods

        #region Public Static Fields

        /// <summary>
        /// Gets an empty array of types.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <c>Type.EmptyTypes</c> field is not available on
        /// the .NET Compact Framework 1.0.
        /// </para>
        /// </remarks>
        public static readonly Type[] EmptyTypes = new Type[0];

        #endregion Public Static Fields

        #region Private Static Fields

        /// <summary>
        /// Start time for the current process.
        /// </summary>
        static readonly DateTime s_processStartTime = DateTime.Now;

        /// <summary>
        /// Cache the application friendly name
        /// </summary>
        static string s_appFriendlyName;

        /// <summary>
        /// Cache the host name for the current machine
        /// </summary>
        static string s_hostName;

        /// <summary>
        /// Text to output when an unsupported feature is requested.
        /// </summary>
        static string s_notAvailableText;

        /// <summary>
        /// Text to output when a <c>null</c> is encountered.
        /// </summary>
        static string s_nullText;

        #endregion
    }
}