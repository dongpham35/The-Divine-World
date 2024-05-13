using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    internal class Item_Attached
    {
        private static Item_Attached instance = new Item_Attached();
        public static Item_Attached Instance
        {
            get
            {
                return instance;
            }
        }
        public int item_attachedID { get;set; }
        public List<int> item_attacheds { get; set; } = new List<int> { 0,0,0,0,0,0 };
        public int num_item_attached { get; set; }
        public string username { get; set; }
    
    }
}
