using CG.Business.Models;
using CG.Business.Repositories;
using CG.Linq.MongoDb.Repositories.Options;
using CG.Validations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;

namespace CG.Linq.MongoDb.Repositories
{
    /// <summary>
    /// This class is a base MongoDb implementation of the <see cref="ILinqRepository{TModel}"/>
    /// interface.
    /// </summary>
    /// <typeparam name="TOptions">The options type associated with the repository.</typeparam>
    /// <typeparam name="TModel">The model type associated with the repository.</typeparam>
    public abstract class MongoDbRepository<TOptions, TModel> :
        LinqRepositoryBase<TOptions, TModel>,
        ILinqRepository<TModel>
        where TModel : class, IModel
        where TOptions : IOptions<MongoDbRepositoryOptions>
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the MongoDb client associated with the repository.
        /// </summary>
        protected MongoClient Client { get; set; }

        /// <summary>
        /// This property contains the name of the MongoDb container.
        /// </summary>
        protected string ContainerName { get; set; }

        /// <summary>
        /// This property contains a reference to a MongoDb database.
        /// </summary>
        protected IMongoDatabase Database { get; private set; }

        /// <summary>
        /// This property contains a reference to a MongoDb collection.
        /// </summary>
        protected IMongoCollection<TModel> Collection { get; private set; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="MongoDbRepository{TOptions, TModel}"/>
        /// class.
        /// </summary>
        /// <param name="options">The options to use for the repository.</param>
        /// <param name="client">The CosmoDb client to use with the repository.</param>
        protected MongoDbRepository(
            TOptions options,
            MongoClient client
            ) : base(options)
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(client, nameof(client));

            // Save the references.
            Client = client;

            // We'll tie the collection back to the model type here by pluralizing
            //   the model's type name and using that as the collection name.
            ContainerName = PluralizationService.CreateService(
                new CultureInfo("en") // Always pluralize in english
                ).Pluralize(typeof(TModel).Name);

            // Get the database.
            Database = Client.GetDatabase(
                Options.Value.DatabaseId
                );

            // Get the collection.
            Collection = Database.GetCollection<TModel>(ContainerName);
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc />
        public override IQueryable<TModel> AsQueryable()
        {
            // Defer to the MongoDb collection.
            return Collection.AsQueryable<TModel>();
        }

        #endregion
    }
}
