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
using System.Collections;
using System.Globalization;
using System.Linq;
using log4net.Core;
using log4net.Layout;

namespace log4net.Util
{
    /// <summary>
    /// Most of the work of the <see cref="PatternLayout"/> class
    /// is delegated to the PatternParser class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>PatternParser</c> processes a pattern string and
    /// returns a chain of <see cref="PatternConverter"/> objects.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    public sealed class PatternParser
    {
        #region Public Instance Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pattern">The pattern to parse.</param>
        /// <remarks>
        /// <para>
        /// Initializes a new instance of the <see cref="PatternParser" /> class 
        /// with the specified pattern string.
        /// </para>
        /// </remarks>
        public PatternParser(string pattern)
        {
            m_pattern = pattern;
        }

        #endregion Public Instance Constructors

        #region Public Instance Methods

        /// <summary>
        /// Parses the pattern into a chain of pattern converters.
        /// </summary>
        /// <returns>The head of a chain of pattern converters.</returns>
        /// <remarks>
        /// <para>
        /// Parses the pattern into a chain of pattern converters.
        /// </para>
        /// </remarks>
        public PatternConverter Parse()
        {
            var converterNamesCache = BuildCache();

            ParseInternal(m_pattern, converterNamesCache);

            return m_head;
        }

        #endregion Public Instance Methods

        #region Public Instance Properties

        /// <summary>
        /// Get the converter registry used by this parser
        /// </summary>
        /// <value>
        /// The converter registry used by this parser
        /// </value>
        /// <remarks>
        /// <para>
        /// Get the converter registry used by this parser
        /// </para>
        /// </remarks>
        public Hashtable PatternConverters
        {
            get { return m_patternConverters; }
        }

        #endregion Public Instance Properties

        #region Protected Instance Methods

        /// <summary>
        /// Resets the internal state of the parser and adds the specified pattern converter 
        /// to the chain.
        /// </summary>
        /// <param name="pc">The pattern converter to add.</param>
        void AddConverter(PatternConverter pc)
        {
            // Add the pattern converter to the list.

            if (m_head == null)
                m_head = m_tail = pc;
            else
            {
                // Set the next converter on the tail
                // Update the tail reference
                // note that a converter may combine the 'next' into itself
                // and therefore the tail would not change!
                m_tail = m_tail.SetNext(pc);
            }
        }

        /// <summary>
        /// Build the unified cache of converters from the static and instance maps
        /// </summary>
        /// <returns>the list of all the converter names</returns>
        /// <remarks>
        /// <para>
        /// Build the unified cache of converters from the static and instance maps
        /// </para>
        /// </remarks>
        string[] BuildCache()
        {
            var converterNamesCache = new string[m_patternConverters.Keys.Count];
            m_patternConverters.Keys.CopyTo(converterNamesCache, 0);

            // sort array so that longer strings come first
            Array.Sort(converterNamesCache, 0, converterNamesCache.Length, StringLengthComparer.Instance);

            return converterNamesCache;
        }

        /// <summary>
        /// Internal method to parse the specified pattern to find specified matches
        /// </summary>
        /// <param name="pattern">the pattern to parse</param>
        /// <param name="matches">the converter names to match in the pattern</param>
        /// <remarks>
        /// <para>
        /// The matches param must be sorted such that longer strings come before shorter ones.
        /// </para>
        /// </remarks>
        void ParseInternal(string pattern, string[] matches)
        {
            var offset = 0;
            while (offset < pattern.Length)
            {
                var i = pattern.IndexOf('%', offset);
                if (i < 0 || i == pattern.Length - 1)
                {
                    ProcessLiteral(pattern.Substring(offset));
                    offset = pattern.Length;
                }
                else
                {
                    if (pattern[i + 1] == '%')
                    {
                        // Escaped
                        ProcessLiteral(pattern.Substring(offset, i - offset + 1));
                        offset = i + 2;
                    }
                    else
                    {
                        ProcessLiteral(pattern.Substring(offset, i - offset));
                        offset = i + 1;

                        var formattingInfo = new FormattingInfo();

                        // Process formatting options

                        // Look for the align flag
                        if (offset < pattern.Length)
                        {
                            if (pattern[offset] == '-')
                            {
                                // Seen align flag
                                formattingInfo.LeftAlign = true;
                                offset++;
                            }
                        }
                        // Look for the minimum length
                        while (offset < pattern.Length && char.IsDigit(pattern[offset]))
                        {
                            // Seen digit
                            if (formattingInfo.Min < 0)
                                formattingInfo.Min = 0;
                            formattingInfo.Min = (formattingInfo.Min * 10) +
                                                 int.Parse(pattern[offset].ToString(CultureInfo.InvariantCulture),
                                                     NumberFormatInfo.InvariantInfo);
                            offset++;
                        }
                        // Look for the separator between min and max
                        if (offset < pattern.Length)
                        {
                            if (pattern[offset] == '.')
                            {
                                // Seen separator
                                offset++;
                            }
                        }
                        // Look for the maximum length
                        while (offset < pattern.Length && char.IsDigit(pattern[offset]))
                        {
                            // Seen digit
                            if (formattingInfo.Max == int.MaxValue)
                                formattingInfo.Max = 0;
                            formattingInfo.Max = (formattingInfo.Max * 10) +
                                                 int.Parse(pattern[offset].ToString(CultureInfo.InvariantCulture),
                                                     NumberFormatInfo.InvariantInfo);
                            offset++;
                        }

                        var remainingStringLength = pattern.Length - offset;

                        // Look for pattern
                        for (var m = 0; m < matches.Length; m++)
                        {
                            if (matches[m].Length <= remainingStringLength)
                            {
                                if (
                                    String.Compare(pattern, offset, matches[m], 0, matches[m].Length, false,
                                        CultureInfo.InvariantCulture) == 0)
                                {
                                    // Found match
                                    offset = offset + matches[m].Length;

                                    string option = null;

                                    // Look for option
                                    if (offset < pattern.Length)
                                    {
                                        if (pattern[offset] == '{')
                                        {
                                            // Seen option start
                                            offset++;

                                            var optEnd = pattern.IndexOf('}', offset);
                                            if (optEnd < 0)
                                            {
                                                // error
                                            }
                                            else
                                            {
                                                option = pattern.Substring(offset, optEnd - offset);
                                                offset = optEnd + 1;
                                            }
                                        }
                                    }

                                    ProcessConverter(matches[m], option, formattingInfo);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Process a parsed converter pattern
        /// </summary>
        /// <param name="converterName">the name of the converter</param>
        /// <param name="option">the optional option for the converter</param>
        /// <param name="formattingInfo">the formatting info for the converter</param>
        void ProcessConverter(string converterName, string option, FormattingInfo formattingInfo)
        {
            LogLog.Debug("PatternParser: Converter [" + converterName + "] Option [" + option + "] Format [min=" +
                         formattingInfo.Min + ",max=" + formattingInfo.Max + ",leftAlign=" + formattingInfo.LeftAlign + "]");

            // Lookup the converter type
            var converterType = (Type)m_patternConverters[converterName];
            if (converterType == null)
                LogLog.Error("PatternParser: Unknown converter name [" + converterName + "] in conversion pattern.");
            else
            {
                // Create the pattern converter
                PatternConverter pc = null;
                try
                {
                    pc = (PatternConverter)Activator.CreateInstance(converterType);
                }
                catch (Exception createInstanceEx)
                {
                    LogLog.Error("PatternParser: Failed to create instance of Type [" + converterType.FullName +
                                 "] using default constructor. Exception: " + createInstanceEx);
                }

                // formattingInfo variable is an instance variable, occasionally reset 
                // and used over and over again
                pc.FormattingInfo = formattingInfo;
                pc.Option = option;

                var optionHandler = pc as IOptionHandler;
                if (optionHandler != null)
                    optionHandler.ActivateOptions();

                AddConverter(pc);
            }
        }

        /// <summary>
        /// Process a parsed literal
        /// </summary>
        /// <param name="text">the literal text</param>
        void ProcessLiteral(string text)
        {
            if (text.Length > 0)
            {
                // Convert into a pattern
                ProcessConverter("literal", text, new FormattingInfo());
            }
        }

        #region StringLengthComparer

        /// <summary>
        /// Sort strings by length
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="IComparer" /> that orders strings by string length.
        /// The longest strings are placed first
        /// </para>
        /// </remarks>
        sealed class StringLengthComparer : IComparer
        {
            public static readonly StringLengthComparer Instance = new StringLengthComparer();

            StringLengthComparer()
            {
            }

            #region Implementation of IComparer

            public int Compare(object x, object y)
            {
                var s1 = x as string;
                var s2 = y as string;

                if (s1 == null && s2 == null)
                    return 0;
                if (s1 == null)
                    return 1;
                if (s2 == null)
                    return -1;

                return s2.Length.CompareTo(s1.Length);
            }

            #endregion
        }

        #endregion // StringLengthComparer

        #endregion Protected Instance Methods

        #region Private Constants

        const char ESCAPE_CHAR = '%';

        #endregion Private Constants

        #region Private Instance Fields

        /// <summary>
        /// The pattern
        /// </summary>
        readonly string m_pattern;

        /// <summary>
        /// Internal map of converter identifiers to converter types
        /// </summary>
        /// <remarks>
        /// <para>
        /// This map overrides the static s_globalRulesRegistry map.
        /// </para>
        /// </remarks>
        readonly Hashtable m_patternConverters = new Hashtable();

        /// <summary>
        /// The first pattern converter in the chain
        /// </summary>
        PatternConverter m_head;

        /// <summary>
        ///  the last pattern converter in the chain
        /// </summary>
        PatternConverter m_tail;

        #endregion Private Instance Fields
    }
}