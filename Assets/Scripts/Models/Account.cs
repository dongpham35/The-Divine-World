using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    internal class Account
    {
        public string username { get; set; }
        public string avatar { get;set; }
        public string email { get; set; }
        public string password { get; set; }
        public int gold { get; set; }
        public int levelID { get; set; }
        public string @class { get; set; }

        public Account()
        {
            this.username = "";
            this.avatar = "";
            this.email = "";
            this.password = "";
            this.gold = 0;
            this.levelID = 0;
            this.@class = "";
        }
    }
}
