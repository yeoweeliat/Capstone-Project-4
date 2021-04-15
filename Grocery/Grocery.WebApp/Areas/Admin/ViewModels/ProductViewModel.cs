using Grocery.WebApp.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;



// viewmodel defines how to view the data in the model
// represent data in the product model onto the ui
// tells what to display, and how to display

namespace Grocery.WebApp.Areas.Admin.ViewModels
{

    // copy paste properties in model class to productviewmodel.
    public class ProductViewModel
    {
        //key attribute required from db attribute, pk field requires key attribute. (key required only for db schemas, db-related attributes are flushed out of vm)
        // when creating viewmodel we dont need it, vm does not need the identifier
        [Required]
        [Display(Name = "Product ID")]
        public Guid ProductID { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(80)] // use ui related attribute, stringlength replaced by maxlength
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public short Quantity { get; set; }

        //use decimal, because money does not allow you to control precision
        [Required]
        [Display(Name = "Selling Price Per Unit")]
        public decimal SellingPricePerUnit { get; set; }

        // byte array is the data you store in the db
        // ui, what it is, is a file uploading process
        // create ImageFile to receive file for you
        public byte[] Image { get; set; } //property to store the image received from the db


        //property to receive the file uploaded for the image
        [Display(Name = "Image for the Product")]
        public IFormFile ImageFile { get ; set; } 



        [Required]
        [Display(Name = "Created By")]
        public Guid CreatedByUserId { get; set; }

        [Display(Name = "Updated By")]
        public Guid? UpdatedByUserId { get; set; } // ? means nullable field

        [Required]
        [Display(Name = "Last Updated On")]
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
