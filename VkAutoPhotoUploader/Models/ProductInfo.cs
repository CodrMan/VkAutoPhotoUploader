using System;

namespace VkAutoPhotoUploader.Models
{
    public class ProductInfo : IEquatable<ProductInfo>
    {
        public string PhotoUrl { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string CatalogName { get; set; }
        public string ProductLink { get; set; }
        public bool IsSync { get; set; }

        public bool Equals(ProductInfo other)
        {
            return PhotoUrl == other.PhotoUrl;
        }
    }
}
