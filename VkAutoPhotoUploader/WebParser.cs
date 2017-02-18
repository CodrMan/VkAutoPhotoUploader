using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

using VkAutoPhotoUploader.Models;
using VkAutoPhotoUploader.Repositories;


namespace VkAutoPhotoUploader
{
    public static class WebParser
    {

        private static readonly string DefaultPhotoUrl = SettingRepository.GetSettings().DefaultPhotoUrl;

        public static Products GetAllProducts()
        {
            var list = new List<Product>();
            var domain = SettingRepository.GetSettings().SiteDomain;
            var url = String.Format("{0}/katalog?&items_per_page=150&page=", domain);
            var counter = 0;

            while (true)
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url + counter);

                List<HtmlNode> toftitle = doc.DocumentNode.Descendants().Where
                    (x => (x.Name == "div" && x.Attributes["class"] != null &&
                           x.Attributes["class"].Value.Contains("views-row"))).ToList();

                if(!toftitle.Any())
                    break;

                foreach (var htmlNode in toftitle)
                {
                    var link = String.Format("{0}{1}", domain, htmlNode.ChildNodes[1].ChildNodes[3].Attributes[0].Value);
                    if (!link.Contains("content"))
                        link = String.Format("{0}{1}", domain, htmlNode.ChildNodes[1].ChildNodes[1].Attributes[0].Value);

                    var productInfo = GetProductInfoByLink(link);
                    list.Add(productInfo);
                }

                counter++;
            }

            return new Products() { ProductList = list };
        }

        private static Product GetProductInfoByLink(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            
            HtmlNode nodeCatalog = doc.DocumentNode.Descendants().FirstOrDefault(
                x => (x.Name == "div" && x.Attributes["class"] != null &&
                        x.Attributes["class"].Value.Contains("field-name-field-prosduct-catalog")));

            HtmlNode nodeTitle = doc.DocumentNode.Descendants().First(
                x => (x.Name == "div" && x.Attributes["class"] != null &&
                        x.Attributes["class"].Value.Contains("pane-page-title")));

            HtmlNode nodeImageUrl = doc.DocumentNode.Descendants().First(
                x => (x.Name == "li" && x.Attributes["class"] != null &&
                        x.Attributes["class"].Value == "gallery-slide"));

            HtmlNode nodePrice = doc.DocumentNode.Descendants().First(
                x => (x.Name == "div" && x.Attributes["class"] != null &&
                        x.Attributes["class"].Value.Contains("field-name-commerce-price")));
            

            return new Product()
            {
                Name = nodeTitle.InnerText.Replace("\n", "").Trim(),
                CatalogName = nodeCatalog != null ?  nodeCatalog.ChildNodes[1].InnerText : "Default",
                Price = nodePrice.InnerText,
                PhotoBytes = GetPhoto(nodeImageUrl.ChildNodes[1].Attributes[0].Value),
                ProductLink = url
            };
        }

        private static byte[] GetPhoto(string url)
        {
            HttpWebRequest request;
            HttpWebResponse response;

            var newUrl = url.Contains("_") ? url.Split('_')[0] + ".jpg" : url;

            for (int i = 0; i < 4; i++)
            {
                try
                {
                    request = WebRequest.Create(i == 3 ? newUrl : newUrl.Replace(".jpg", String.Format("_{0}.jpg", i))) as HttpWebRequest;
                    response = request.GetResponse() as HttpWebResponse;
                    var rez = ReadFully(response.GetResponseStream());
                    if (!rez.Any())
                        throw new Exception();
                    return rez;
                }
                catch (Exception) { }
            }

            request = WebRequest.Create(DefaultPhotoUrl) as HttpWebRequest;
            response = request.GetResponse() as HttpWebResponse;
            return ReadFully(response.GetResponseStream());
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
