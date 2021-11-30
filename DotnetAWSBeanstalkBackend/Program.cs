using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DotnetAWSBeanstalkBackend.Controllers;

namespace DotnetAWSBeanstalkBackend
{
    public class Program
    {
        public static readonly HttpClient HttpClient = new();

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static HttpClient SetHttpClientItnjSecretKey(string key)
        {
            HttpClient.DefaultRequestHeaders.Remove("itnj_key");
            HttpClient.DefaultRequestHeaders.Add("itnj_key", key);
            return HttpClient;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
