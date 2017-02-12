using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using VkAutoPhotoUploader.Models;

namespace VkAutoPhotoUploader
{
    public class XmlRepository
    {
        private static AppSettings _appSettings;

        public static void SaveItem<T>(T item)
        {
            
        }

        public static AppSettings GetSettings()
        {
            if (_appSettings != null)
                return _appSettings;

            var fileName = Path.GetFullPath("App_Data/AppSettings.xml");
            var doc = XDocument.Load(fileName);

            Stream str = new MemoryStream();
            doc.Save(str);
            str.Position = 0;

            XmlSerializer xs = new XmlSerializer(typeof(AppSettings));
            _appSettings = (AppSettings)xs.Deserialize(str);

            return _appSettings;
        }
    }
}
