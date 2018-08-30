using System.Collections.Generic;
using ESFA.DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Services.AppLogs
{
    public interface IAppLogsReader
    {
        IEnumerable<AppLogViewModel> GetApplicationLogs(long jobId);
    }
}
