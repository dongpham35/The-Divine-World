using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    internal class Up_level
    {
        private static Up_level instance = new Up_level();
        public static Up_level Instance { get =>  instance; }
        public int levelID { get; set; }
        public string @class { get; set; }
        public int blood { get; set; }
        public int attack_damage { get; set; }
        public int amor { get; set; }
        public int speed { get; set; }
        public List<Up_level> up_levels { get; set; } = new List<Up_level>();
    }
}
