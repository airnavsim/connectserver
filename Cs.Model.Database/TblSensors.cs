using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cs.Model.Database
{
    [Table("tblsensors")]
    public class TblSensors
    {
        [Required]
        public ulong Id { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public string Xplane11Ext { get; set; }
        public float Accuracy { get; set; }

    }
}
