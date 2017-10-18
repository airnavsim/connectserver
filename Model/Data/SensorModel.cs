using System;
using System.Collections.Generic;
using System.Text;

namespace Cs.Model.Data
{
    public class SensorModel
    {
        public ulong Id { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }

        public bool CollectingEnable { get; set; }
        public DateTime CollectingLast { get; set; }
        public string SimCommand { get; set; }
        public List<string> CollectingClientsGuid { get; set; }

        public float Subscribeaccuracy { get; set; }

        public bool _ValueExist { get; set; }
        public string _Value { get; set; }
        public string _ValueLast { get; set; }
        public DateTime _ValueLastUpdated { get; set; }
        
    }
}
