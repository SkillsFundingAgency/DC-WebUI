using System.Collections.Generic;
using DC.Web.Ui.Services.ViewModels;

namespace DC.Web.Ui.Services.AppLogs
{
    public interface IAppLogsReader
    {
        IEnumerable<AppLogViewModel> GetApplicationLogs(long jobId);
    }
}
