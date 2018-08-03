using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface ICollectionManagementService
    {
        Task<IEnumerable<SubmissionOptionViewModel>> GetSubmssionOptions(long ukprn);

        Task<ReturnPeriodViewModel> GetCurrentPeriod(string collectionName);

        Task<IEnumerable<CollectionViewModel>> GetAvailableCollections(long ukprn, string collectionType);

        Task<bool> IsValidCollection(long ukprn, string collectionType);
    }
}
