using System;
using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.ViewModels
{
    /// <summary>
    /// View model for requesting in-kind donations from the community
    /// </summary>
    public class DonationRequestViewModel
    {
        public string RequestId { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(200, ErrorMessage = "Item name cannot exceed 200 characters")]
        [Display(Name = "Item Needed")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Quantity needed is required")]
        [Display(Name = "Quantity Needed")]
        public string QuantityNeeded { get; set; }

        [Display(Name = "Priority Level")]
        public string Priority { get; set; } = "Medium";

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Needed By Date")]
        [DataType(DataType.Date)]
        public DateTime? NeededByDate { get; set; }

        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Display(Name = "Request Status")]
        public string Status { get; set; } = "Active";

        public string RequestedByUserId { get; set; }

        [Display(Name = "Requested By")]
        public string RequestedByName { get; set; }
    }
}