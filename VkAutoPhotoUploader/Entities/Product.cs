using System;
using System.Linq;

using VkAutoPhotoUploader.Models;
using VkAutoPhotoUploader.Repositories;


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

        public void SavePhoto(int albumId) //TODO check contains photo in group
        {
            var caption = String.Format(Properties.Resources.PhotoDescription, Name, Price, ProductLink);
            var uploadServerHttpParams = String.Format(Properties.Resources.GetUploadServerUrl, albumId, SettingRepository.GetSettings().GroupId);
            var uploadServerModel = WebProcessor.VkReguest<UploadServerResult>(uploadServerHttpParams);
            var uploadPhotoModel = WebProcessor.SendPhotos(uploadServerModel.response.upload_url, PhotoBytes);
            var savePhotoHttpParams = String.Format(Properties.Resources.SavePhotoUrl, albumId, SettingRepository.GetSettings().GroupId, uploadPhotoModel.server, uploadPhotoModel.photos_list, uploadPhotoModel.hash, caption);
            var saveResult = WebProcessor.VkReguest<SavePhotoResult>(savePhotoHttpParams);

            if (saveResult.response[0].id.Contains("photo"))
            {
                IsSync = true;
                AlbumId = albumId;
                PhotoId = saveResult.response[0].id;
            }
        }

        public bool IsContainsInGroup()
        {
            var isContains = false;
            if (PhotoId == null)
                return false;
            try
            {
                var photoParam = String.Format(Properties.Resources.PhotoIdFormat, SettingRepository.GetSettings().GroupId, PhotoId.Split('_')[1]);
                var result = WebProcessor.VkReguest<PhotoGetByIdResult>(String.Format(Properties.Resources.GetPhotoByIdUrl, photoParam));
                if (result != null && result.response.Any())
                    isContains = true;
            }
            catch (Exception)
            {
                isContains = false;
            }
            
            return isContains;
        }

        public bool RemoveFromGroup()
        {
            var result = false;

            try
            {
                var isDeletePhoto = WebProcessor.VkReguest<DeletePhotoResult>(String.Format(Properties.Resources.DeletePhotoUrl, SettingRepository.GetSettings().GroupId, PhotoId.Split('_')[1]));
                result = isDeletePhoto?.response == 1;
            }
            catch (Exception)
            {
                result = false;
            }
            
            return result;
        }
    }
}
