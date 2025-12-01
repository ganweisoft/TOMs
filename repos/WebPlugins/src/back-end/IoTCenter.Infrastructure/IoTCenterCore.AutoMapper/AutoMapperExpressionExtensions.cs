// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Linq.Expressions;
using AutoMapper;

namespace IoTCenterCore.AutoMapper
{
    public static class AutoMapperExpressionExtensions
    {
        public static IMappingExpression<TDestination, TMember>
            Ignore<TDestination, TMember, TResult>(
            this IMappingExpression<TDestination, TMember> mappingExpression,
            Expression<Func<TMember, TResult>> destinationMember)
        {
            return mappingExpression.ForMember(destinationMember, options => options.Ignore());
        }
    }
}
