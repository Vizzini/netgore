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
using System.Globalization;
using System.Linq;
using System.Text;

namespace log4net.DateFormatter
{
    /// <summary>
    /// Formats a <see cref="DateTime"/> as <c>"dd MMM yyyy HH:mm:ss,fff"</c>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Formats a <see cref="DateTime"/> in the format 
    /// <c>"dd MMM yyyy HH:mm:ss,fff"</c> for example, 
    /// <c>"06 Nov 1994 15:49:37,459"</c>.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    /// <author>Angelika Schnagl</author>
    public class DateTimeDateFormatter : AbsoluteTimeDateFormatter
    {
        #region Public Instance Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Initializes a new instance of the <see cref="DateTimeDateFormatter" /> class.
        /// </para>
        /// </remarks>
        public DateTimeDateFormatter()
        {
            m_dateTimeFormatInfo = DateTimeFormatInfo.InvariantInfo;
        }

        #endregion Public Instance Constructors

        #region Override implementation of AbsoluteTimeDateFormatter

        /// <summary>
        /// Formats the date without the milliseconds part
        /// </summary>
        /// <param name="dateToFormat">The date to format.</param>
        /// <param name="buffer">The string builder to write to.</param>
        /// <remarks>
        /// <para>
        /// Formats a DateTime in the format <c>"dd MMM yyyy HH:mm:ss"</c>
        /// for example, <c>"06 Nov 1994 15:49:37"</c>.
        /// </para>
        /// <para>
        /// The base class will append the <c>",fff"</c> milliseconds section.
        /// This method will only be called at most once per second.
        /// </para>
        /// </remarks>
        protected override void FormatDateWithoutMillis(DateTime dateToFormat, StringBuilder buffer)
        {
            var day = dateToFormat.Day;
            if (day < 10)
                buffer.Append('0');
            buffer.Append(day);
            buffer.Append(' ');

            buffer.Append(m_dateTimeFormatInfo.GetAbbreviatedMonthName(dateToFormat.Month));
            buffer.Append(' ');

            buffer.Append(dateToFormat.Year);
            buffer.Append(' ');

            // Append the 'HH:mm:ss'
            base.FormatDateWithoutMillis(dateToFormat, buffer);
        }

        #endregion Override implementation of AbsoluteTimeDateFormatter

        #region Private Instance Fields

        /// <summary>
        /// The format info for the invariant culture.
        /// </summary>
        readonly DateTimeFormatInfo m_dateTimeFormatInfo;

        #endregion Private Instance Fields
    }
}