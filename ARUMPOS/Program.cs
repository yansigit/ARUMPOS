using ArumModels.Models;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using System.IO;
using Splat;
using System.Net.Http;

namespace ARUMPOS
{
    class Program
    {
        public static int ShopId;
        public static readonly HttpClient HttpClient = new HttpClient();

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            using StreamReader file = File.OpenText(@"settings.json");
            using JsonTextReader reader = new(file);
            var json = (JObject)JToken.ReadFrom(reader);
            ShopId = json["shopId"].ToObject<int>();

            // 프린트 테스트
            var shop = Shop.GetById(ShopId);
            POSPrinterLibrary.POSFunctions.TestPrinting(shop.PrinterCOM, shop.BaudRate);

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);

            //Console.Write("�����߽��ϴ�.");
        }

        public static HttpClient SetHttpClientItnjSecretKey(string key)
        {
            HttpClient.DefaultRequestHeaders.Remove("itnj_key");
            HttpClient.DefaultRequestHeaders.Add("itnj_key", key);
            return HttpClient;
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .AfterSetup(AfterSetupCallback)
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        private static void AfterSetupCallback(AppBuilder appBuilder)
        {
            // InitWsClient();
            IconProvider.Register<FontAwesomeIconProvider>();
        }
    }
}
