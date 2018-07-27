using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DC.Web.Ui.Services.Models;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface ICollectionManagementService
    {
        Task<IEnumerable<SubmissionOption>> GetSubmssionOptions(long ukprn);

        Task<ReturnPeriod> GetPeriod(string collectionName, DateTime dateTimeUtc);
    }
}
