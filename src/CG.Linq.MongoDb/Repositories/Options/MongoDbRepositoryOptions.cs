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

        #endregion
    }
}
