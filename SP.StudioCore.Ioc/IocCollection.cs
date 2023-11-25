﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace SP.StudioCore.Ioc
{
    public static class IocCollection
    {
        private static IServiceCollection? _services;
        private static IServiceProvider? _provider;

        /// <summary>
        /// 加入到自定义的IOC容器中
        /// </summary>
        /// <param name="services"></param>
        public static void AddService(this IServiceCollection services)
        {
            _services = services;
        }

        public static IServiceCollection? AddService<TService>(ServiceLifetime contextLifetime) where TService : class
        {
            return contextLifetime switch
            {
                ServiceLifetime.Singleton => _services?.AddSingleton<TService>(),
                ServiceLifetime.Scoped => _services?.AddScoped<TService>(),
                ServiceLifetime.Transient => _services?.AddTransient<TService>(),
                _ => throw new NotSupportedException()
            };
        }

        public static IServiceCollection? AddService<TService>(TService service, ServiceLifetime contextLifetime) where TService : class
        {
            return contextLifetime switch
            {
                ServiceLifetime.Singleton => _services?.AddSingleton(service),
                ServiceLifetime.Scoped => _services?.AddScoped(t => service),
                ServiceLifetime.Transient => _services?.AddTransient(t => service),
                _ => throw new NotSupportedException()
            };
        }


        private static IServiceProvider? Provider
        {
            get
            {
                if (HttpContextService.Current != null)
                {
                    return _provider = HttpContextService.Current.RequestServices;
                }
                return _provider ??= _services?.BuildServiceProvider();
            }
        }

        /// <summary>
        /// 从容器中拿出对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? GetService<T>() where T : class
        {
            return Provider?.GetService<T>();
        }

        public static object? GetService(Type serviceType)
        {
            return Provider?.GetService(serviceType);
        }

        public static IEnumerable<T>? GetServices<T>()
        {
            return Provider?.GetServices<T>();
        }
    }
}
