#region Copyright & License

//
// Copyright 2001-2005 The Apache Software Foundation
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
using System.IO;
using System.Linq;
using System.Security;

namespace log4net.Util.PatternStringConverters
{
    /// <summary>
    /// Write an environment variable to the output
    /// </summary>
    /// <remarks>
    /// <para>
    /// Write an environment variable to the output writer.
    /// The value of the <see cref="log4net.Util.PatternConverter.Option"/> determines 
    /// the name of the variable to output.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    sealed class EnvironmentPatternConverter : PatternConverter
    {
        /// <summary>
        /// Write an environment variable to the output
        /// </summary>
        /// <param name="writer">the writer to write to</param>
        /// <param name="state">null, state is not set</param>
        /// <remarks>
        /// <para>
        /// Writes the environment variable to the output <paramref name="writer"/>.
        /// The name of the environment variable to output must be set
        /// using the <see cref="log4net.Util.PatternConverter.Option"/>
        /// property.
        /// </para>
        /// </remarks>
        protected override void Convert(TextWriter writer, object state)
        {
            try
            {
                if (Option != null && Option.Length > 0)
                {
                    // Lookup the environment variable
                    var envValue = Environment.GetEnvironmentVariable(Option);
                    if (envValue != null && envValue.Length > 0)
                        writer.Write(envValue);
                }
            }
            catch (SecurityException secEx)
            {
                // This security exception will occur if the caller does not have 
                // unrestricted environment permission. If this occurs the expansion 
                // will be skipped with the following warning message.
                LogLog.Debug(
                    "EnvironmentPatternConverter: Security exception while trying to expand environment variables. Error Ignored. No Expansion.",
                    secEx);
            }
            catch (Exception ex)
            {
                LogLog.Error("EnvironmentPatternConverter: Error occurred while converting environment variable.", ex);
            }
        }
    }
}