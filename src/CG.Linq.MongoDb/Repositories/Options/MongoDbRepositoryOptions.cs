using CG.Business.Repositories.Options;
using CG.Linq.MongoDb.Properties;
using System;
using System.ComponentModel.DataAnnotations;

namespace CG.Linq.MongoDb.Repositories.Options
{
    /// <summary>
    /// This class contains configuration settings for a MongodDb repository.
    /// </summary>
    public class MongoDbRepositoryOptions : RepositoryOptions
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the database identifier to use with the repository.
        /// </summary>
        [Required(ErrorMessageResourceName = "MongoDbRepositoryOptions_DbId",
                  ErrorMessageResourceType = typeof(Resources))]
        public string DatabaseId { get; set; }

        /// <summary>
        /// This property contains the Uri to use with the repository.
        /// </summary>
        [Required(ErrorMessageResourceName = "MongoDbRepositoryOptions_Uri",
                  ErrorMessageResourceType = typeof(Resources))]
        public string Uri { get; set; }

        /// <summary>
        /// This property indicates whether the database should be created, if
        /// needed, at startup (or not). Note, this step is only ever performed
        /// when running in the <c>Development</c> environment, in order to 
        /// prevent horrible accidents in production.
        /// </summary>
        public bool EnsureCreated { get; set; }

        /// <summary>
        /// This property indicates whether the database should be dropped, if 
        /// it already exists (or not). Note, this step is only ever performed
        /// when running in the <c>Development</c> environment, in order to 
        /// prevent horrible accidents in production.
        /// </summary>
        public bool DropDatabase { get; set; }

        /// <summary>
        /// This property indicates whether the database should be seeded with 
        /// data, if needed, at startup (or not). Note, this step is only ever 
        /// performed when running in the <c>Development</c> environment, in order 
        /// to prevent horrible accidents in production.
        /// </summary>
        public bool SeedDatabase { get; set; }

        #endregion
    }
}
