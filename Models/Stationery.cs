using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fingers10.ExcelExport.Attributes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Stationery_Management_System.Models
{
    public class Stationery
    {
        [Key]
        [IncludeInReport(Order =1)]
        [DisplayName("Stationery id")]
        public int Stationery_Id { get; set; }



        [Required]
        [IncludeInReport(Order = 2)]
        [DisplayName("Stationery Name")]

        public string Stationery_Name { get; set; }




        [Required(ErrorMessage = "Stationery Quantity is Requerd ")]
        [Range(1, 1000, ErrorMessage = "Stationery Quantity must be between 1 and 1000")]
        [IncludeInReport(Order = 3)]
        [DisplayName("Stationery Quantity")]
        public int Stationery_Quantity { get; set; }





        [IncludeInReport(Order = 4)]
        [DisplayName("Stationery Description")]
        public string? Stationery_Description { get; set; }



        [IncludeInReport(Order = 5)]
        [DisplayName("Stationery Price")]
        public int Stationery_Price { get; set; }
       

        //[Required]
        public string Stationery_Image { get; set; }

        
        public int? Assign_to { get; set; }
        [ForeignKey("Assign_to")]
        public virtual UserRoles? Roles { get; set; }
         

    }


}
