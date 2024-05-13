using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    internal class Sessions
    {
        private static Sessions instance = new Sessions();
        public static Sessions Instance
        {
            get
            {
                return instance;
            }
        }
        public int sessionID { get; set; }
        public string username { get; set; }
        public string username2ID { get;set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get;set; }
        public string winnerID { get;set; }
        public List<Sessions> sessions { get; set; } = new List<Sessions>();
    }
}
