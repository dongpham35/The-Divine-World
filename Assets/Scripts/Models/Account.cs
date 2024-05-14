using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    internal class Account
    {
        private static Account instance = new Account();
        public static Account Instance 
        { get 
            { 
                return instance; 
            } 
        }


        public string username { get; set; }
        public string avatar { get;set; }
        public string email { get; set; }
        public string password { get; set; }
        public int gold { get; set; }
        public int levelID { get; set; }
        public string @class { get; set; }

        public int experience_points { get; set; }

    }
}
