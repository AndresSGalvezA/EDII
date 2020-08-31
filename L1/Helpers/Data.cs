using System.Collections.Generic;
using L1.Models;

namespace L1.Helpers
{
    public class Data
    {
        private static Data _instance = null;

        public static Data Instance
        {
            get
            {
                if (_instance == null) _instance = new Data();
                return _instance;
            }
        }
        
        public List<Drink> Items = new List<Drink>();
        public BTree<Drink> Tree = new BTree<Drink>(5);
    }
}