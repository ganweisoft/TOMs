//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using AutoMapper;
using System;
using System.Linq.Expressions;

namespace IoTCenterHost.Core.Extension
{
    public class ModelProfile<TSource, TDestination>
    {
        private IMappingExpression<TSource, TDestination> _mappingExpression;
        private ModelProfile _profile;

        private ModelProfile()
        {
            _profile = new ModelProfile();
        }
        public Profile AutomapperProfile => _profile;

        public static ModelProfile<TSource, TDestination> CreateMap()
        {
            var result = new ModelProfile<TSource, TDestination>();
            result._mappingExpression = result._profile.CreateMap<TSource, TDestination>();
            return result;
        }

        public ModelProfile<TSource, TDestination> ForMember(string destinationProperty, string sourceProperty)
        {
            _mappingExpression = _mappingExpression.ForMember(destinationProperty, config => config.MapFrom(sourceProperty));
            return this;
        }

        public ModelProfile<TSource, TDestination> ForMember<TMember>(string destinationProperty, Expression<Func<TSource, TMember>> sourceProperty)
        {
            _mappingExpression = _mappingExpression.ForMember(destinationProperty, config => config.MapFrom(sourceProperty));
            return this;
        }

        public ModelProfile<TSource, TDestination> ForMember<TMember>(Expression<Func<TDestination, TMember>> destinationMember, Expression<Func<TSource, TMember>> sourceMember)
        {
            _mappingExpression = _mappingExpression.ForMember(destinationMember, config => config.MapFrom(sourceMember));
            return this;
        }

        public ModelProfile<TSource, TDestination> IgnoreMember<TMember>(Expression<Func<TDestination, TMember>> destinationMember)
        {
            _mappingExpression = _mappingExpression.ForMember(destinationMember, config => config.Ignore());
            return this;
        }
    }

    class ModelProfile : Profile
    {
    }
}
