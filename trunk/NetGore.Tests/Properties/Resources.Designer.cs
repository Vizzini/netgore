﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NetGore.Tests.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NetGore.Tests.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;Bodies&gt;
        ///	&lt;Body Index=&quot;1&quot;&gt;
        ///		&lt;Size Width=&quot;30&quot; Height=&quot;78&quot; /&gt;
        ///		&lt;Body SkelBody=&quot;basic&quot; /&gt;
        ///		&lt;Stand SkelSet=&quot;stand&quot; /&gt;
        ///		&lt;Walk SkelSet=&quot;walk&quot; /&gt;
        ///		&lt;Jump SkelSet=&quot;jump&quot; /&gt;
        ///		&lt;Fall SkelSet=&quot;fall&quot; /&gt;
        ///		
        ///		&lt;Punch SkelSet=&quot;punch&quot; X=&quot;$width/2&quot; Y=&quot;0&quot; Width=&quot;$width&quot; Height=&quot;$height/2&quot; /&gt;
        ///	&lt;/Body&gt;
        ///	&lt;Body Index=&quot;2&quot;&gt;
        ///		&lt;Size Width=&quot;300&quot; Height=&quot;78&quot; /&gt;
        ///		&lt;Body SkelBody=&quot;aa&quot; /&gt;
        ///		&lt;Stand SkelSet=&quot;b&quot; /&gt;
        ///		&lt;Walk SkelSet=&quot;c&quot; /&gt;
        ///		&lt;Jump SkelSet=&quot;dd&quot; /&gt;
        ///		&lt;Fall SkelSet=&quot;e [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string BasicXmlFile {
            get {
                return ResourceManager.GetString("BasicXmlFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP TABLE IF EXISTS `testdb_a`;
        ///CREATE TABLE IF NOT EXISTS `testdb_a` (
        ///  `boo` tinyint(1) NOT NULL,
        ///  `boou` tinyint(1) unsigned NOT NULL,
        ///  `b` tinyint(4) NOT NULL,
        ///  `bn` tinyint(4) DEFAULT NULL,
        ///  `bu` tinyint(3) unsigned NOT NULL,
        ///  `bun` tinyint(3) unsigned DEFAULT NULL,
        ///  `s` smallint(6) NOT NULL,
        ///  `sn` smallint(6) DEFAULT NULL,
        ///  `su` smallint(5) unsigned NOT NULL,
        ///  `sun` smallint(5) unsigned DEFAULT NULL,
        ///  `i` int(11) NOT NULL,
        ///  `in` int(11) DEFAULT NULL,
        ///  `iu` int(10) unsigned  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string testdb_a {
            get {
                return ResourceManager.GetString("testdb_a", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to testdb_a.
        /// </summary>
        internal static string testdb_a_name {
            get {
                return ResourceManager.GetString("testdb_a_name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP VIEW IF EXISTS testdb_a_view;
        ///CREATE VIEW testdb_a_view AS SELECT * FROM testdb_a;.
        /// </summary>
        internal static string testdb_a_view {
            get {
                return ResourceManager.GetString("testdb_a_view", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to testdb_a_view.
        /// </summary>
        internal static string testdb_a_view_name {
            get {
                return ResourceManager.GetString("testdb_a_view_name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP TABLE IF EXISTS `testdb_b`;
        ///CREATE TABLE `testdb_b` (
        ///  `a` int(11) NOT NULL,
        ///  `b` int(11) NOT NULL,
        ///  `abcdEFGhij` int(11) NOT NULL,
        ///  `asdfA` int(11) NOT NULL,
        ///  `asdfB` int(11) NOT NULL,
        ///  `asdfC` int(11) NOT NULL,
        ///  `bbbbA` int(11) NOT NULL,
        ///  `bbbbB` int(11) NOT NULL,
        ///  `bbbbC` int(11) NOT NULL
        ///) ENGINE=InnoDB DEFAULT CHARSET=latin1.
        /// </summary>
        internal static string testdb_b {
            get {
                return ResourceManager.GetString("testdb_b", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to testdb_b.
        /// </summary>
        internal static string testdb_b_name {
            get {
                return ResourceManager.GetString("testdb_b_name", resourceCulture);
            }
        }
    }
}
