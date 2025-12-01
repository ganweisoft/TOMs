// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections;
using System.Collections.Generic;

namespace IoTCenterCore.AutoMapper
{
    public static class ObjectMapperExtensions
    {
        private static IObjectMapping _mapper;

        public static void SetMapper(IObjectMapping mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public static TDestination MapTo<TDestination>(this object source)
        {
            if (_mapper == null)
            {
                throw new ArgumentNullException(nameof(_mapper));
            }
            return _mapper.Map<object, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            if (_mapper == null)
            {
                throw new ArgumentNullException(nameof(_mapper));
            }
            return _mapper.Map(source, destination);
        }

        public static List<TDestination> MapToList<TDestination>(this IEnumerable source)
        {
            return MapTo<List<TDestination>>(source);
        }
    }
}
