﻿using CG.Linq.MongoDb.Repositories.Options;
using CG.Validations;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IServiceCollection"/>
    /// type, for registering types related to MongoDb
    /// </summary>
    public static partial class MongoDbServiceCollectionExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method loads and registers MongoDb specific repository options.
        /// </summary>
        /// <typeparam name="TOptions">The type of associated repository options.</typeparam>
        /// <param name="serviceCollection">The service collection to use for 
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <param name="serviceLifetime">The service lifetime to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/>
        /// parameter, for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever one
        /// or more arguments are invalid, or missing.</exception>
        public static IServiceCollection AddMongoDbRepositories<TOptions>(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped
            ) where TOptions : MongoDbRepositoryOptions, new()
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Register the repository options.
            serviceCollection.ConfigureOptions<TOptions>(
                configuration
                );

            // Return the service collection.
            return serviceCollection;
        }

        #endregion
    }
}
