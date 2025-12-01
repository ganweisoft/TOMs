// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data.Model;
using IoTCenterWebApi.BaseCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ganweisoft.IoTCenter.Module.EquipList
{
    public static class PredicateBuilder
    {
        public static IQueryable<SetParm> ApplyRoleEquipFilter(this IQueryable<SetParm> query,
            PermissionObj currentRolePermission)
        {
            if (currentRolePermission == null)
            {
                return query;
            }

            var browseEquips = currentRolePermission.BrowseEquips;
            var controlEquips = currentRolePermission.ControlEquips;
            var controlEquipsUnit = currentRolePermission.ControlEquipsUnitOfDict;

            if (!browseEquips.Any())
            {
                return query.Where(e => false);
            }

            var baseQuery = query.Where(e => browseEquips.Contains(e.EquipNo));

            if (!controlEquips.Any() && !controlEquipsUnit.Any())
            {
                return baseQuery;
            }

            Expression<Func<SetParm, bool>> combinedCondition = e => false;

            if (controlEquips.Any())
            {
                var condition = PredicateBuilder.In<SetParm>(e => e.EquipNo, controlEquips);
                combinedCondition = combinedCondition.Or(condition);
            }

            foreach (var (equipNo, setNos) in controlEquipsUnit)
            {
                var condition = PredicateBuilder.And<SetParm>(
                    e => e.EquipNo == equipNo,
                    e => setNos.Contains(e.SetNo)
                );
                combinedCondition = combinedCondition.Or(condition);
            }

            return baseQuery.Where(combinedCondition);
        }


        public static Expression<Func<T, bool>> True<T>() => _ => true;
        public static Expression<Func<T, bool>> False<T>() => _ => false;

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> In<T>(Expression<Func<T, int>> selector, IEnumerable<int> values)
        {
            if (!values.Any())
            {
                return x => false;
            }

            var equals = values.Select(value => (Expression)Expression.Equal(selector.Body, Expression.Constant(value, typeof(int))));

            var body = equals.Aggregate<Expression>(Expression.OrElse);

            return Expression.Lambda<Func<T, bool>>(body, selector.Parameters);
        }
    }
}
