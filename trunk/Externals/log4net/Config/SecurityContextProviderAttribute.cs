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
using System.Linq;
using System.Reflection;
using log4net.Core;
using log4net.Repository;
using log4net.Util;

namespace log4net.Config
{
    /// <summary>
    /// Assembly level attribute to configure the <see cref="SecurityContextProvider"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute may only be used at the assembly scope and can only
    /// be used once per assembly.
    /// </para>
    /// <para>
    /// Use this attribute to configure the <see cref="XmlConfigurator"/>
    /// without calling one of the <see cref="XmlConfigurator.Configure()"/>
    /// methods.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    [AttributeUsage(AttributeTargets.Assembly)]
    [Serializable]
    public sealed class SecurityContextProviderAttribute : ConfiguratorAttribute
    {
        #region Constructor

        /// <summary>
        /// Construct provider attribute with type specified
        /// </summary>
        /// <param name="providerType">the type of the provider to use</param>
        /// <remarks>
        /// <para>
        /// The provider specified must subclass the <see cref="SecurityContextProvider"/>
        /// class.
        /// </para>
        /// </remarks>
        public SecurityContextProviderAttribute(Type providerType) : base(100)
            /* configurator priority 100 to execute before the XmlConfigurator */
        {
            m_providerType = providerType;
        }

        #endregion

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the type of the provider to use.
        /// </summary>
        /// <value>
        /// the type of the provider to use.
        /// </value>
        /// <remarks>
        /// <para>
        /// The provider specified must subclass the <see cref="SecurityContextProvider"/>
        /// class.
        /// </para>
        /// </remarks>
        public Type ProviderType
        {
            get { return m_providerType; }
            set { m_providerType = value; }
        }

        #endregion Public Instance Properties

        #region Override ConfiguratorAttribute

        /// <summary>
        /// Configures the SecurityContextProvider
        /// </summary>
        /// <param name="sourceAssembly">The assembly that this attribute was defined on.</param>
        /// <param name="targetRepository">The repository to configure.</param>
        /// <remarks>
        /// <para>
        /// Creates a provider instance from the <see cref="ProviderType"/> specified.
        /// Sets this as the default security context provider <see cref="SecurityContextProvider.DefaultProvider"/>.
        /// </para>
        /// </remarks>
        public override void Configure(Assembly sourceAssembly, ILoggerRepository targetRepository)
        {
            if (m_providerType == null)
            {
                LogLog.Error("SecurityContextProviderAttribute: Attribute specified on assembly [" + sourceAssembly.FullName +
                             "] with null ProviderType.");
            }
            else
            {
                LogLog.Debug("SecurityContextProviderAttribute: Creating provider of type [" + m_providerType.FullName + "]");

                var provider = Activator.CreateInstance(m_providerType) as SecurityContextProvider;

                if (provider == null)
                {
                    LogLog.Error("SecurityContextProviderAttribute: Failed to create SecurityContextProvider instance of type [" +
                                 m_providerType.Name + "].");
                }
                else
                    SecurityContextProvider.DefaultProvider = provider;
            }
        }

        #endregion

        #region Private Instance Fields

        Type m_providerType = null;

        #endregion Private Instance Fields
    }
}