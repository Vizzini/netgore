﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NetGore.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NetGore.Properties.Resources", typeof(Resources).Assembly);
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
        
        internal static System.Drawing.Bitmap Blank {
            get {
                object obj = ResourceManager.GetObject("Blank", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // The distance multiplier to apply to the values unpacked from channels to get the offset. This decreases our resolution,
        ///// giving us a choppier image, but increases our range. Lower values give higher resolution but require smaller distances.
        ///// This MUST be the same in all the refraction effects!
        ///const float DistanceMultiplier = 2.0;
        ///
        ///// The value of the reflection channels that will be used to not perform any reflection. Having this non-zero allows us to
        ///// reflect in both directions instead of j [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ExplosionRefractionEffectShader {
            get {
                return ResourceManager.GetString("ExplosionRefractionEffectShader", resourceCulture);
            }
        }
        
        internal static System.Drawing.Bitmap Joint {
            get {
                object obj = ResourceManager.GetObject("Joint", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap Lightbulb {
            get {
                object obj = ResourceManager.GetObject("Lightbulb", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap Move {
            get {
                object obj = ResourceManager.GetObject("Move", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap Resize {
            get {
                object obj = ResourceManager.GetObject("Resize", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap Triangle {
            get {
                object obj = ResourceManager.GetObject("Triangle", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // The distance multiplier to apply to the values unpacked from channels to get the offset. This decreases our resolution,
        ///// giving us a choppier image, but increases our range. Lower values give higher resolution but require smaller distances.
        ///// This MUST be the same in all the refraction effects!
        ///const float DistanceMultiplier = 2.0;
        ///
        ///// The value of the reflection channels that will be used to not perform any reflection. Having this non-zero allows us to
        ///// reflect in both directions instead of j [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string WaterRefractionEffectShader {
            get {
                return ResourceManager.GetString("WaterRefractionEffectShader", resourceCulture);
            }
        }
    }
}
