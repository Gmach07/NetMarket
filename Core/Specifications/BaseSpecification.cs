﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {

        public BaseSpecification() { }
        public BaseSpecification(Expression<Func<T, bool>> criteria)  
        {
            Criteria = criteria;    
        }
        public Expression<Func<T, bool>> Criteria { get; }

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        protected void AddInclude (Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        // para ordenar 
        public Expression<Func<T, object>> OrderBy { get; private set; }

        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression) {
            OrderBy = orderByExpression;

        }
        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;

        }
        // para paginacion 
        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPagingEnable { get; private set; }

        protected void ApplyPaging (int skip, int take)
        {
            Skip = skip;
            Take = take;    
            IsPagingEnable = true;
        }
    }
}
