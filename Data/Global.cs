
using JPFData.Models.Identity;

namespace JPFData
{
    public class Global
    {
        private static volatile Global _instance;
        private static readonly object Lock = new object();
        

        private Global()
        {
        }


        public ApplicationUser User { get; set; }


        public static Global Instance
        {
            get
            {
                lock (Lock)
                {
                    if(_instance == null)
                        _instance = new Global();
                }

                return _instance;
            }
        }
    }
}
