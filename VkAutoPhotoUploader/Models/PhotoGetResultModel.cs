namespace VkAutoPhotoUploader.Models
{
    public class PhotoGetResultModel
    {
        public InnerModel response { get; set; }

        public class InnerModel
        {
            public int count { get; set; }
            public string[] items { get; set; }
        }
    }
}
