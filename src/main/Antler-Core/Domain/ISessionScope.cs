﻿using System;

namespace SmartElk.Antler.Core.Domain
{
    public interface ISessionScope: IDisposable
    {
        void Commit();        
        IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class;
        TInternal GetInternal<TInternal>() where TInternal : class;
    }
}
