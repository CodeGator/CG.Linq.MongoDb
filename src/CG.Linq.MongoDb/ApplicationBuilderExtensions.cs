using CG.Linq.MongoDb.Repositories.Options;
using CG.Validations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// This delegate type represents a callback to seed a database.
    /// </summary>
    /// <typeparam name="TClient">The type of associated MongoDb client.</typeparam>
    /// <param name="client">The MongoDb client to use for the operation.</param>
    /// <param name="wasDropped">Indicates whether the data-context was recently dropped.</param>
    /// <param name="wasCreated">Indicates whether the data-context was recently created.</param>
    public delegate void SeedAction<in TClient>(
        TClient client,
        bool wasDropped,
        bool wasCreated
        ) where TClient : MongoClient;



    /// <summary>
    /// This class contains extension methods related to the <see cref="IApplicationBuilder"/>
    /// type.
    /// </summary>
    public static partial class ApplicationBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method performs any startup logic required by MongoDb, such as 
        /// dropping the underlying database (if needed), or creating the underlying 
        /// database (if needed), or adding seed data to an otherwise blank 
        /// database. 
        /// </summary>
        /// <typeparam name="TClient">The type of associated client.</typeparam>
        /// <typeparam name="TOptions">The type of associated options.</typeparam>
        /// <param name="applicationBuilder">The application builder to use for 
        /// the operation.</param>
        /// <param name="seedDelegate">A delegate for seeding the database with 
        /// startup data.</param>
        /// <returns>The value of the <paramref name="applicationBuilder"/>
        /// parameter, for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever one
        /// or more arguments are invalid, or missing.</exception>
        public static IApplicationBuilder UseCosmoDb<TClient, TOptions>(
            this IApplicationBuilder applicationBuilder,
            SeedAction<TClient> seedDelegate
            ) where TClient : MongoClient
              where TOptions : MongoDbRepositoryOptions
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(applicationBuilder, nameof(applicationBuilder))
                .ThrowIfNull(seedDelegate, nameof(seedDelegate));

            // Get the registered options.
            var options = applicationBuilder.ApplicationServices.GetRequiredService<
                IOptions<TOptions>
                >();

            var wasDropped = false;
            var wasCreated = false;

            // Should we manipulate the database?
            if (options.Value.EnsureCreated ||
                options.Value.DropDatabase ||
                options.Value.SeedDatabase)
            {
                // Apply any pending migrations.
                using (var scope = applicationBuilder.ApplicationServices.CreateScope())
                {
                    // Get a CosmoDb client.
                    var client = scope.ServiceProvider.GetService<TClient>();

                    // Should we drop the database? 
                    if (options.Value.DropDatabase)
                    {
                        // Drop the database.
                        client.DropDatabase(
                            options.Value.DatabaseId
                            );

                        // Keep track of what we've done.
                        wasDropped = true;
                    }

                    // Should we make sure the database exists?
                    if (options.Value.EnsureCreated)
                    {
                        // Get the database.
                        var database = client.GetDatabase(
                            options.Value.DatabaseId
                            );

                        // Keep track of what we've done.
                        wasCreated = true;
                    }

                    // Should we make sure the database has seed data?
                    if (options.Value.SeedDatabase)
                    {
                        // Perform the data seeding operation.
                        seedDelegate(
                            client,
                            wasDropped,
                            wasCreated
                            );
                    }
                }
            }

            // Return the application builder.
            return applicationBuilder;
        }

        #endregion
    }
}
