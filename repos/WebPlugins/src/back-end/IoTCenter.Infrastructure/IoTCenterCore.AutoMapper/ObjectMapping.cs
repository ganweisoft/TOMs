// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using AutoMapper;
using AutoMapper.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IoTCenterCore.AutoMapper
{
    public class ObjectMapping : IObjectMapping
    {
        private static readonly object sync = new object();
        private IConfigurationProvider _config;
        private IMapper _mapper;

        public ObjectMapping(IConfigurationProvider config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _mapper = _config.CreateMapper();
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return Map<TSource, TDestination>(source, default);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            try
            {
                if (source == null)
                {
                    return default;
                }

                var sourceType = GetType(source);

                var destinationType = GetType(destination);

                return GetResult(sourceType, destinationType, source, destination);
            }
            catch (AutoMapperMappingException ex)
            {
                return GetResult(GetType(ex.MemberMap?.SourceType), GetType(ex.MemberMap?.DestinationType), source, destination);
            }
        }

        private Type GetType<T>(T obj)
        {
            if (obj == null)
            {
                return GetType(typeof(T));
            }
            return GetType(obj.GetType());
        }

        public static bool IsCollection(Type type)
        {
            if (type.IsArray)
            {
                return true;
            }

            return IsGenericCollection(type);
        }

        public static bool IsGenericCollection(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            var typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(IEnumerable<>)
                   || typeDefinition == typeof(IReadOnlyCollection<>)
                   || typeDefinition == typeof(IReadOnlyList<>)
                   || typeDefinition == typeof(ICollection<>)
                   || typeDefinition == typeof(IList<>)
                   || typeDefinition == typeof(List<>);
        }

        private Type GetType(Type type)
        {
            if (!IsCollection(type))
            {
                return type;
            }

            if (type.IsArray)
            {
                return type.GetElementType();
            }

            var genericArgumentsTypes = type.GetTypeInfo().GetGenericArguments();
            if (genericArgumentsTypes == null || genericArgumentsTypes.Length == 0)
            {
                throw new ArgumentException(nameof(genericArgumentsTypes));
            }

            return genericArgumentsTypes[0];
        }

        private TDestination GetResult<TDestination>(Type sourceType, Type destinationType, object source, TDestination destination)
        {
            if (Exists(sourceType, destinationType))
            {
                return GetResult(source, destination);
            }

            lock (sync)
            {
                if (Exists(sourceType, destinationType))
                {
                    return GetResult(source, destination);
                }

                ConfigMap(sourceType, destinationType);
            }
            return GetResult(source, destination);
        }

        private bool Exists(Type sourceType, Type destinationType)
        {
            return _config.Internal().FindTypeMapFor(sourceType, destinationType) != null;
        }

        private TDestination GetResult<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _mapper.Map(source, destination);
        }

        private void ConfigMap(Type sourceType, Type destinationType)
        {
            var maps = _config.Internal().GetAllTypeMaps();

            _config = new MapperConfiguration(t =>
            {
                t.CreateMap(sourceType, destinationType);

                foreach (var map in maps)
                {
                    t.CreateMap(map.SourceType, map.DestinationType);
                }
            });

            _mapper = _config.CreateMapper();
        }
    }
}
