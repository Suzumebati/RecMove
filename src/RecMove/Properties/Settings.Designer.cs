﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace RecMove.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.6.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("H:\\")]
        public string TextBox_SrcDir {
            get {
                return ((string)(this["TextBox_SrcDir"]));
            }
            set {
                this["TextBox_SrcDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("W:\\スプラ")]
        public string TextBox_DstDir {
            get {
                return ((string)(this["TextBox_DstDir"]));
            }
            set {
                this["TextBox_DstDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("*.mp4;*.jpg")]
        public string TextBox_FileExtention {
            get {
                return ((string)(this["TextBox_FileExtention"]));
            }
            set {
                this["TextBox_FileExtention"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Check_CreateYmdFolder {
            get {
                return ((bool)(this["Check_CreateYmdFolder"]));
            }
            set {
                this["Check_CreateYmdFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Check_SaveSubFolder {
            get {
                return ((bool)(this["Check_SaveSubFolder"]));
            }
            set {
                this["Check_SaveSubFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Check_CopyMode {
            get {
                return ((bool)(this["Check_CopyMode"]));
            }
            set {
                this["Check_CopyMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Check_SeqNumberAdd {
            get {
                return ((bool)(this["Check_SeqNumberAdd"]));
            }
            set {
                this["Check_SeqNumberAdd"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Check_Overwite {
            get {
                return ((bool)(this["Check_Overwite"]));
            }
            set {
                this["Check_Overwite"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("☓☓練習　その{REC_INDEX} ({REC_DATE})")]
        public string TextBox_Title {
            get {
                return ((string)(this["TextBox_Title"]));
            }
            set {
                this["TextBox_Title"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Check_YoutubeUpload {
            get {
                return ((bool)(this["Check_YoutubeUpload"]));
            }
            set {
                this["Check_YoutubeUpload"] = value;
            }
        }
    }
}
