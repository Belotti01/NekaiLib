﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nekai.Analyzers {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Nekai.Analyzers.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Throwing Exceptions inside constructors can result in unwanted behaviour..
        /// </summary>
        internal static string DontThrowInConstructorsDescription {
            get {
                return ResourceManager.GetString("DontThrowInConstructorsDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Constructor &apos;{0}&apos; might throw an Exception..
        /// </summary>
        internal static string DontThrowInConstructorsMessageFormat {
            get {
                return ResourceManager.GetString("DontThrowInConstructorsMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Constructors should not throw Exceptions..
        /// </summary>
        internal static string DontThrowInConstructorsTitle {
            get {
                return ResourceManager.GetString("DontThrowInConstructorsTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Remove base type..
        /// </summary>
        internal static string OperationResultBaseTypeCodeFix {
            get {
                return ResourceManager.GetString("OperationResultBaseTypeCodeFix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OperationResult enums should use int as their base type..
        /// </summary>
        internal static string OperationResultBaseTypeDescription {
            get {
                return ResourceManager.GetString("OperationResultBaseTypeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enum &apos;{0}&apos;&apos;s base type is &apos;{1}&apos;, but &apos;int&apos; was expected..
        /// </summary>
        internal static string OperationResultBaseTypeMessageFormat {
            get {
                return ResourceManager.GetString("OperationResultBaseTypeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OperationResult enums should use int as their base type..
        /// </summary>
        internal static string OperationResultBaseTypeTitle {
            get {
                return ResourceManager.GetString("OperationResultBaseTypeTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OperationResult enums must be decorated with the OperationResultAttribute..
        /// </summary>
        internal static string OperationResultWithoutAttributeDescription {
            get {
                return ResourceManager.GetString("OperationResultWithoutAttributeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enum &apos;{0}&apos; is missing the OperationResultAttribute.
        /// </summary>
        internal static string OperationResultWithoutAttributeMessageFormat {
            get {
                return ResourceManager.GetString("OperationResultWithoutAttributeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OperationResult enum is missing OperationResultAttribute..
        /// </summary>
        internal static string OperationResultWithoutAttributeTitle {
            get {
                return ResourceManager.GetString("OperationResultWithoutAttributeTitle", resourceCulture);
            }
        }
    }
}
