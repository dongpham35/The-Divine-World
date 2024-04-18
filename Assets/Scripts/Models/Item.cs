using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    internal class Item
    {
        public int itemID { get; set; }
        public string image { get; set; }   
        public string name {  get; set; }
        public string description { get; set; } 
        public string type { get; set; }
        public int value { get; set; }
    }
}
