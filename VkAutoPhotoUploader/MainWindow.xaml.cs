﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using VkAutoPhotoUploader.Entities;
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
                var createAlbumHttpParams = String.Format(Properties.Resources.CreateAlbumUrl, items.First().CatalogName, _groupId);
                var albumId = WebProcessor.VkReguest<CreateAlbumResult>(createAlbumHttpParams).response.aid;

                foreach (var item in items)
                {
                    try
                    {
                        var caption = String.Format(Properties.Resources.PhotoDescription, item.Name, item.Price, item.ProductLink);
                        var uploadServerHttpParams = String.Format(Properties.Resources.GetUploadServerUrl, albumId, _groupId);
                        var uploadServerModel = WebProcessor.VkReguest<UploadServerResult>(uploadServerHttpParams);
                        var uploadPhotoModel = WebProcessor.SendPhotos(uploadServerModel.response.upload_url, item.PhotoBytes);
                        var savePhotoHttpParams = String.Format(Properties.Resources.SavePhotoUrl, albumId, _groupId, uploadPhotoModel.server, uploadPhotoModel.photos_list, uploadPhotoModel.hash, caption);
                        var saveResult = WebProcessor.VkReguest<SavePhotoResult>(savePhotoHttpParams);

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
