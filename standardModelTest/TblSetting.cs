using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cs.Model.Database
{
    [Table("tblsetting")]
    public class TblSetting
    {
        [Required]
        public string Name { get; set; }
        public string Value { get; set; }

    }
}
