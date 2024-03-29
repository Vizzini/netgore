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

using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace log4net.Util
{
    /// <summary>
    /// Utility class for transforming strings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Utility class for transforming strings.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    public sealed class Transform
    {
        #region Private Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform" /> class. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// Uses a private access modifier to prevent instantiation of this class.
        /// </para>
        /// </remarks>
        Transform()
        {
        }

        #endregion Private Instance Constructors

        #region XML String Methods

        /// <summary>
        /// Replace invalid XML characters in text string
        /// </summary>
        /// <param name="textData">the XML text input string</param>
        /// <param name="mask">the string to use in place of invalid characters</param>
        /// <returns>A string that does not contain invalid XML characters.</returns>
        /// <remarks>
        /// <para>
        /// Certain Unicode code points are not allowed in the XML InfoSet, for
        /// details see: <a href="http://www.w3.org/TR/REC-xml/#charsets">http://www.w3.org/TR/REC-xml/#charsets</a>.
        /// </para>
        /// <para>
        /// This method replaces any illegal characters in the input string
        /// with the mask string specified.
        /// </para>
        /// </remarks>
        public static string MaskXmlInvalidCharacters(string textData, string mask)
        {
            return INVALIDCHARS.Replace(textData, mask);
        }

        /// <summary>
        /// Write a string to an <see cref="XmlWriter"/>
        /// </summary>
        /// <param name="writer">the writer to write to</param>
        /// <param name="textData">the string to write</param>
        /// <param name="invalidCharReplacement">The string to replace non XML compliant chars with</param>
        /// <remarks>
        /// <para>
        /// The test is escaped either using XML escape entities
        /// or using CDATA sections.
        /// </para>
        /// </remarks>
        public static void WriteEscapedXmlString(XmlWriter writer, string textData, string invalidCharReplacement)
        {
            var stringData = MaskXmlInvalidCharacters(textData, invalidCharReplacement);
            // Write either escaped text or CDATA sections

            var weightCData = 12 * (1 + CountSubstrings(stringData, CDATA_END));
            var weightStringEscapes = 3 * (CountSubstrings(stringData, "<") + CountSubstrings(stringData, ">")) +
                                      4 * CountSubstrings(stringData, "&");

            if (weightStringEscapes <= weightCData)
            {
                // Write string using string escapes
                writer.WriteString(stringData);
            }
            else
            {
                // Write string using CDATA section

                var end = stringData.IndexOf(CDATA_END);

                if (end < 0)
                    writer.WriteCData(stringData);
                else
                {
                    var start = 0;
                    while (end > -1)
                    {
                        writer.WriteCData(stringData.Substring(start, end - start));
                        if (end == stringData.Length - 3)
                        {
                            start = stringData.Length;
                            writer.WriteString(CDATA_END);
                            break;
                        }
                        else
                        {
                            writer.WriteString(CDATA_UNESCAPABLE_TOKEN);
                            start = end + 2;
                            end = stringData.IndexOf(CDATA_END, start);
                        }
                    }

                    if (start < stringData.Length)
                        writer.WriteCData(stringData.Substring(start));
                }
            }
        }

        #endregion XML String Methods

        #region Private Helper Methods

        /// <summary>
        /// Count the number of times that the substring occurs in the text
        /// </summary>
        /// <param name="text">the text to search</param>
        /// <param name="substring">the substring to find</param>
        /// <returns>the number of times the substring occurs in the text</returns>
        /// <remarks>
        /// <para>
        /// The substring is assumed to be non repeating within itself.
        /// </para>
        /// </remarks>
        static int CountSubstrings(string text, string substring)
        {
            var count = 0;
            var offset = 0;
            var length = text.Length;
            var substringLength = substring.Length;

            if (length == 0)
                return 0;
            if (substringLength == 0)
                return 0;

            while (offset < length)
            {
                var index = text.IndexOf(substring, offset);

                if (index == -1)
                    break;

                count++;
                offset = index + substringLength;
            }
            return count;
        }

        #endregion

        #region Private Static Fields

        const string CDATA_END = "]]>";
        const string CDATA_UNESCAPABLE_TOKEN = "]]";

        static readonly Regex INVALIDCHARS = new Regex(@"[^\x09\x0A\x0D\x20-\xFF\u00FF-\u07FF\uE000-\uFFFD]",
            RegexOptions.Compiled);

        #endregion Private Static Fields
    }
}