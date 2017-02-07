using System.Xml.Serialization;

namespace VkAutoPhotoUploader.Models
{
    [XmlRoot(ElementName = "response")]
    public class UploadServerModel
    {
        [XmlElement("album_id")]
        public int AlbumId { get; set; }

        [XmlElement("user_id")]
        public int UserId { get; set; }

        [XmlElement("upload_url")]
        public string UploadUrl { get; set; }
    }
}
