using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    internal class Friend
    {
        
        private static Friend instance= new Friend();
        public static Friend Instance
        {
            get
            {
                return instance;
            }
        }
        public int friendID { get; set; }
        public string username { get; set; }
        public string username2ID { get; set; }

        public List<Friend> friends { get; set;} = new List<Friend>();
    }
}
