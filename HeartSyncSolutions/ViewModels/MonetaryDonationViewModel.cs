using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.ViewModels
{
    public class MonetaryDonationViewModel
    {
        // Donation Type (hidden field in form)
        [Required]
        public string DonationType { get; set; } = "monetary";

        // Donation Amount
        [Required(ErrorMessage = "Please enter a donation amount")]
        [Range(10, double.MaxValue, ErrorMessage = "Minimum donation amount is R10")]
        [Display(Name = "Donation Amount")]
        public double Amount { get; set; }

        // Is Recurring Donation (monthly)
        [Display(Name = "Make this a monthly recurring donation")]
        public bool IsRecurring { get; set; } = false;

        // These will be set by the controller
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
