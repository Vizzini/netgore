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

using System.Linq;
using log4net.Core;

namespace log4net.Util
{
    /// <summary>
    /// An entry in the <see cref="LevelMapping"/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is an abstract base class for types that are stored in the
    /// <see cref="LevelMapping"/> object.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    public abstract class LevelMappingEntry : IOptionHandler
    {
        #region Public Instance Constructors

        #endregion // Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// The level that is the key for this mapping 
        /// </summary>
        /// <value>
        /// The <see cref="Level"/> that is the key for this mapping 
        /// </value>
        /// <remarks>
        /// <para>
        /// Get or set the <see cref="Level"/> that is the key for this
        /// mapping subclass.
        /// </para>
        /// </remarks>
        public Level Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        #endregion // Public Instance Properties

        #region IOptionHandler Members

        /// <summary>
        /// Initialize any options defined on this entry
        /// </summary>
        /// <remarks>
        /// <para>
        /// Should be overridden by any classes that need to initialise based on their options
        /// </para>
        /// </remarks>
        public virtual void ActivateOptions()
        {
            // default implementation is to do nothing
        }

        #endregion

        #region Private Instance Fields

        Level m_level;

        #endregion // Private Instance Fields
    }
}