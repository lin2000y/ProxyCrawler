﻿using Autofac;
using ProxyCrawler.Job;
using ProxyCrawler.ProxyProviders;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;

#if !DEBUG
using Topshelf;
#endif

namespace ProxyCrawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogHelper.LogInit();
            DependencyInjection();
#if DEBUG
            new SyncProxyJob().Execute(null).ConfigureAwait(false).GetAwaiter().GetResult();
            Console.WriteLine("Job finished");
            Console.ReadLine();
#else
            HostFactory.Run(host =>
            {
                host.RunAsLocalSystem();
                host.StartAutomaticallyDelayed();
                host.Service<QuartzService>();
                host.SetServiceName("ProxyCrawler");
                host.SetDisplayName("ProxyCrawler");
            });
#endif
        }

        private static void DependencyInjection()
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new StdSchedulerFactory(new NameValueCollection { { "quartz.serializer.type", "binary" } }))
                .SingleInstance();

            // TODO:Baibian Ip，Ip解码
            // builder.RegisterType<BaibianIpProxyProvider>().As<IProxyProvider>();
            builder.RegisterType<CoderBusyProxyProvider>().As<IProxyProvider>();
            builder.RegisterType<KuaidailiProxyProvider>().As<IProxyProvider>();
            builder.RegisterType<MayiProxyProvider>().As<IProxyProvider>();
            builder.RegisterType<SixIpProxyProvider>().As<IProxyProvider>();
            builder.RegisterType<XicidailiProxyProvider>().As<IProxyProvider>();
            builder.RegisterType<YundailiProxyProvider>().As<IProxyProvider>();

            var container = builder.Build();
            DependencyResolver.SetDependencyResolver(t => container.Resolve(t));
        }
    }
}