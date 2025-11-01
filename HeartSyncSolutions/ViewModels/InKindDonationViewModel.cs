using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.ViewModels
{
    public class InKindDonationViewModel
    {
        [Required]
        public string DonationType { get; set; } = "inKind";

        // Item Description
        [Required(ErrorMessage = "Please describe the items you'd like to donate")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Description must be between 10 and 500 characters")]
        [Display(Name = "Item Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select an available date")]
        [Display(Name = "Drop-Off Date")]
        [DataType(DataType.Date)]
        public DateTime AvailableDate { get; set; }
    }
}