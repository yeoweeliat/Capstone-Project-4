using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Grocery.WebApp.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Grocery.WebApp.Models
{
    public class MyIdentityUser : IdentityUser<Guid>
    {
        [Required]
        [Display(Name="Display Name")]
        [MinLength(2)]
        [MaxLength(60)]
        [StringLength(60)] // can remove db related
        public string DisplayName { get; set; }
        
        [Required]
        [Display(Name = "Gender")]
        [PersonalData] // for GDPR compliance, inform the ecosystem that it is (gdpr compliant), government compliance we need to adhere to
        public MyAppGenderTypes Gender { get; set; } //property can be enum class
    
        [Required]
        [Display(Name = "Date of Birth")]
        [PersonalData]
        [Column(TypeName="smalldatetime")] //create as datacolumn of this type, not added, default is datetime2
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Is Admin User?")] // set default value=false not here, set to fluent api
        public bool IsAdminUser { get; set; }

        [Display(Name = "Photo")]
        public byte[] Photo { get; set; }


        
        // 1-to-many relationship
        #region Navigation Properties

        [ForeignKey(nameof(Product.CreatedByUserId))] //point to CreatedByUserId_FK in product table
        public ICollection<Product> ProductsCreatedByUser { get; set; }

        [ForeignKey(nameof(Product.UpdatedByUserId))]
        public ICollection<Product> ProductsUpdatedByUser { get; set; }

        #endregion
        
    }
}
