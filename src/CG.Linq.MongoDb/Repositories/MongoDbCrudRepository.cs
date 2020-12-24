using CG.Business.Models;
using CG.Business.Repositories;
using CG.Linq.MongoDb.Properties;
using CG.Linq.MongoDb.Repositories.Options;
using CG.Validations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Linq.MongoDb.Repositories
{
    /// <summary>
    /// This class is a base MongoDb implementation of the <see cref="ICrudRepository{TModel, TKey}"/>
    /// interface.
    /// </summary>
    /// <typeparam name="TOptions">The options type associated with the repository.</typeparam>
    /// <typeparam name="TModel">The type of associated model.</typeparam>
    /// <typeparam name="TKey">The key type associated with the model.</typeparam>
    public abstract class MongoDbCrudRepository<TOptions, TModel, TKey> :
        CrudRepositoryBase<TOptions, TModel, TKey>,
        ICrudRepository<TModel, TKey>
        where TModel : class, IModel<TKey>
        where TOptions : IOptions<MongoDbRepositoryOptions>
        where TKey : new()
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
        /// This constructor creates a new instance of the <see cref="MongoDbCrudRepository{TOptions, TModel, TKey}"/>
        /// class.
        /// </summary>
        /// <param name="options">The options to use for the repository.</param>
        /// <param name="client">The CosmoDb client to use with the repository.</param>
        protected MongoDbCrudRepository(
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

        // *******************************************************************

        /// <inheritdoc />
        public override async Task<TModel> AddAsync(
            TModel model,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(model, nameof(model));

            try
            {
                // Is the key missing?
                if (KeyUtility.IsKeyMissing(model.Key))
                {
                    // Create a new random key value.
                    model.Key = KeyUtility.CreateRandomKey<TKey>();
                }

                // Defer to the CosmoDb container.
                await Collection.InsertOneAsync(
                    model,
                    new InsertOneOptions() { BypassDocumentValidation = true },
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the model.
                return model;
            }
            catch (Exception ex)
            {
                // Add better context to the error.
                throw new RepositoryException(
                    message: string.Format(
                        Resources.MongoDbCrudRepository_AddAsync,
                        nameof(MongoDbCrudRepository<TOptions, TModel, TKey>),
                        typeof(TModel).Name,
                        JsonSerializer.Serialize(model)
                        ),
                    innerException: ex
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public override async Task<TModel> UpdateAsync(
            TModel model,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(model, nameof(model));

            try
            {
                // Perform the update where the key matches something in the database.
                var filter = Builders<TModel>.Filter.Eq("Key", model.Key);

                // Defer to the CosmoDb container.
                var newModel = await Collection.FindOneAndReplaceAsync(
                    filter,
                    model,
                    null,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the result.
                return newModel;
            }
            catch (Exception ex)
            {
                // Add better context to the error.
                throw new RepositoryException(
                    message: string.Format(
                        Resources.MongoDbCrudRepository_UpdateAsync,
                        nameof(MongoDbCrudRepository<TOptions, TModel, TKey>),
                        typeof(TModel).Name,
                        JsonSerializer.Serialize(model)
                        ),
                    innerException: ex
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public override async Task DeleteAsync(
            TModel model,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(model, nameof(model));

            try
            {
                // Perform the delete where the key matches something in the database.
                var filter = Builders<TModel>.Filter.Eq("Key", model.Key);

                // Defer to the CosmoDb container.
                await Collection.DeleteOneAsync(
                    filter,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Add better context to the error.
                throw new RepositoryException(
                    message: string.Format(
                        Resources.MongoDbCrudRepository_DeleteAsync,
                        nameof(MongoDbCrudRepository<TOptions, TModel, TKey>),
                        typeof(TModel).Name,
                        JsonSerializer.Serialize(model)
                        ),
                    innerException: ex
                    );
            }
        }

        #endregion
    }



    /// <summary>
    /// This class is a base MongoDb implementation of the <see cref="ICrudRepository{TModel, TKey1, TKey2}"/>
    /// interface.
    /// </summary>
    /// <typeparam name="TOptions">The options type associated with the repository.</typeparam>
    /// <typeparam name="TModel">The type of associated model.</typeparam>
    /// <typeparam name="TKey1">The key 1 type associated with the model.</typeparam>
    /// <typeparam name="TKey2">The key 2 type associated with the model.</typeparam>
    public abstract class MongoDbCrudRepository<TOptions, TModel, TKey1, TKey2> :
        CrudRepositoryBase<TOptions, TModel, TKey1, TKey2>,
        ICrudRepository<TModel, TKey1, TKey2>
        where TModel : class, IModel<TKey1, TKey2>
        where TOptions : IOptions<MongoDbRepositoryOptions>
        where TKey1 : new()
        where TKey2 : new()
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
        /// This constructor creates a new instance of the <see cref="MongoDbCrudRepository{TOptions, TModel, TKey1, TKey2}"/>
        /// class.
        /// </summary>
        /// <param name="options">The options to use for the repository.</param>
        /// <param name="client">The CosmoDb client to use with the repository.</param>
        protected MongoDbCrudRepository(
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

        // *******************************************************************

        /// <inheritdoc />
        public override async Task<TModel> AddAsync(
            TModel model,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(model, nameof(model));

            try
            {
                // Defer to the CosmoDb container.
                await Collection.InsertOneAsync(
                    model,
                    new InsertOneOptions() { BypassDocumentValidation = true },
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the model.
                return model;
            }
            catch (Exception ex)
            {
                // Add better context to the error.
                throw new RepositoryException(
                    message: string.Format(
                        Resources.MongoDbCrudRepository_AddAsync,
                        nameof(MongoDbCrudRepository<TOptions, TModel, TKey1, TKey2>),
                        typeof(TModel).Name,
                        JsonSerializer.Serialize(model)
                        ),
                    innerException: ex
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public override async Task<TModel> UpdateAsync(
            TModel model,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(model, nameof(model));

            try
            {
                // Perform the update where the key matches something in the database.
                var filter = Builders<TModel>.Filter.Eq("Key", $"{model.Key1}|{model.Key2}");

                // Defer to the CosmoDb container.
                var newModel = await Collection.FindOneAndReplaceAsync(
                    filter,
                    model,
                    null,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the result.
                return newModel;
            }
            catch (Exception ex)
            {
                // Add better context to the error.
                throw new RepositoryException(
                    message: string.Format(
                        Resources.MongoDbCrudRepository_UpdateAsync,
                        nameof(MongoDbCrudRepository<TOptions, TModel, TKey1, TKey2>),
                        typeof(TModel).Name,
                        JsonSerializer.Serialize(model)
                        ),
                    innerException: ex
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public override async Task DeleteAsync(
            TModel model,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(model, nameof(model));

            try
            {
                // Perform the delete where the key matches something in the database.
                var filter = Builders<TModel>.Filter.Eq("Key", $"{model.Key1}|{model.Key2}");

                // Defer to the CosmoDb container.
                await Collection.DeleteOneAsync(
                    filter,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Add better context to the error.
                throw new RepositoryException(
                    message: string.Format(
                        Resources.MongoDbCrudRepository_DeleteAsync,
                        nameof(MongoDbCrudRepository<TOptions, TModel, TKey1, TKey2>),
                        typeof(TModel).Name,
                        JsonSerializer.Serialize(model)
                        ),
                    innerException: ex
                    );
            }
        }

        #endregion
    }



    /// <summary>
    /// This class is a base MongoDb implementation of the <see cref="ICrudRepository{TModel, TKey1, TKey2, TKey3}"/>
    /// interface.
    /// </summary>
    /// <typeparam name="TOptions">The options type associated with the repository.</typeparam>
    /// <typeparam name="TModel">The type of associated model.</typeparam>
    /// <typeparam name="TKey1">The key 1 type associated with the model.</typeparam>
    /// <typeparam name="TKey2">The key 2 type associated with the model.</typeparam>
    /// <typeparam name="TKey3">The key 3 type associated with the model.</typeparam>
    public abstract class MongoDbCrudRepository<TOptions, TModel, TKey1, TKey2, TKey3> :
        CrudRepositoryBase<TOptions, TModel, TKey1, TKey2, TKey3>,
        ICrudRepository<TModel, TKey1, TKey2, TKey3>
        where TModel : class, IModel<TKey1, TKey2, TKey3>
        where TOptions : IOptions<MongoDbRepositoryOptions>
        where TKey1 : new()
        where TKey2 : new()
        where TKey3 : new()
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
        /// This constructor creates a new instance of the <see cref="MongoDbCrudRepository{TOptions, TModel, TKey1, TKey2, TKey3}"/>
        /// class.
        /// </summary>
        /// <param name="options">The options to use for the repository.</param>
        /// <param name="client">The CosmoDb client to use with the repository.</param>
        protected MongoDbCrudRepository(
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

        // *******************************************************************

        /// <inheritdoc />
        public override async Task<TModel> AddAsync(
            TModel model,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(model, nameof(model));

            try
            {
                // Defer to the CosmoDb container.
                await Collection.InsertOneAsync(
                    model,
                    new InsertOneOptions() { BypassDocumentValidation = true },
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the model.
                return model;
            }
            catch (Exception ex)
            {
                // Add better context to the error.
                throw new RepositoryException(
                    message: string.Format(
                        Resources.MongoDbCrudRepository_AddAsync,
                        nameof(MongoDbCrudRepository<TOptions, TModel, TKey1, TKey2, TKey3>),
                        typeof(TModel).Name,
                        JsonSerializer.Serialize(model)
                        ),
                    innerException: ex
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public override async Task<TModel> UpdateAsync(
            TModel model,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(model, nameof(model));

            try
            {
                // Perform the update where the key matches something in the database.
                var filter = Builders<TModel>.Filter.Eq("Key", $"{model.Key1}|{model.Key2}|{model.Key3}");

                // Defer to the CosmoDb container.
                var newModel = await Collection.FindOneAndReplaceAsync(
                    filter,
                    model,
                    null,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the result.
                return newModel;
            }
            catch (Exception ex)
            {
                // Add better context to the error.
                throw new RepositoryException(
                    message: string.Format(
                        Resources.MongoDbCrudRepository_UpdateAsync,
                        nameof(MongoDbCrudRepository<TOptions, TModel, TKey1, TKey2, TKey3>),
                        typeof(TModel).Name,
                        JsonSerializer.Serialize(model)
                        ),
                    innerException: ex
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public override async Task DeleteAsync(
            TModel model,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(model, nameof(model));

            try
            {
                // Perform the delete where the key matches something in the database.
                var filter = Builders<TModel>.Filter.Eq("Key", $"{model.Key1}|{model.Key2}|{model.Key3}");

                // Defer to the CosmoDb container.
                await Collection.DeleteOneAsync(
                    filter,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Add better context to the error.
                throw new RepositoryException(
                    message: string.Format(
                        Resources.MongoDbCrudRepository_DeleteAsync,
                        nameof(MongoDbCrudRepository<TOptions, TModel, TKey1, TKey2, TKey3>),
                        typeof(TModel).Name,
                        JsonSerializer.Serialize(model)
                        ),
                    innerException: ex
                    );
            }
        }

        #endregion
    }
}
