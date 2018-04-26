using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DC.Web.Ui.Services.JobQueue
{
    public interface IJobQueueService
    {
        Task<long> AddJobAsync(Job job);
    }
}
