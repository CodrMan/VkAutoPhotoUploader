using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;

using VkAutoPhotoUploader.Models;
using VkAutoPhotoUploader.Properties;
using VkAutoPhotoUploader.Repositories;


namespace VkAutoPhotoUploader
{
    public partial class MainWindow : Window
    {
        private readonly string _groupId = SettingRepository.GetSettings().GroupId;

        public MainWindow()
        {
            InitializeComponent();
            CreateSettings();

            this.txtLog.AppendText("\n");
            AddLog("Start programm!");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.auth || Settings.Default.timeOut < DateTime.Now)
            {
                new Authorization(this).Show();
                AddLog("Start authorization!");
            }
        }

        private void getProductsbtn_Click(object sender, RoutedEventArgs e)
        {
            var products = WebParser.GetAllProducts();
            ProductRepository.SaveProducts(products);
        }

        private void startUploadbtn_Click(object sender, RoutedEventArgs e)
        {
            var products = ProductRepository.GetProducts();
            var groupByCatalog = products.ProductList.GroupBy(x => x.CatalogName);

            foreach (IEnumerable<Product> items in groupByCatalog)
            {
                var createAlbumHttpParams = String.Format("photos.createAlbum?title={0}&group_id={1}&upload_by_admins_only=1", items.First().CatalogName, _groupId);
                var albumId = WebProcessor.VkReguest<CreateAlbumModel>(createAlbumHttpParams).response.aid;

                foreach (var item in items)
                {
                    try
                    {
                        var caption = String.Format("{0}\n\nЦена: {1}\n\n{2}", item.Name, item.Price, item.ProductLink);
                        var uploadServerHttpParams = String.Format("photos.getUploadServer?album_id={0}&group_id={1}", albumId, _groupId);
                        var uploadServerModel = WebProcessor.VkReguest<UploadServerModel>(uploadServerHttpParams);
                        var uploadPhotoModel = WebProcessor.SendPhotos(uploadServerModel.response.upload_url, item.PhotoBytes);
                        var savePhotoHttpParams = String.Format("photos.save?album_id={0}&group_id={1}&server={2}&photos_list={3}&hash={4}&caption={5}",
                            albumId, _groupId, uploadPhotoModel.server, uploadPhotoModel.photos_list, uploadPhotoModel.hash, caption);
                        var saveResult = WebProcessor.VkReguest<SavePhotoResultModel>(savePhotoHttpParams);

                        if (saveResult.response[0].id.Contains("photo"))
                        {
                            item.IsSync = true;
                            item.AlbumId = albumId;
                            item.PhotoId = saveResult.response[0].id;
                        }

                        Thread.Sleep(1000);
                    }
                    catch (Exception)
                    {
                        item.IsSync = false;
                    }
                }
            }

            ProductRepository.SaveProducts(products);
        }

        private void CreateSettings()
        {
            Settings.Default.auth = true;
            Settings.Default.id = "";
            Settings.Default.token = "";
            Settings.Default.timeOut = DateTime.Now;
            Settings.Default.Save();
        }

        public void AddLog(string str)
        {
            this.txtLog.AppendText(DateTime.Now.ToString("dd.MM.yy  hh:mm:ss") + " : " + str + "\n");
            this.txtLog.Focus();
            this.txtLog.CaretIndex = this.txtLog.Text.Length;
        } //TODO event
    }
}
