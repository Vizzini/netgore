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
using log4net.Core;
using log4net.DateFormatter;
using log4net.Util;

namespace log4net.Layout.Pattern
{
    /// <summary>
    /// Write the TimeStamp to the output
    /// </summary>
    /// <remarks>
    /// <para>
    /// Date pattern converter, uses a <see cref="IDateFormatter"/> to format 
    /// the date of a <see cref="LoggingEvent"/>.
    /// </para>
    /// <para>
    /// Uses a <see cref="IDateFormatter"/> to format the <see cref="LoggingEvent.TimeStamp"/> 
    /// in Universal time.
    /// </para>
    /// <para>
    /// See the <see cref="DatePatternConverter"/> for details on the date pattern syntax.
    /// </para>
    /// </remarks>
    /// <seealso cref="DatePatternConverter"/>
    /// <author>Nicko Cadell</author>
    class UtcDatePatternConverter : DatePatternConverter
    {
        /// <summary>
        /// Write the TimeStamp to the output
        /// </summary>
        /// <param name="writer"><see cref="TextWriter" /> that will receive the formatted result.</param>
        /// <param name="loggingEvent">the event being logged</param>
        /// <remarks>
        /// <para>
        /// Pass the <see cref="LoggingEvent.TimeStamp"/> to the <see cref="IDateFormatter"/>
        /// for it to render it to the writer.
        /// </para>
        /// <para>
        /// The <see cref="LoggingEvent.TimeStamp"/> passed is in the local time zone, this is converted
        /// to Universal time before it is rendered.
        /// </para>
        /// </remarks>
        /// <seealso cref="DatePatternConverter"/>
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            try
            {
                m_dateFormatter.FormatDate(loggingEvent.TimeStamp.ToUniversalTime(), writer);
            }
            catch (Exception ex)
            {
                LogLog.Error("UtcDatePatternConverter: Error occurred while converting date.", ex);
            }
        }
    }
}