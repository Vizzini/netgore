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

using System.IO;
using System.Linq;
using log4net.Core;
using log4net.Util;

namespace log4net.Layout.Pattern
{
    /// <summary>
    /// Converter to output and truncate <c>'.'</c> separated strings
    /// </summary>
    /// <remarks>
    /// <para>
    /// This abstract class supports truncating a <c>'.'</c> separated string
    /// to show a specified number of elements from the right hand side.
    /// This is used to truncate class names that are fully qualified.
    /// </para>
    /// <para>
    /// Subclasses should override the <see cref="GetFullyQualifiedName"/> method to
    /// return the fully qualified string.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    abstract class NamedPatternConverter : PatternLayoutConverter, IOptionHandler
    {
        protected int m_precision = 0;

        #region Implementation of IOptionHandler

        /// <summary>
        /// Initialize the converter 
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="IOptionHandler"/> delayed object
        /// activation scheme. The <see cref="ActivateOptions"/> method must 
        /// be called on this object after the configuration properties have
        /// been set. Until <see cref="ActivateOptions"/> is called this
        /// object is in an undefined state and must not be used. 
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then 
        /// <see cref="ActivateOptions"/> must be called again.
        /// </para>
        /// </remarks>
        public void ActivateOptions()
        {
            m_precision = 0;

            if (Option != null)
            {
                var optStr = Option.Trim();
                if (optStr.Length > 0)
                {
                    int precisionVal;
                    if (SystemInfo.TryParse(optStr, out precisionVal))
                    {
                        if (precisionVal <= 0)
                            LogLog.Error("NamedPatternConverter: Precision option (" + optStr + ") isn't a positive integer.");
                        else
                            m_precision = precisionVal;
                    }
                    else
                        LogLog.Error("NamedPatternConverter: Precision option \"" + optStr + "\" not a decimal integer.");
                }
            }
        }

        #endregion

        /// <summary>
        /// Convert the pattern to the rendered message
        /// </summary>
        /// <param name="writer"><see cref="TextWriter" /> that will receive the formatted result.</param>
        /// <param name="loggingEvent">the event being logged</param>
        /// <remarks>
        /// Render the <see cref="GetFullyQualifiedName"/> to the precision
        /// specified by the <see cref="PatternConverter.Option"/> property.
        /// </remarks>
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            var name = GetFullyQualifiedName(loggingEvent);
            if (m_precision <= 0)
                writer.Write(name);
            else
            {
                var len = name.Length;

                // We subtract 1 from 'len' when assigning to 'end' to avoid out of
                // bounds exception in return name.Substring(end+1, len). This can happen if
                // precision is 1 and the logger name ends with a dot. 
                var end = len - 1;
                for (var i = m_precision; i > 0; i--)
                {
                    end = name.LastIndexOf('.', end - 1);
                    if (end == -1)
                    {
                        writer.Write(name);
                        return;
                    }
                }
                writer.Write(name.Substring(end + 1, len - end - 1));
            }
        }

        /// <summary>
        /// Get the fully qualified string data
        /// </summary>
        /// <param name="loggingEvent">the event being logged</param>
        /// <returns>the fully qualified name</returns>
        /// <remarks>
        /// <para>
        /// Overridden by subclasses to get the fully qualified name before the
        /// precision is applied to it.
        /// </para>
        /// <para>
        /// Return the fully qualified <c>'.'</c> (dot/period) separated string.
        /// </para>
        /// </remarks>
        protected abstract string GetFullyQualifiedName(LoggingEvent loggingEvent);
    }
}