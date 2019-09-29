using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models.AccountViewModels
{
    public class CompleteRegisterViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(30)]
        public string Gender { get; set; }

        [Required]
 
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Address (Line 1)")]
        public string Addressline1 { get; set; }

        [StringLength(100)]
        [Display(Name = "Address (Line 2)")]
        public string Addressline2 { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "State/Province")]
        public string Province { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Zip/Postal Code")]
        public string ZipCode { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }


    }
}
