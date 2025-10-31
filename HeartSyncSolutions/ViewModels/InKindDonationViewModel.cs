using System;
using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.ViewModels
{
    /// <summary>
    /// View model for in-kind (physical item) donations - Drop-off only
    /// </summary>
    public class InKindDonationViewModel
    {
        public string InKindOfferId { get; set; }

        [Required(ErrorMessage = "Item description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters", MinimumLength = 10)]
        [Display(Name = "Item Description")]
        public string ItemDescription { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "New Offer";

        [Display(Name = "Offered Date")]
        [DataType(DataType.DateTime)]
        public DateTime OfferedDate { get; set; } = DateTime.Now;

        public string ApplicationUserId { get; set; }

        [Display(Name = "Donor Name")]
        public string DonorName { get; set; }

        [Display(Name = "Contact Email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string ContactEmail { get; set; }

        [Display(Name = "Contact Phone")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string ContactPhone { get; set; }

        [Display(Name = "Item Category")]
        public string ItemCategory { get; set; }

        [Display(Name = "Estimated Quantity")]
        public string Quantity { get; set; }

        // This is always false - donations must be dropped off
        public bool RequiresPickup { get; set; } = false;

        // Not used since pickup is not available
        public string PickupAddress { get; set; }

        [Display(Name = "Additional Notes")]
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string AdditionalNotes { get; set; }
    }
}