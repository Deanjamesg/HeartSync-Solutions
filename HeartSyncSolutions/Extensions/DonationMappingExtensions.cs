using HeartSyncSolutions.Models;
using HeartSyncSolutions.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace HeartSyncSolutions.Extensions
{
    /// <summary>
    /// Extension methods for mapping between donation models and view models
    /// </summary>
    public static class DonationMappingExtensions
    {
        // Monetary Donation mappings
        public static MonetaryDonationViewModel ToViewModel(this MonetaryDonations model)
        {
            if (model == null) return null;

            return new MonetaryDonationViewModel
            {
                MonetaryDonationId = model.MonetaryDonationID,
                DonationAmount = model.DonationAmount,
                Date = model.Date,
                IsAnonymous = model.IsAnonymous,
                ApplicationUserId = model.ApplicationUserID,
                DonorName = model.IsAnonymous ? "Anonymous" : 
                    (model.ApplicationUser != null ? $"{model.ApplicationUser.FirstName} {model.ApplicationUser.LastName}" : "Guest"),
                DonorEmail = model.ApplicationUser?.Email
            };
        }

        public static MonetaryDonations ToModel(this MonetaryDonationViewModel viewModel)
        {
            if (viewModel == null) return null;

            return new MonetaryDonations
            {
                MonetaryDonationID = viewModel.MonetaryDonationId,
                DonationAmount = viewModel.DonationAmount,
                Date = viewModel.Date,
                IsAnonymous = viewModel.IsAnonymous,
                ApplicationUserID = viewModel.ApplicationUserId
            };
        }

        // In-Kind Donation mappings
        public static InKindDonationViewModel ToViewModel(this InKindOffer model)
        {
            if (model == null) return null;

            return new InKindDonationViewModel
            {
                InKindOfferId = model.InKindOfferID,
                ItemDescription = model.ItemDescription,
                Status = model.Status,
                OfferedDate = model.OfferedDate,
                ApplicationUserId = model.ApplicationUserID,
                DonorName = model.ApplicationUser != null ? 
                    $"{model.ApplicationUser.FirstName} {model.ApplicationUser.LastName}" : "Guest",
                ContactEmail = model.ApplicationUser?.Email,
                ContactPhone = model.ApplicationUser?.ContactNumber
            };
        }

        public static InKindOffer ToModel(this InKindDonationViewModel viewModel)
        {
            if (viewModel == null) return null;

            return new InKindOffer
            {
                InKindOfferID = viewModel.InKindOfferId,
                ItemDescription = viewModel.ItemDescription,
                Status = viewModel.Status,
                OfferedDate = viewModel.OfferedDate,
                ApplicationUserID = viewModel.ApplicationUserId
            };
        }

        // Collection mappings
        public static IEnumerable<MonetaryDonationViewModel> ToViewModelList(this IEnumerable<MonetaryDonations> models)
        {
            return models?.Select(m => m.ToViewModel()) ?? new List<MonetaryDonationViewModel>();
        }

        public static IEnumerable<InKindDonationViewModel> ToViewModelList(this IEnumerable<InKindOffer> models)
        {
            return models?.Select(m => m.ToViewModel()) ?? new List<InKindDonationViewModel>();
        }
    }
}