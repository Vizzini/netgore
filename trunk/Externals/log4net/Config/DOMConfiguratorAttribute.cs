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

namespace log4net.Config
{
    /// <summary>
    /// Assembly level attribute to configure the <see cref="XmlConfigurator"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>AliasDomainAttribute is obsolete. Use AliasRepositoryAttribute instead of AliasDomainAttribute.</b>
    /// </para>
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
    /// <author>Gert Driesen</author>
    [AttributeUsage(AttributeTargets.Assembly)]
    [Serializable]
    [Obsolete("Use XmlConfiguratorAttribute instead of DOMConfiguratorAttribute")]
    public sealed class DOMConfiguratorAttribute : XmlConfiguratorAttribute
    {
    }
}