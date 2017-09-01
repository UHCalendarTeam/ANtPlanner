using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace CalDavServices.Extensions
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        ///     Ignores a member for a given selector
        ///     see: https://stackoverflow.com/questions/26898442/how-to-ignore-a-property-in-automapper
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="map"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IMappingExpression<TSource, TDestination> IgnoreMember<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> map, Expression<Func<TDestination, object>> selector)
        {
            map.ForMember(selector, config => config.Ignore());
            return map;
        }
    }
}