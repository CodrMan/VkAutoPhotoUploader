using System.IO;
using Newtonsoft.Json;

using VkAutoPhotoUploader.Models;


namespace VkAutoPhotoUploader.Repositories
{
    public class ProductRepository
    {
        private static readonly string ProductFilePath = Path.GetFullPath("App_Data/Products.json");

        public static void SaveProducts(Products item)
        {
            var str = JsonConvert.SerializeObject(item, Formatting.Indented);
            File.WriteAllText(ProductFilePath, str);
        }

        public static Products GetProducts()
        {
            return JsonConvert.DeserializeObject<Products>(File.ReadAllText(ProductFilePath));
        }
    }
}
