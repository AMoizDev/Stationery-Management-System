using System.ComponentModel.DataAnnotations.Schema;

namespace Stationery_Management_System.Models
{
    public class Request
    {
        public int requestId { get; set; }
        public int userId { get; set; }
        
        public int? stationaryId { get; set; }
        [ForeignKey("stationaryId")]
        public virtual Stationery? Stationery { get; set; }
        public int quantity { get; set; }
        public int amount { get; set; }

        public int superior_id { get; set; }
        [ForeignKey("superior_id")]
        public virtual users Users { get; set; }
        public string status { get; set; }

    }
}
