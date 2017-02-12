namespace VkAutoPhotoUploader.Models
{
    public class UploadPhotoModel
    {
        public int server { get; set; }
        public string photos_list { get; set; }
        public int aid { get; set; }
        public string hash { get; set; }
    }
}
