﻿using System.Data.Entity;
using System.Reflection;
using SmartElk.Antler.Domain;
using SmartElk.Antler.Domain.Configuration;
using SmartElk.Antler.EntityFramework.Internal;

namespace SmartElk.Antler.EntityFramework.Configuration
{
    public class EntityFrameworkPlusSqlServer: IEntityFrameworkStorage
    {        
        private Assembly _assemblyWithMappings;
        private string _connectionString;
        private IDatabaseInitializer<DataContext> _databaseInitializer;
        private bool _enableLazyLoading;

        protected EntityFrameworkPlusSqlServer()
        {
            _assemblyWithMappings = Assembly.GetCallingAssembly();
            _databaseInitializer = new DropCreateDatabaseAlways<DataContext>();
            _enableLazyLoading = true;
        }

        public static IEntityFrameworkStorage Use()
        {
            return new EntityFrameworkPlusSqlServer();            
        }

        public IEntityFrameworkStorage WithConnectionString(string connectionString)
        {
            this._connectionString = connectionString;
            return this;
        }

        public IEntityFrameworkStorage WithMappings(Assembly assembly)
        {
            this._assemblyWithMappings = assembly;
            return this;
        }

        public IEntityFrameworkStorage WithDatabaseInitializer(IDatabaseInitializer<DataContext> databaseInitializer)
        {
            this._databaseInitializer = databaseInitializer;
            return this;
        }

        public IEntityFrameworkStorage WithLazyLoading()
        {
            this._enableLazyLoading = true;
            return this;
        }

        public IEntityFrameworkStorage WithoutLazyLoading()
        {
            this._enableLazyLoading = false;
            return this;
        }

        public virtual void Configure(IDomainConfigurator configurator)
        {
            var dataContextFactory = string.IsNullOrEmpty(_connectionString)
                                         ? new DataContextFactory(_assemblyWithMappings, _enableLazyLoading)
                                         : new DataContextFactory(_connectionString, _assemblyWithMappings,
                                                                  _enableLazyLoading); 
            
            var sessionScopeFactory = new EntityFrameworkSessionScopeFactory(dataContextFactory);
            configurator.Configuration.Container.Put<ISessionScopeFactory>(sessionScopeFactory, configurator.Name);                                    
            Database.SetInitializer(_databaseInitializer);             
        }
    }
}
