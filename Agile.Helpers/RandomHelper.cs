using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Helpers
{
    public class RandomHelper
    {
        private static Random _instance = null;

        public static Random Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Random();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
    }
}
