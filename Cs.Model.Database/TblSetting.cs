using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


//https://stackoverflow.com/questions/39199049/what-is-the-equivalent-of-serializable-in-net-core-conversion-projects
//  [Serializable]
namespace Cs.Model.Database
{
    
    [Table("tblsetting")]
    public class TblSetting
    {
        [Required]
        public string Name { get; set;}
        public string Value { get; set; }
        
    }
}
