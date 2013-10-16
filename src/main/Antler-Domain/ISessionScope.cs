﻿using System;

namespace SmartElk.Antler.Domain
{
    public interface ISessionScope: IDisposable
    {
        void Commit();
        void Rollback();
        IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class;
        object InternalSession { get; }
    }
}
