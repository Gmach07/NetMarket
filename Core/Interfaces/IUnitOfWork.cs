﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
      
            IGenericRepository<TEntity> Repository<TEntity>() where TEntity : ClaseBase;

            Task<int> Complete();
        
    }
}
