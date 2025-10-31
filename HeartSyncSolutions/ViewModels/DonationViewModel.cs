using System;
using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.ViewModels
{
    /// <summary>
    /// Base view model containing shared attributes for all donation types
    /// </summary>
    public class DonationViewModel
    {
        public string DonationId { get; set; }

        [Display(Name = "Donation Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Donor Name")]
        public string DonorName { get; set; }

        [Display(Name = "Donation Type")]
        public string DonationType { get; set; }

        public string ApplicationUserId { get; set; }

        [Display(Name = "Anonymous Donation")]
        public bool IsAnonymous { get; set; }
    }
}