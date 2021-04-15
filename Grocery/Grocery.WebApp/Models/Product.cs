using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


//everytime you make a change to your model, you need to do add-migration and update-database

namespace Grocery.WebApp.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Required]
        [Column(name: "ProductId")] // name of column in db
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // to indicate db constraint
        [Comment("The unique ID of the product...")] //db schema related documentation
        public Guid ProductID { get; set; }


        [Required]
        [StringLength(80)]
        [Column(TypeName = "varchar")]
        [Comment("The name of thepProduct sold by the store...")]
        public string ProductName { get; set; }

        
        [Required]
        [Comment("The quantity of products currently available in the store...")]
        //[DefaultValue(0)] // will not work in all databases, done using onmodelcreating() method
        public short Quantity { get; set; }


        //use decimal, because money does not allow you to control precision
        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal SellingPricePerUnit { get; set; }


        [Comment("The image of the product...")]
        public byte[] Image { get; set; }

        [Required]
        public Guid CreatedByUserId { get; set; }

        public Guid? UpdatedByUserId { get; set; } // ? means nullable field

        [Required]
        public DateTime LastUpdatedOn { get; set; }


        // define how you navigate between objects,
        // need to create navigation properties section in applicationuser class,  - for setting the configuration process for fk relationship

        // establish a relationship between product table and  myidentityuser table
        // hash region is a way of clumbing code together (add mapping mechanism that we want)
        

        #region Navigation Properties

        public MyIdentityUser CreatedByUser { get; set; }
        public MyIdentityUser UpdatedByUser { get; set; }

        #endregion
        
    }
}
