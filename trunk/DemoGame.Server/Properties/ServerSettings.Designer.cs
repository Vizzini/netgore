﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DemoGame.Server.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    public sealed partial class ServerSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static ServerSettings defaultInstance = ((ServerSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ServerSettings())));
        
        public static ServerSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Welcome to NetGore!\r\nUse the arrow keys to move, control to attack, alt to talk t" +
            "o NPCs and use world entities, and space to pick up items.")]
        public string MOTD {
            get {
                return ((string)(this["MOTD"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("180000")]
        public uint DefaultMapItemLife {
            get {
                return ((uint)(this["DefaultMapItemLife"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30000")]
        public uint MapItemExpirationUpdateRate {
            get {
                return ((uint)(this["MapItemExpirationUpdateRate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public ushort MaxConnections {
            get {
                return ((ushort)(this["MaxConnections"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AccountDropExistingConnectionWhenInUse {
            get {
                return ((bool)(this["AccountDropExistingConnectionWhenInUse"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("6")]
        public byte MaxConnectionsPerIP {
            get {
                return ((byte)(this["MaxConnectionsPerIP"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public uint MaxGroupShareDistance {
            get {
                return ((uint)(this["MaxGroupShareDistance"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4")]
        public byte MaxRecentlyCreatedAccounts {
            get {
                return ((byte)(this["MaxRecentlyCreatedAccounts"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("800")]
        public uint RespawnablesUpdateRate {
            get {
                return ((uint)(this["RespawnablesUpdateRate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("36000000")]
        public uint RoutineServerSaveRate {
            get {
                return ((uint)(this["RoutineServerSaveRate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public uint ServerUpdateRate {
            get {
                return ((uint)(this["ServerUpdateRate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("150")]
        public uint SyncExtraUserInformationRate {
            get {
                return ((uint)(this["SyncExtraUserInformationRate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public global::NetGore.World.MapID InvalidPersistentNPCLoadMap {
            get {
                return ((global::NetGore.World.MapID)(this["InvalidPersistentNPCLoadMap"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10, 10")]
        public global::SFML.Graphics.Vector2 InvalidPersistentNPCLoadPosition {
            get {
                return ((global::SFML.Graphics.Vector2)(this["InvalidPersistentNPCLoadPosition"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public global::NetGore.World.MapID InvalidUserLoadMap {
            get {
                return ((global::NetGore.World.MapID)(this["InvalidUserLoadMap"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1024, 600")]
        public global::SFML.Graphics.Vector2 InvalidUserLoadPosition {
            get {
                return ((global::SFML.Graphics.Vector2)(this["InvalidUserLoadPosition"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public global::DemoGame.ItemTemplateID UnarmedItemTemplateID {
            get {
                return ((global::DemoGame.ItemTemplateID)(this["UnarmedItemTemplateID"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("AboveNormal")]
        public global::System.Threading.ThreadPriority ThreadPriority {
            get {
                return ((global::System.Threading.ThreadPriority)(this["ThreadPriority"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string PasswordSalt {
            get {
                return ((string)(this["PasswordSalt"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-0.48")]
        public float CharacterJumpVelocity {
            get {
                return ((float)(this["CharacterJumpVelocity"]));
            }
        }
    }
}
