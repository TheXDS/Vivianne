﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TheXDS.Vivianne.Resources.Strings.Tools {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class FceCleanupTool {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal FceCleanupTool() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TheXDS.Vivianne.Resources.Strings.Tools.FceCleanupTool", typeof(FceCleanupTool).Assembly);
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
        ///   Looks up a localized string similar to {0}: {1} triangles with invalid flags.
        /// </summary>
        internal static string BadTriangleFlags_comp {
            get {
                return ResourceManager.GetString("BadTriangleFlags_comp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The following parts have triangles with invalid material flags:
        ///{0}.
        /// </summary>
        internal static string BadTriangleFlags_Details {
            get {
                return ResourceManager.GetString("BadTriangleFlags_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} triangles with invalid flags.
        /// </summary>
        internal static string BadTriangleFlags_Title {
            get {
                return ResourceManager.GetString("BadTriangleFlags_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The header indicates that there&apos;s more than one art defined in the FCE file. This is not consistent with FCE parsing in NFS3..
        /// </summary>
        internal static string HeaderDamage_Arts_Details {
            get {
                return ResourceManager.GetString("HeaderDamage_Arts_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Damaged header: Unexpected number of arts.
        /// </summary>
        internal static string HeaderDamage_Arts_Title {
            get {
                return ResourceManager.GetString("HeaderDamage_Arts_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of declared car parts is outside the valid range of values..
        /// </summary>
        internal static string HeaderDamage_CarPartCount_Details {
            get {
                return ResourceManager.GetString("HeaderDamage_CarPartCount_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid car part count.
        /// </summary>
        internal static string HeaderDamage_CarPartCount_Title {
            get {
                return ResourceManager.GetString("HeaderDamage_CarPartCount_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of declared colors is outside the valid range of values..
        /// </summary>
        internal static string HeaderDamage_ColorCount_Details {
            get {
                return ResourceManager.GetString("HeaderDamage_ColorCount_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid primary color count.
        /// </summary>
        internal static string HeaderDamage_ColorCount_Title {
            get {
                return ResourceManager.GetString("HeaderDamage_ColorCount_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of declared primary and secondary colors does not match..
        /// </summary>
        internal static string HeaderDamage_ColorMismatch_Details {
            get {
                return ResourceManager.GetString("HeaderDamage_ColorMismatch_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Primary/secondary color mismatch.
        /// </summary>
        internal static string HeaderDamage_ColorMismatch_Title {
            get {
                return ResourceManager.GetString("HeaderDamage_ColorMismatch_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The header declares more Dummy parts than supported by the FCE file format, suggesting an overrun of dummy part definitions. The number of parts will be truncated to 16..
        /// </summary>
        internal static string HeaderDamage_DummyCount_Details {
            get {
                return ResourceManager.GetString("HeaderDamage_DummyCount_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Damaged header: Invalid number of Dummies.
        /// </summary>
        internal static string HeaderDamage_DummyCount_Title {
            get {
                return ResourceManager.GetString("HeaderDamage_DummyCount_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There&apos;s more car part names than car parts in the FCE file. These unused names can be removed safely..
        /// </summary>
        internal static string StrayPartNames_1_Details {
            get {
                return ResourceManager.GetString("StrayPartNames_1_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} stray part name(s).
        /// </summary>
        internal static string StrayPartNames_1_Title {
            get {
                return ResourceManager.GetString("StrayPartNames_1_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There&apos;s more car parts than car part names in the FCE File. A name for these parts will be inferred and set. Please verify that the part is in the proper location and has the appropriate name..
        /// </summary>
        internal static string StrayPartNames_2_Details {
            get {
                return ResourceManager.GetString("StrayPartNames_2_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} unnamed part(s).
        /// </summary>
        internal static string StrayPartNames_2_Title {
            get {
                return ResourceManager.GetString("StrayPartNames_2_Title", resourceCulture);
            }
        }
    }
}
