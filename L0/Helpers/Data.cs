using System.Collections.Generic;

namespace L0.Helpers
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

        public Queue<Movie> Movies = new Queue<Movie>();
    }
}
