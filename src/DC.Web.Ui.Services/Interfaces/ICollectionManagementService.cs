using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface ICollectionManagementService
    {
        Task<IEnumerable<SubmissionOptionViewModel>> GetSubmssionOptionsAsync(long ukprn);

        Task<ReturnPeriodViewModel> GetCurrentPeriodAsync(string collectionName);

        Task<IEnumerable<CollectionViewModel>> GetAvailableCollectionsAsync(long ukprn, string collectionType);

        Task<bool> IsValidCollectionAsync(long ukprn, string collectionType);

        Task<ReturnPeriodViewModel> GetNextPeriodAsync(string collectionName);

        Task<Collection> GetCollectionAsync(long ukprn, string collectionName);
    }
}
