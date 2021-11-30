using Amazon;
using Amazon.S3.Transfer;
using ArumModels.Models;
using Avalonia.Controls;
using Material.Dialog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ARUMPOS.Lib
{
    public class Common
    {
        async public static Task<string> UploadFileAsync(string filePath, string name, string variant = "menus")
        {
            const string bucketName = "arumdream";
            RegionEndpoint bucketRegion = RegionEndpoint.APNortheast2;

            try
            {
                var fileTransferUtility = new TransferUtility("AKIAWZYLJVAKZKJX37PY", "gvYx86zJfxqKfycKr6w8cjiaawQMk5rheSmroZJ5", bucketRegion);
                var fileExt = filePath.Split('.').Last();
                var key = $"uploads/{variant}/{name}.{fileExt}";
                await fileTransferUtility.UploadAsync(filePath, bucketName, key);

                return key;
            }
            catch (Exception e)
            {
                DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                {
                    Borderless = false,
                    ContentHeader = "문제가 있습니다",
                    StartupLocation = WindowStartupLocation.CenterOwner,
                    SupportingText = e.ToString()
                });

                return "";
            }
        }
    }
}