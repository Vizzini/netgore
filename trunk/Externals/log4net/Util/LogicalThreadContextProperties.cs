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
using System.Runtime.Remoting.Messaging;

namespace log4net.Util
{
    /// <summary>
    /// Implementation of Properties collection for the <see cref="log4net.LogicalThreadContext"/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Class implements a collection of properties that is specific to each thread.
    /// The class is not synchronized as each thread has its own <see cref="PropertiesDictionary"/>.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    public sealed class LogicalThreadContextProperties : ContextPropertiesBase
    {
        #region Public Instance Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// <para>
        /// Initializes a new instance of the <see cref="LogicalThreadContextProperties" /> class.
        /// </para>
        /// </remarks>
        internal LogicalThreadContextProperties()
        {
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the value of a property
        /// </summary>
        /// <value>
        /// The value for the property with the specified key
        /// </value>
        /// <remarks>
        /// <para>
        /// Get or set the property value for the <paramref name="key"/> specified.
        /// </para>
        /// </remarks>
        public override object this[string key]
        {
            get
            {
                // Don't create the dictionary if it does not already exist
                var dictionary = GetProperties(false);
                if (dictionary != null)
                    return dictionary[key];
                return null;
            }
            set
            {
                // Force the dictionary to be created
                GetProperties(true)[key] = value;
            }
        }

        #endregion Public Instance Properties

        #region Public Instance Methods

        /// <summary>
        /// Clear all the context properties
        /// </summary>
        /// <remarks>
        /// <para>
        /// Clear all the context properties
        /// </para>
        /// </remarks>
        public void Clear()
        {
            var dictionary = GetProperties(false);
            if (dictionary != null)
                dictionary.Clear();
        }

        /// <summary>
        /// Remove a property
        /// </summary>
        /// <param name="key">the key for the entry to remove</param>
        /// <remarks>
        /// <para>
        /// Remove the value for the specified <paramref name="key"/> from the context.
        /// </para>
        /// </remarks>
        public void Remove(string key)
        {
            var dictionary = GetProperties(false);
            if (dictionary != null)
                dictionary.Remove(key);
        }

        #endregion Public Instance Methods

        #region Internal Instance Methods

        /// <summary>
        /// Get the PropertiesDictionary stored in the LocalDataStoreSlot for this thread.
        /// </summary>
        /// <param name="create">create the dictionary if it does not exist, otherwise return null if is does not exist</param>
        /// <returns>the properties for this thread</returns>
        /// <remarks>
        /// <para>
        /// The collection returned is only to be used on the calling thread. If the
        /// caller needs to share the collection between different threads then the 
        /// caller must clone the collection before doings so.
        /// </para>
        /// </remarks>
        internal PropertiesDictionary GetProperties(bool create)
        {
            var properties = (PropertiesDictionary)CallContext.GetData("log4net.Util.LogicalThreadContextProperties");
            if (properties == null && create)
            {
                properties = new PropertiesDictionary();
                CallContext.SetData("log4net.Util.LogicalThreadContextProperties", properties);
            }
            return properties;
        }

        #endregion Internal Instance Methods
    }
}