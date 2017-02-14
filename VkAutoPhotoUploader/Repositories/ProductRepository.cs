using System.IO;
using Newtonsoft.Json;
using VkAutoPhotoUploader.Models;

namespace VkAutoPhotoUploader.Repositories
{
    public class ProductRepository
    {
        private static readonly string ProductFilePath = Path.GetFullPath("App_Data/Products.json");

        //private static readonly string ProductFilePath = Path.GetFullPath("App_Data/ProductsResult.json");

        public static void SaveProducts(Products item, bool isResult)
        {
            var filePath = isResult ? Path.GetFullPath("App_Data/ProductsResult.json") : ProductFilePath;

            var str = JsonConvert.SerializeObject(item, Formatting.Indented);
            File.WriteAllText(filePath, str);
        }

        public static Products GetProducts()
        {
            return JsonConvert.DeserializeObject<Products>(File.ReadAllText(ProductFilePath));
        }
    }
}
