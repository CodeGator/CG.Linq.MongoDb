﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CG.Linq.MongoDb.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CG.Linq.MongoDb.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to {0} failed to add a new {1}! parameter: {2}. See inner exception(s) for more detail..
        /// </summary>
        internal static string MongoDbCrudRepository_AddAsync {
            get {
                return ResourceManager.GetString("MongoDbCrudRepository_AddAsync", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} failed to delete a {1}! parameter: {2}. See inner exception(s) for more detail..
        /// </summary>
        internal static string MongoDbCrudRepository_DeleteAsync {
            get {
                return ResourceManager.GetString("MongoDbCrudRepository_DeleteAsync", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} failed to update a {1}! parameter: {2}. See inner exception(s) for more detail..
        /// </summary>
        internal static string MongoDbCrudRepository_UpdateAsync {
            get {
                return ResourceManager.GetString("MongoDbCrudRepository_UpdateAsync", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MongoDbRepository options require a &apos;DatabaseId&apos; field and it appears to be missing, or empty. .
        /// </summary>
        internal static string MongoDbRepositoryOptions_DbId {
            get {
                return ResourceManager.GetString("MongoDbRepositoryOptions_DbId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MongoDbRepository options require a &apos;Uri&apos; field and it appears to be missing, or empty..
        /// </summary>
        internal static string MongoDbRepositoryOptions_Uri {
            get {
                return ResourceManager.GetString("MongoDbRepositoryOptions_Uri", resourceCulture);
            }
        }
    }
}