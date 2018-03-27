using System.Collections.Generic;
using DC.Web.Ui.Services.Models;

namespace DC.Web.Ui.Services.AppLogs
{
    public interface IAppLogsReader
    {
        IEnumerable<AppLog> GetApplicationLogs(string correlationId);
    }
}
