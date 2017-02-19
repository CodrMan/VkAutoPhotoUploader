namespace VkAutoPhotoUploader.Models
{
    public class PhotoGetByIdResult
    {
        public InnerModel[] response { get; set; }

        public class InnerModel
        {
            public int pid { get; set; }
        }
    }
}
