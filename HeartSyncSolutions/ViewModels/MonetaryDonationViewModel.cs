using System;
using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.ViewModels
{
    /// <summary>
    /// View model for monetary (cash) donations
    /// </summary>
    public class MonetaryDonationViewModel
    {
        public string MonetaryDonationId { get; set; }

        [Required(ErrorMessage = "Donation amount is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Donation amount must be greater than 0")]
        [Display(Name = "Donation Amount")]
        [DataType(DataType.Currency)]
        public double DonationAmount { get; set; }

        [Display(Name = "Donation Date")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Make this donation anonymous")]
        public bool IsAnonymous { get; set; }

        public string ApplicationUserId { get; set; }

        [Display(Name = "Donor Name")]
        public string DonorName { get; set; }

        [Display(Name = "Donor Email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string DonorEmail { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Message (Optional)")]
        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        public string Message { get; set; }
    }
}