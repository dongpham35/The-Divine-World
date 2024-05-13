using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    internal class Inventory_Item
    {
        private static Inventory_Item instance = new Inventory_Item();
        public static Inventory_Item Instance
        {
            get
            {
                return instance;
            }
        }
        public int inventoryID { get; set; }
        public int itemID { get; set; }
        public int quality { get; set; }

        public List<Inventory_Item> items { get; set; } = new List<Inventory_Item>();
    }
}
