using Project.Tree;
using System;
using System.Collections.Generic;
using System.IO;

namespace Project.Models
{
    public class ProductRequest : IComparable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        public int CompareTo(object obj) { return ID.CompareTo(((ProductRequest)obj).ID); }

        public static string ObjectToString(object obj)
        {
            var Actual = (ProductRequest)obj;
            Actual.Name ??= string.Empty;
            return $"{string.Format("{0,-100}", Actual.ID.ToString())}{SDES.CipDec(string.Format("{0,-100}", Actual.Name), true)}{SDES.CipDec(string.Format("{0,-100}", Actual.Price.ToString()), true)}";
        }

        public static ProductRequest StringToObject(string info)
        {
            var infoSeparada = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                infoSeparada.Add(info.Substring(0, 100));
                info = info.Substring(100);
            }

            var auxPrice = 0.00;
            double.TryParse(SDES.CipDec(infoSeparada[2], false).Trim(), out auxPrice);

            return new ProductRequest()
            {
                ID = Convert.ToInt32(infoSeparada[0].Trim()),
                Name = SDES.CipDec(infoSeparada[1], false).Trim(),
                Price = auxPrice
            };
        }

        public static ProductRequest Update(object info, string[] freshInfo)
        {
            var originInfo = (ProductRequest)info;
            originInfo.Name = freshInfo[0] == null ? originInfo.Name : freshInfo[0];
            originInfo.Price = freshInfo[1] == null ? originInfo.Price : Convert.ToDouble(freshInfo[1]);
            return originInfo;
        }

        public static void InsertCSV(Stream info)
        {
            using var archive = new StreamReader(info);
            var line = "";

            while ((line = archive.ReadLine()) != null)
            {
                var arrayAux = line.Split(';');
                var nameComplete = arrayAux[0].Split('\"');
                var priceNumber = arrayAux[1].Split('\"', ',');
                BStarTree<ProductRequest>.InsertTree(new ProductRequest { ID = BStarTree<ProductRequest>.CreateId(), Name = nameComplete[1], Price = Convert.ToDouble(priceNumber[2]) });
            }
        }
    }
}