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
using System.Reflection;
using System.Security;
using System.Xml;
using log4net.Appender;
using log4net.Core;
using log4net.ObjectRenderer;
using log4net.Util;

namespace log4net.Repository.Hierarchy
{
    /// <summary>
    /// Initializes the log4net environment using an XML DOM.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Configures a <see cref="Hierarchy"/> using an XML DOM.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    public class XmlHierarchyConfigurator
    {
        enum ConfigUpdateMode
        {
            Merge,
            Overwrite
        }

        #region Public Instance Constructors

        /// <summary>
        /// Construct the configurator for a hierarchy
        /// </summary>
        /// <param name="hierarchy">The hierarchy to build.</param>
        /// <remarks>
        /// <para>
        /// Initializes a new instance of the <see cref="XmlHierarchyConfigurator" /> class
        /// with the specified <see cref="Hierarchy" />.
        /// </para>
        /// </remarks>
        public XmlHierarchyConfigurator(Hierarchy hierarchy)
        {
            m_hierarchy = hierarchy;
            m_appenderBag = new Hashtable();
        }

        #endregion Public Instance Constructors

        #region Public Instance Methods

        /// <summary>
        /// Configure the hierarchy by parsing a DOM tree of XML elements.
        /// </summary>
        /// <param name="element">The root element to parse.</param>
        /// <remarks>
        /// <para>
        /// Configure the hierarchy by parsing a DOM tree of XML elements.
        /// </para>
        /// </remarks>
        public void Configure(XmlElement element)
        {
            if (element == null || m_hierarchy == null)
                return;

            var rootElementName = element.LocalName;

            if (rootElementName != CONFIGURATION_TAG)
            {
                LogLog.Error("XmlHierarchyConfigurator: Xml element is - not a <" + CONFIGURATION_TAG + "> element.");
                return;
            }

            if (!LogLog.InternalDebugging)
            {
                // Look for a debug attribute to enable internal debug
                var debugAttribute = element.GetAttribute(INTERNAL_DEBUG_ATTR);
                LogLog.Debug("XmlHierarchyConfigurator: " + INTERNAL_DEBUG_ATTR + " attribute [" + debugAttribute + "].");

                if (debugAttribute.Length > 0 && debugAttribute != "null")
                    LogLog.InternalDebugging = OptionConverter.ToBoolean(debugAttribute, true);
                else
                    LogLog.Debug("XmlHierarchyConfigurator: Ignoring " + INTERNAL_DEBUG_ATTR + " attribute.");

                var confDebug = element.GetAttribute(CONFIG_DEBUG_ATTR);
                if (confDebug.Length > 0 && confDebug != "null")
                {
                    LogLog.Warn("XmlHierarchyConfigurator: The \"" + CONFIG_DEBUG_ATTR + "\" attribute is deprecated.");
                    LogLog.Warn("XmlHierarchyConfigurator: Use the \"" + INTERNAL_DEBUG_ATTR + "\" attribute instead.");
                    LogLog.InternalDebugging = OptionConverter.ToBoolean(confDebug, true);
                }
            }

            // Default mode is merge
            var configUpdateMode = ConfigUpdateMode.Merge;

            // Look for the config update attribute
            var configUpdateModeAttribute = element.GetAttribute(CONFIG_UPDATE_MODE_ATTR);
            if (configUpdateModeAttribute != null && configUpdateModeAttribute.Length > 0)
            {
                // Parse the attribute
                try
                {
                    configUpdateMode =
                        (ConfigUpdateMode)OptionConverter.ConvertStringTo(typeof(ConfigUpdateMode), configUpdateModeAttribute);
                }
                catch
                {
                    LogLog.Error("XmlHierarchyConfigurator: Invalid " + CONFIG_UPDATE_MODE_ATTR + " attribute value [" +
                                 configUpdateModeAttribute + "]");
                }
            }

            // IMPL: The IFormatProvider argument to Enum.ToString() is deprecated in .NET 2.0
            LogLog.Debug("XmlHierarchyConfigurator: Configuration update mode [" + configUpdateMode + "].");

            // Only reset configuration if overwrite flag specified
            if (configUpdateMode == ConfigUpdateMode.Overwrite)
            {
                // Reset to original unset configuration
                m_hierarchy.ResetConfiguration();
                LogLog.Debug("XmlHierarchyConfigurator: Configuration reset before reading config.");
            }

            /* Building Appender objects, placing them in a local namespace
			   for future reference */

            /* Process all the top level elements */

            foreach (XmlNode currentNode in element.ChildNodes)
            {
                if (currentNode.NodeType == XmlNodeType.Element)
                {
                    var currentElement = (XmlElement)currentNode;

                    if (currentElement.LocalName == LOGGER_TAG)
                        ParseLogger(currentElement);
                    else if (currentElement.LocalName == CATEGORY_TAG)
                    {
                        // TODO: deprecated use of category
                        ParseLogger(currentElement);
                    }
                    else if (currentElement.LocalName == ROOT_TAG)
                        ParseRoot(currentElement);
                    else if (currentElement.LocalName == RENDERER_TAG)
                        ParseRenderer(currentElement);
                    else if (currentElement.LocalName == APPENDER_TAG)
                    {
                        // We ignore appenders in this pass. They will
                        // be found and loaded if they are referenced.
                    }
                    else
                    {
                        // Read the param tags and set properties on the hierarchy
                        SetParameter(currentElement, m_hierarchy);
                    }
                }
            }

            // Lastly set the hierarchy threshold
            var thresholdStr = element.GetAttribute(THRESHOLD_ATTR);
            LogLog.Debug("XmlHierarchyConfigurator: Hierarchy Threshold [" + thresholdStr + "]");
            if (thresholdStr.Length > 0 && thresholdStr != "null")
            {
                var thresholdLevel = (Level)ConvertStringTo(typeof(Level), thresholdStr);
                if (thresholdLevel != null)
                    m_hierarchy.Threshold = thresholdLevel;
                else
                {
                    LogLog.Warn("XmlHierarchyConfigurator: Unable to set hierarchy threshold using value [" + thresholdStr +
                                "] (with acceptable conversion types)");
                }
            }

            // Done reading config
        }

        #endregion Public Instance Methods

        #region Protected Instance Methods

        /// <summary>
        /// Converts a string value to a target type.
        /// </summary>
        /// <param name="type">The type of object to convert the string to.</param>
        /// <param name="value">The string value to use as the value of the object.</param>
        /// <returns>
        /// <para>
        /// An object of type <paramref name="type"/> with value <paramref name="value"/> or 
        /// <c>null</c> when the conversion could not be performed.
        /// </para>
        /// </returns>
        protected object ConvertStringTo(Type type, string value)
        {
            // Hack to allow use of Level in property
            if (typeof(Level) == type)
            {
                // Property wants a level
                var levelValue = m_hierarchy.LevelMap[value];

                if (levelValue == null)
                    LogLog.Error("XmlHierarchyConfigurator: Unknown Level Specified [" + value + "]");

                return levelValue;
            }
            return OptionConverter.ConvertStringTo(type, value);
        }

        /// <summary>
        /// Creates an object as specified in XML.
        /// </summary>
        /// <param name="element">The XML element that contains the definition of the object.</param>
        /// <param name="defaultTargetType">The object type to use if not explicitly specified.</param>
        /// <param name="typeConstraint">The type that the returned object must be or must inherit from.</param>
        /// <returns>The object or <c>null</c></returns>
        /// <remarks>
        /// <para>
        /// Parse an XML element and create an object instance based on the configuration
        /// data.
        /// </para>
        /// <para>
        /// The type of the instance may be specified in the XML. If not
        /// specified then the <paramref name="defaultTargetType"/> is used
        /// as the type. However the type is specified it must support the
        /// <paramref name="typeConstraint"/> type.
        /// </para>
        /// </remarks>
        protected object CreateObjectFromXml(XmlElement element, Type defaultTargetType, Type typeConstraint)
        {
            Type objectType = null;

            // Get the object type
            var objectTypeString = element.GetAttribute(TYPE_ATTR);
            if (objectTypeString == null || objectTypeString.Length == 0)
            {
                if (defaultTargetType == null)
                {
                    LogLog.Error("XmlHierarchyConfigurator: Object type not specified. Cannot create object of type [" +
                                 typeConstraint.FullName + "]. Missing Value or Type.");
                    return null;
                }
                else
                {
                    // Use the default object type
                    objectType = defaultTargetType;
                }
            }
            else
            {
                // Read the explicit object type
                try
                {
                    objectType = SystemInfo.GetTypeFromString(objectTypeString, true, true);
                }
                catch (Exception ex)
                {
                    LogLog.Error("XmlHierarchyConfigurator: Failed to find type [" + objectTypeString + "]", ex);
                    return null;
                }
            }

            var requiresConversion = false;

            // Got the object type. Check that it meets the typeConstraint
            if (typeConstraint != null)
            {
                if (!typeConstraint.IsAssignableFrom(objectType))
                {
                    // Check if there is an appropriate type converter
                    if (OptionConverter.CanConvertTypeTo(objectType, typeConstraint))
                        requiresConversion = true;
                    else
                    {
                        LogLog.Error("XmlHierarchyConfigurator: Object type [" + objectType.FullName +
                                     "] is not assignable to type [" + typeConstraint.FullName +
                                     "]. There are no acceptable type conversions.");
                        return null;
                    }
                }
            }

            // Create using the default constructor
            object createdObject = null;
            try
            {
                createdObject = Activator.CreateInstance(objectType);
            }
            catch (Exception createInstanceEx)
            {
                LogLog.Error("XmlHierarchyConfigurator: Failed to construct object of type [" + objectType.FullName +
                             "] Exception: " + createInstanceEx);
            }

            // Set any params on object
            foreach (XmlNode currentNode in element.ChildNodes)
            {
                if (currentNode.NodeType == XmlNodeType.Element)
                    SetParameter((XmlElement)currentNode, createdObject);
            }

            // Check if we need to call ActivateOptions
            var optionHandler = createdObject as IOptionHandler;
            if (optionHandler != null)
                optionHandler.ActivateOptions();

            // Ok object should be initialized

            if (requiresConversion)
            {
                // Convert the object type
                return OptionConverter.ConvertTypeTo(createdObject, typeConstraint);
            }
            else
            {
                // The object is of the correct type
                return createdObject;
            }
        }

        /// <summary>
        /// Parse appenders by IDREF.
        /// </summary>
        /// <param name="appenderRef">The appender ref element.</param>
        /// <returns>The instance of the appender that the ref refers to.</returns>
        /// <remarks>
        /// <para>
        /// Parse an XML element that represents an appender and return 
        /// the appender.
        /// </para>
        /// </remarks>
        protected IAppender FindAppenderByReference(XmlElement appenderRef)
        {
            var appenderName = appenderRef.GetAttribute(REF_ATTR);

            var appender = (IAppender)m_appenderBag[appenderName];
            if (appender != null)
                return appender;
            else
            {
                // Find the element with that id
                XmlElement element = null;

                if (appenderName != null && appenderName.Length > 0)
                {
                    foreach (XmlElement curAppenderElement in appenderRef.OwnerDocument.GetElementsByTagName(APPENDER_TAG))
                    {
                        if (curAppenderElement.GetAttribute("name") == appenderName)
                        {
                            element = curAppenderElement;
                            break;
                        }
                    }
                }

                if (element == null)
                {
                    LogLog.Error("XmlHierarchyConfigurator: No appender named [" + appenderName + "] could be found.");
                    return null;
                }
                else
                {
                    appender = ParseAppender(element);
                    if (appender != null)
                        m_appenderBag[appenderName] = appender;
                    return appender;
                }
            }
        }

        /// <summary>
        /// Look for a method on the <paramref name="targetType"/> that matches the <paramref name="name"/> supplied
        /// </summary>
        /// <param name="targetType">the type that has the method</param>
        /// <param name="name">the name of the method</param>
        /// <returns>the method info found</returns>
        /// <remarks>
        /// <para>
        /// The method must be a public instance method on the <paramref name="targetType"/>.
        /// The method must be named <paramref name="name"/> or "Add" followed by <paramref name="name"/>.
        /// The method must take a single parameter.
        /// </para>
        /// </remarks>
        MethodInfo FindMethodInfo(Type targetType, string name)
        {
            var requiredMethodNameA = name;
            var requiredMethodNameB = "Add" + name;

            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var methInfo in methods)
            {
                if (!methInfo.IsStatic)
                {
                    if (string.Compare(methInfo.Name, requiredMethodNameA, true, CultureInfo.InvariantCulture) == 0 ||
                        string.Compare(methInfo.Name, requiredMethodNameB, true, CultureInfo.InvariantCulture) == 0)
                    {
                        // Found matching method name

                        // Look for version with one arg only
                        var methParams = methInfo.GetParameters();
                        if (methParams.Length == 1)
                            return methInfo;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Test if an element has no attributes or child elements
        /// </summary>
        /// <param name="element">the element to inspect</param>
        /// <returns><c>true</c> if the element has any attributes or child elements, <c>false</c> otherwise</returns>
        bool HasAttributesOrElements(XmlElement element)
        {
            foreach (XmlNode node in element.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Attribute || node.NodeType == XmlNodeType.Element)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Test if a <see cref="Type"/> is constructible with <c>Activator.CreateInstance</c>.
        /// </summary>
        /// <param name="type">the type to inspect</param>
        /// <returns><c>true</c> if the type is creatable using a default constructor, <c>false</c> otherwise</returns>
        static bool IsTypeConstructible(Type type)
        {
            if (type.IsClass && !type.IsAbstract)
            {
                var defaultConstructor = type.GetConstructor(new Type[0]);
                if (defaultConstructor != null && !defaultConstructor.IsAbstract && !defaultConstructor.IsPrivate)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Parses an appender element.
        /// </summary>
        /// <param name="appenderElement">The appender element.</param>
        /// <returns>The appender instance or <c>null</c> when parsing failed.</returns>
        /// <remarks>
        /// <para>
        /// Parse an XML element that represents an appender and return
        /// the appender instance.
        /// </para>
        /// </remarks>
        protected IAppender ParseAppender(XmlElement appenderElement)
        {
            var appenderName = appenderElement.GetAttribute(NAME_ATTR);
            var typeName = appenderElement.GetAttribute(TYPE_ATTR);

            LogLog.Debug("XmlHierarchyConfigurator: Loading Appender [" + appenderName + "] type: [" + typeName + "]");
            try
            {
                var appender = (IAppender)Activator.CreateInstance(SystemInfo.GetTypeFromString(typeName, true, true));
                appender.Name = appenderName;

                foreach (XmlNode currentNode in appenderElement.ChildNodes)
                {
                    /* We're only interested in Elements */
                    if (currentNode.NodeType == XmlNodeType.Element)
                    {
                        var currentElement = (XmlElement)currentNode;

                        // Look for the appender ref tag
                        if (currentElement.LocalName == APPENDER_REF_TAG)
                        {
                            var refName = currentElement.GetAttribute(REF_ATTR);

                            var appenderContainer = appender as IAppenderAttachable;
                            if (appenderContainer != null)
                            {
                                LogLog.Debug("XmlHierarchyConfigurator: Attaching appender named [" + refName +
                                             "] to appender named [" + appender.Name + "].");

                                var referencedAppender = FindAppenderByReference(currentElement);
                                if (referencedAppender != null)
                                    appenderContainer.AddAppender(referencedAppender);
                            }
                            else
                            {
                                LogLog.Error("XmlHierarchyConfigurator: Requesting attachment of appender named [" + refName +
                                             "] to appender named [" + appender.Name +
                                             "] which does not implement log4net.Core.IAppenderAttachable.");
                            }
                        }
                        else
                        {
                            // For all other tags we use standard set param method
                            SetParameter(currentElement, appender);
                        }
                    }
                }

                var optionHandler = appender as IOptionHandler;
                if (optionHandler != null)
                    optionHandler.ActivateOptions();

                LogLog.Debug("XmlHierarchyConfigurator: Created Appender [" + appenderName + "]");
                return appender;
            }
            catch (Exception ex)
            {
                // Yes, it's ugly.  But all exceptions point to the same problem: we can't create an Appender

                LogLog.Error(
                    "XmlHierarchyConfigurator: Could not create Appender [" + appenderName + "] of type [" + typeName +
                    "]. Reported error follows.", ex);
                return null;
            }
        }

        /// <summary>
        /// Parses the children of a logger element.
        /// </summary>
        /// <param name="catElement">The category element.</param>
        /// <param name="log">The logger instance.</param>
        /// <param name="isRoot">Flag to indicate if the logger is the root logger.</param>
        /// <remarks>
        /// <para>
        /// Parse the child elements of a &lt;logger&gt; element.
        /// </para>
        /// </remarks>
        protected void ParseChildrenOfLoggerElement(XmlElement catElement, Logger log, bool isRoot)
        {
            // Remove all existing appenders from log. They will be
            // reconstructed if need be.
            log.RemoveAllAppenders();

            foreach (XmlNode currentNode in catElement.ChildNodes)
            {
                if (currentNode.NodeType == XmlNodeType.Element)
                {
                    var currentElement = (XmlElement)currentNode;

                    if (currentElement.LocalName == APPENDER_REF_TAG)
                    {
                        var appender = FindAppenderByReference(currentElement);
                        var refName = currentElement.GetAttribute(REF_ATTR);
                        if (appender != null)
                        {
                            LogLog.Debug("XmlHierarchyConfigurator: Adding appender named [" + refName + "] to logger [" +
                                         log.Name + "].");
                            log.AddAppender(appender);
                        }
                        else
                            LogLog.Error("XmlHierarchyConfigurator: Appender named [" + refName + "] not found.");
                    }
                    else if (currentElement.LocalName == LEVEL_TAG || currentElement.LocalName == PRIORITY_TAG)
                        ParseLevel(currentElement, log, isRoot);
                    else
                        SetParameter(currentElement, log);
                }
            }

            var optionHandler = log as IOptionHandler;
            if (optionHandler != null)
                optionHandler.ActivateOptions();
        }

        /// <summary>
        /// Parses a level element.
        /// </summary>
        /// <param name="element">The level element.</param>
        /// <param name="log">The logger object to set the level on.</param>
        /// <param name="isRoot">Flag to indicate if the logger is the root logger.</param>
        /// <remarks>
        /// <para>
        /// Parse an XML element that represents a level.
        /// </para>
        /// </remarks>
        protected void ParseLevel(XmlElement element, Logger log, bool isRoot)
        {
            var loggerName = log.Name;
            if (isRoot)
                loggerName = "root";

            var levelStr = element.GetAttribute(VALUE_ATTR);
            LogLog.Debug("XmlHierarchyConfigurator: Logger [" + loggerName + "] Level string is [" + levelStr + "].");

            if (INHERITED == levelStr)
            {
                if (isRoot)
                    LogLog.Error("XmlHierarchyConfigurator: Root level cannot be inherited. Ignoring directive.");
                else
                {
                    LogLog.Debug("XmlHierarchyConfigurator: Logger [" + loggerName + "] level set to inherit from parent.");
                    log.Level = null;
                }
            }
            else
            {
                log.Level = log.Hierarchy.LevelMap[levelStr];
                if (log.Level == null)
                    LogLog.Error("XmlHierarchyConfigurator: Undefined level [" + levelStr + "] on Logger [" + loggerName + "].");
                else
                {
                    LogLog.Debug("XmlHierarchyConfigurator: Logger [" + loggerName + "] level set to [name=\"" + log.Level.Name +
                                 "\",value=" + log.Level.Value + "].");
                }
            }
        }

        /// <summary>
        /// Parses a logger element.
        /// </summary>
        /// <param name="loggerElement">The logger element.</param>
        /// <remarks>
        /// <para>
        /// Parse an XML element that represents a logger.
        /// </para>
        /// </remarks>
        protected void ParseLogger(XmlElement loggerElement)
        {
            // Create a new log4net.Logger object from the <logger> element.
            var loggerName = loggerElement.GetAttribute(NAME_ATTR);

            LogLog.Debug("XmlHierarchyConfigurator: Retrieving an instance of log4net.Repository.Logger for logger [" + loggerName +
                         "].");
            var log = m_hierarchy.GetLogger(loggerName) as Logger;

            // Setting up a logger needs to be an atomic operation, in order
            // to protect potential log operations while logger
            // configuration is in progress.
            lock (log)
            {
                var additivity = OptionConverter.ToBoolean(loggerElement.GetAttribute(ADDITIVITY_ATTR), true);

                LogLog.Debug("XmlHierarchyConfigurator: Setting [" + log.Name + "] additivity to [" + additivity + "].");
                log.Additivity = additivity;
                ParseChildrenOfLoggerElement(loggerElement, log, false);
            }
        }

        /// <summary>
        /// Parses an object renderer.
        /// </summary>
        /// <param name="element">The renderer element.</param>
        /// <remarks>
        /// <para>
        /// Parse an XML element that represents a renderer.
        /// </para>
        /// </remarks>
        protected void ParseRenderer(XmlElement element)
        {
            var renderingClassName = element.GetAttribute(RENDERING_TYPE_ATTR);
            var renderedClassName = element.GetAttribute(RENDERED_TYPE_ATTR);

            LogLog.Debug("XmlHierarchyConfigurator: Rendering class [" + renderingClassName + "], Rendered class [" +
                         renderedClassName + "].");
            var renderer =
                (IObjectRenderer)OptionConverter.InstantiateByClassName(renderingClassName, typeof(IObjectRenderer), null);
            if (renderer == null)
            {
                LogLog.Error("XmlHierarchyConfigurator: Could not instantiate renderer [" + renderingClassName + "].");
                return;
            }
            else
            {
                try
                {
                    m_hierarchy.RendererMap.Put(SystemInfo.GetTypeFromString(renderedClassName, true, true), renderer);
                }
                catch (Exception e)
                {
                    LogLog.Error("XmlHierarchyConfigurator: Could not find class [" + renderedClassName + "].", e);
                }
            }
        }

        /// <summary>
        /// Parses the root logger element.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <remarks>
        /// <para>
        /// Parse an XML element that represents the root logger.
        /// </para>
        /// </remarks>
        protected void ParseRoot(XmlElement rootElement)
        {
            var root = m_hierarchy.Root;
            // logger configuration needs to be atomic
            lock (root)
            {
                ParseChildrenOfLoggerElement(rootElement, root, true);
            }
        }

        /// <summary>
        /// Sets a parameter on an object.
        /// </summary>
        /// <param name="element">The parameter element.</param>
        /// <param name="target">The object to set the parameter on.</param>
        /// <remarks>
        /// The parameter name must correspond to a writable property
        /// on the object. The value of the parameter is a string,
        /// therefore this function will attempt to set a string
        /// property first. If unable to set a string property it
        /// will inspect the property and its argument type. It will
        /// attempt to call a static method called <c>Parse</c> on the
        /// type of the property. This method will take a single
        /// string argument and return a value that can be used to
        /// set the property.
        /// </remarks>
        protected void SetParameter(XmlElement element, object target)
        {
            // Get the property name
            var name = element.GetAttribute(NAME_ATTR);

            // If the name attribute does not exist then use the name of the element
            if (element.LocalName != PARAM_TAG || name == null || name.Length == 0)
                name = element.LocalName;

            // Look for the property on the target object
            var targetType = target.GetType();
            Type propertyType = null;

            PropertyInfo propInfo = null;
            MethodInfo methInfo = null;

            // Try to find a writable property
            propInfo = targetType.GetProperty(name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            if (propInfo != null && propInfo.CanWrite)
            {
                // found a property
                propertyType = propInfo.PropertyType;
            }
            else
            {
                propInfo = null;

                // look for a method with the signature Add<property>(type)
                methInfo = FindMethodInfo(targetType, name);

                if (methInfo != null)
                    propertyType = methInfo.GetParameters()[0].ParameterType;
            }

            if (propertyType == null)
                LogLog.Error("XmlHierarchyConfigurator: Cannot find Property [" + name + "] to set object on [" + target + "]");
            else
            {
                string propertyValue = null;

                if (element.GetAttributeNode(VALUE_ATTR) != null)
                    propertyValue = element.GetAttribute(VALUE_ATTR);
                else if (element.HasChildNodes)
                {
                    // Concatenate the CDATA and Text nodes together
                    foreach (XmlNode childNode in element.ChildNodes)
                    {
                        if (childNode.NodeType == XmlNodeType.CDATA || childNode.NodeType == XmlNodeType.Text)
                        {
                            if (propertyValue == null)
                                propertyValue = childNode.InnerText;
                            else
                                propertyValue += childNode.InnerText;
                        }
                    }
                }

                if (propertyValue != null)
                {
                    try
                    {
                        // Expand environment variables in the string.
                        propertyValue = OptionConverter.SubstituteVariables(propertyValue, Environment.GetEnvironmentVariables());
                    }
                    catch (SecurityException)
                    {
                        // This security exception will occur if the caller does not have 
                        // unrestricted environment permission. If this occurs the expansion 
                        // will be skipped with the following warning message.
                        LogLog.Debug(
                            "XmlHierarchyConfigurator: Security exception while trying to expand environment variables. Error Ignored. No Expansion.");
                    }

                    Type parsedObjectConversionTargetType = null;

                    // Check if a specific subtype is specified on the element using the 'type' attribute
                    var subTypeString = element.GetAttribute(TYPE_ATTR);
                    if (subTypeString != null && subTypeString.Length > 0)
                    {
                        // Read the explicit subtype
                        try
                        {
                            var subType = SystemInfo.GetTypeFromString(subTypeString, true, true);

                            LogLog.Debug("XmlHierarchyConfigurator: Parameter [" + name + "] specified subtype [" +
                                         subType.FullName + "]");

                            if (!propertyType.IsAssignableFrom(subType))
                            {
                                // Check if there is an appropriate type converter
                                if (OptionConverter.CanConvertTypeTo(subType, propertyType))
                                {
                                    // Must re-convert to the real property type
                                    parsedObjectConversionTargetType = propertyType;

                                    // Use sub type as intermediary type
                                    propertyType = subType;
                                }
                                else
                                {
                                    LogLog.Error("XmlHierarchyConfigurator: Subtype [" + subType.FullName + "] set on [" + name +
                                                 "] is not a subclass of property type [" + propertyType.FullName +
                                                 "] and there are no acceptable type conversions.");
                                }
                            }
                            else
                            {
                                // The subtype specified is found and is actually a subtype of the property
                                // type, therefore we can switch to using this type.
                                propertyType = subType;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogLog.Error(
                                "XmlHierarchyConfigurator: Failed to find type [" + subTypeString + "] set on [" + name + "]", ex);
                        }
                    }

                    // Now try to convert the string value to an acceptable type
                    // to pass to this property.

                    var convertedValue = ConvertStringTo(propertyType, propertyValue);

                    // Check if we need to do an additional conversion
                    if (convertedValue != null && parsedObjectConversionTargetType != null)
                    {
                        LogLog.Debug("XmlHierarchyConfigurator: Performing additional conversion of value from [" +
                                     convertedValue.GetType().Name + "] to [" + parsedObjectConversionTargetType.Name + "]");
                        convertedValue = OptionConverter.ConvertTypeTo(convertedValue, parsedObjectConversionTargetType);
                    }

                    if (convertedValue != null)
                    {
                        if (propInfo != null)
                        {
                            // Got a converted result
                            LogLog.Debug("XmlHierarchyConfigurator: Setting Property [" + propInfo.Name + "] to " +
                                         convertedValue.GetType().Name + " value [" + convertedValue + "]");

                            try
                            {
                                // Pass to the property
                                propInfo.SetValue(target, convertedValue, BindingFlags.SetProperty, null, null,
                                    CultureInfo.InvariantCulture);
                            }
                            catch (TargetInvocationException targetInvocationEx)
                            {
                                LogLog.Error(
                                    "XmlHierarchyConfigurator: Failed to set parameter [" + propInfo.Name + "] on object [" +
                                    target + "] using value [" + convertedValue + "]", targetInvocationEx.InnerException);
                            }
                        }
                        else if (methInfo != null)
                        {
                            // Got a converted result
                            LogLog.Debug("XmlHierarchyConfigurator: Setting Collection Property [" + methInfo.Name + "] to " +
                                         convertedValue.GetType().Name + " value [" + convertedValue + "]");

                            try
                            {
                                // Pass to the property
                                methInfo.Invoke(target, BindingFlags.InvokeMethod, null, new object[] { convertedValue },
                                    CultureInfo.InvariantCulture);
                            }
                            catch (TargetInvocationException targetInvocationEx)
                            {
                                LogLog.Error(
                                    "XmlHierarchyConfigurator: Failed to set parameter [" + name + "] on object [" + target +
                                    "] using value [" + convertedValue + "]", targetInvocationEx.InnerException);
                            }
                        }
                    }
                    else
                    {
                        LogLog.Warn("XmlHierarchyConfigurator: Unable to set property [" + name + "] on object [" + target +
                                    "] using value [" + propertyValue + "] (with acceptable conversion types)");
                    }
                }
                else
                {
                    object createdObject = null;

                    if (propertyType == typeof(string) && !HasAttributesOrElements(element))
                    {
                        // If the property is a string and the element is empty (no attributes
                        // or child elements) then we special case the object value to an empty string.
                        // This is necessary because while the String is a class it does not have
                        // a default constructor that creates an empty string, which is the behavior
                        // we are trying to simulate and would be expected from CreateObjectFromXml
                        createdObject = "";
                    }
                    else
                    {
                        // No value specified
                        Type defaultObjectType = null;
                        if (IsTypeConstructible(propertyType))
                            defaultObjectType = propertyType;

                        createdObject = CreateObjectFromXml(element, defaultObjectType, propertyType);
                    }

                    if (createdObject == null)
                        LogLog.Error("XmlHierarchyConfigurator: Failed to create object to set param: " + name);
                    else
                    {
                        if (propInfo != null)
                        {
                            // Got a converted result
                            LogLog.Debug("XmlHierarchyConfigurator: Setting Property [" + propInfo.Name + "] to object [" +
                                         createdObject + "]");

                            try
                            {
                                // Pass to the property
                                propInfo.SetValue(target, createdObject, BindingFlags.SetProperty, null, null,
                                    CultureInfo.InvariantCulture);
                            }
                            catch (TargetInvocationException targetInvocationEx)
                            {
                                LogLog.Error(
                                    "XmlHierarchyConfigurator: Failed to set parameter [" + propInfo.Name + "] on object [" +
                                    target + "] using value [" + createdObject + "]", targetInvocationEx.InnerException);
                            }
                        }
                        else if (methInfo != null)
                        {
                            // Got a converted result
                            LogLog.Debug("XmlHierarchyConfigurator: Setting Collection Property [" + methInfo.Name +
                                         "] to object [" + createdObject + "]");

                            try
                            {
                                // Pass to the property
                                methInfo.Invoke(target, BindingFlags.InvokeMethod, null, new object[] { createdObject },
                                    CultureInfo.InvariantCulture);
                            }
                            catch (TargetInvocationException targetInvocationEx)
                            {
                                LogLog.Error(
                                    "XmlHierarchyConfigurator: Failed to set parameter [" + methInfo.Name + "] on object [" +
                                    target + "] using value [" + createdObject + "]", targetInvocationEx.InnerException);
                            }
                        }
                    }
                }
            }
        }

        #endregion Protected Instance Methods

        #region Private Constants

        // String constants used while parsing the XML data
        const string ADDITIVITY_ATTR = "additivity";
        const string APPENDER_REF_TAG = "appender-ref";
        const string APPENDER_TAG = "appender";

        // TODO: Deprecate use of category tags
        const string CATEGORY_TAG = "category";
        const string CONFIGURATION_TAG = "log4net";
        const string CONFIG_DEBUG_ATTR = "configDebug";
        const string CONFIG_UPDATE_MODE_ATTR = "update";
        const string INHERITED = "inherited";
        const string INTERNAL_DEBUG_ATTR = "debug";
        const string LEVEL_TAG = "level";
        // TODO: Deprecate use of priority tag

        const string LOGGER_TAG = "logger";
        const string NAME_ATTR = "name";
        const string PARAM_TAG = "param";
        const string PRIORITY_TAG = "priority";
        const string REF_ATTR = "ref";
        const string RENDERED_TYPE_ATTR = "renderedClass";
        const string RENDERER_TAG = "renderer";
        const string RENDERING_TYPE_ATTR = "renderingClass";
        const string ROOT_TAG = "root";
        const string THRESHOLD_ATTR = "threshold";
        const string TYPE_ATTR = "type";
        const string VALUE_ATTR = "value";

        // flag used on the level element

        #endregion Private Constants

        #region Private Instance Fields

        /// <summary>
        /// key: appenderName, value: appender.
        /// </summary>
        readonly Hashtable m_appenderBag;

        /// <summary>
        /// The Hierarchy being configured.
        /// </summary>
        readonly Hierarchy m_hierarchy;

        #endregion Private Instance Fields
    }
}