﻿using FluentNHibernate;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernateDemo.Data;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Api
{
    public static class NHibernateExtensions
    {
        public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionString)
        {
            var mapper = new ModelMapper();
            var assembly = typeof(BaseEntity).Assembly;

            mapper.AddMappings(assembly.ExportedTypes);
            HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            var configuration = new Configuration();
            configuration.DataBaseIntegration(c =>
            {
                c.Dialect<PostgreSQLDialect>();
                c.Driver<NpgsqlDriver>();
                c.ConnectionString = connectionString;
                c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                c.LogFormattedSql = true;
                c.LogSqlInConsole = true;
            });
            configuration.AddMapping(domainMapping);

            var persistenceModel = new PersistenceModel();
            persistenceModel.AddMappingsFromAssembly(assembly);
            persistenceModel.Configure(configuration);

            var sessionFactory = configuration.BuildSessionFactory();

            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => sessionFactory.OpenSession());
            services.AddScoped(factory => sessionFactory.OpenStatelessSession());
            services.AddScoped<ISamuraiAppDbContext, SamuraiAppDbContext>();

            return services;
        }
    }
}
