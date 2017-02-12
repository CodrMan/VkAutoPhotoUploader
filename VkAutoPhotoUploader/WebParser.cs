using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using VkAutoPhotoUploader.Models;
using VkAutoPhotoUploader.Repositories;

namespace VkAutoPhotoUploader
{
    public static class WebParser
    {
        public static Products GetAllProducts()
        {
            var list = new List<ProductInfo>();
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

            return new Products() { ProductInfos = list };
        }

        private static ProductInfo GetProductInfoByLink(string url)
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
            

            return new ProductInfo()
            {
                Name = nodeTitle.InnerText.Replace("\n", "").Trim(),
                CatalogName = nodeCatalog != null ?  nodeCatalog.ChildNodes[1].InnerText : "Default",
                Price = nodePrice.InnerText,
                PhotoUrl = nodeImageUrl.ChildNodes[1].Attributes[0].Value,
                ProductLink = url
            };
        }
    }
}
