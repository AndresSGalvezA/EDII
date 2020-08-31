using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class BranchOfficeRequest:IComparable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public static BranchOfficeRequest Update(object info, string[] freshInfo)
        {
            var originInfo = (BranchOfficeRequest)info;
            originInfo.Name = freshInfo[0] == null ? originInfo.Name : freshInfo[0];
            originInfo.Address = freshInfo[1] == null ? originInfo.Address : freshInfo[1];
            return originInfo;
        }

        public static BranchOfficeRequest StringToObject(string info)
        {
            var SeparedText = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                SeparedText.Add(info.Substring(0, 100));
                info = info.Substring(100);
            }

            return new BranchOfficeRequest()
            {
                ID = Convert.ToInt32(SeparedText[0].Trim()),
                Name = SDES.CipDec(SeparedText[1], false).Trim(),
                Address = SDES.CipDec(SeparedText[2], false).Trim()
            };
        }

        public int CompareTo(object obj) { return this.ID.CompareTo(((BranchOfficeRequest)obj).ID); }

        public static string ObjectToString(object Nuevo)
        {
            var Actual = (BranchOfficeRequest)Nuevo;
            Actual.Name = Actual.Name == null ? string.Empty : Actual.Name;
            Actual.Address = Actual.Address == null ? string.Empty : Actual.Address;
            return $"{string.Format("{0,-100}", Actual.ID.ToString())}{SDES.CipDec(string.Format("{0,-100}", Actual.Name), true)}{SDES.CipDec(string.Format("{0,-100}", Actual.Address), true)}";
        }
    }
}
