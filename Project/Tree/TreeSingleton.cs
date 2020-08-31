using System;

namespace Project.Tree
{
    public class TreeSingleton
    {
        private static TreeSingleton _instance = null;
        public Delegate GetNode;
        public Delegate GetString;
        public int Rank;
        public int Key;
        public string FilePath;
        
        public static TreeSingleton Instance
        {
            get
            {
                if (_instance == null) _instance = new TreeSingleton();
                return _instance;
            }
        }
    }
}