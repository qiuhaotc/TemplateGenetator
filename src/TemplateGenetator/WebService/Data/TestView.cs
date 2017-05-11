using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebService.Data
{
    [Table("TestView")]
    public class TestView
    {
        [Key]
        public int TP_ID { get; set; }
        public string TP_Name { get; set; }
        public string TP_URL { get; set; }
        public string TP_Desc { get; set; }
        public string TP_AddDate { get; set; }
        public int TP_Order { get; set; }
    }
}