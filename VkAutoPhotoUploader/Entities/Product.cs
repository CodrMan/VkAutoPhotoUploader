namespace VkAutoPhotoUploader.Entities
{
    public class Product
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string PhotoId { get; set; }
        public string CatalogName { get; set; }
        public string ProductLink { get; set; }
        public int AlbumId { get; set; }
        public bool IsSync { get; set; }
        public byte[] PhotoBytes { get; set; }
    }
}
