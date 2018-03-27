using DC.Web.Ui.Services.Models;
using System.Collections.Generic;
using System.Linq;

namespace DC.Web.Ui.Services.AppLogs
{
    public class AppLogsReader : IAppLogsReader
    {
        private readonly AppLogsContext _context;

        //for testing
        public AppLogsReader()
        {
            
        }
        public AppLogsReader(AppLogsContext appLogsContext)
        {
            _context = appLogsContext;
        }

        public IEnumerable<AppLog> GetApplicationLogs(string correlationId)
        {
            return _context.Logs.Where(x=> x.JobId == correlationId).OrderByDescending(x=> x.TimeStampUtc);
        }
    }
}