using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    internal class Item
    {
        private static Item instance = new Item();
        public static Item Instance
        {
            get
            {
                return instance;
            }
        }
        public int itemID { get; set; }
        public string image { get; set; }   
        public string name {  get; set; }
        public string description { get; set; } 
        public string type { get; set; }
        public int value { get; set; }

        public int cost { get;set; }
        public List<Item> items { get; set; } = new List<Item>();
    }
}
