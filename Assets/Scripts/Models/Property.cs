using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    internal class Property
    {
        private static Property instance = new Property();
        public static Property Instance
        {
            get
            {
                return instance;
            }
        }
        public int propertyID { get; set; }
        public string username { get; set; }
        public int blood { get; set; }
        public int attack_damage { get; set; }
        public int amor { get; set; }
        public int critical_rate { get; set; }  
        public int speed { get; set; }
        public int amor_penetraction { get; set; } 
    }
}
