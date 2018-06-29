using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace DC.Web.Ui.Services.JobQueue
{
    public interface IJobQueueService
    {
        Task<long> AddJobAsync(IlrJob job);
    }
}
