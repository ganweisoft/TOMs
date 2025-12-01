//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IoTCenterHost.Core.Extension
{
    public static class AutoMapperExtension
    {
        #region 字段  
        private static Mapper Mapper;
        #endregion

        static AutoMapperExtension()
        {

        }
        #region 方法 

        public static void SetMapper(AutoMapper.IConfigurationProvider configurationProvider)
        {
            Mapper = new Mapper(configurationProvider);
        }
        public static TDestination Map<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        public static object Map(this object source, object destination, Type sourceType, Type destinationType)
        {
            return Mapper.Map(source, destination, sourceType, destinationType);
        }

        public static TDestination Map<TDestination>(this object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        public static IEnumerable<TDestination> Map<TDestination>(this IEnumerable source)
        {
            var result = new List<TDestination>();

            foreach (var item in source)
            {
                result.Add(item.Map<TDestination>());
            }

            return result;
        }
        public static TDestination[] MapArr<TDestination>(this IEnumerable source)
        {
            var result = new List<TDestination>();

            foreach (var item in source)
            {
                result.Add(item.Map<TDestination>());
            }

            return result.ToArray();
        }

        public static object Map(this object source, Type sourceType, Type destinationType)
        {
            return Mapper.Map(source, sourceType, destinationType);
        }

        public static TDestination Map<TSource, TDestination>(this TDestination destination, TSource source)
        {
            return Mapper.Map(source, destination);
        }
        #endregion
    }
}
