using System;
using System.Collections.Generic;

namespace Project.Models
{
    public class BranchProductRequest : IComparable
    {
        public int IdBranch { get; set; }
        public int IdProduct { get; set; }
        public int Inventory { get; set; }

        public static BranchProductRequest Update(object info, string[] freshInfo)
        {
            var originInfo = (BranchProductRequest)info;
            originInfo.Inventory = Convert.ToInt32(freshInfo[0]);
            return originInfo;
        }

        public static BranchProductRequest StringToObject(string info)
        {
            var infoSeparada = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                infoSeparada.Add(info.Substring(0, 100));
                info = info.Substring(100);
            }

            var auxInvent = 0;
            int.TryParse(SDES.CipDec(infoSeparada[2], false), out auxInvent);

            return new BranchProductRequest()
            {
                IdBranch = Convert.ToInt32(infoSeparada[0].Trim()),
                IdProduct = Convert.ToInt32(infoSeparada[1].Trim()),
                Inventory = auxInvent
            };
        }

        public int CompareTo(object obj) { return $"{IdBranch}-{this.IdProduct}".CompareTo($"{((BranchProductRequest)obj).IdBranch}-{((BranchProductRequest)obj).IdProduct}"); }

        public static string ObjectToString(object Nuevo)
        {
            var Actual = (BranchProductRequest)Nuevo;
            return $"{string.Format("{0,-100}", Actual.IdBranch.ToString())}{string.Format("{0,-100}", Actual.IdProduct.ToString())}{SDES.CipDec(string.Format("{0,-100}", Actual.Inventory.ToString()), true)}";
        }
    }
}