using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace FlashcardsAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void InterceptWith<TInterceptor, TInterface, TImplementation>(this IServiceCollection services)
            where TInterceptor : class, IInterceptor
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddTransient<TInterface>(serviceProvider =>
            {
                var proxyGenerator = new ProxyGenerator();
                var actualService = serviceProvider.GetRequiredService<TImplementation>();
                var interceptor = serviceProvider.GetRequiredService<TInterceptor>();
                return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(actualService, interceptor);
            });

            services.AddTransient<TImplementation>();
        }
    }
}
