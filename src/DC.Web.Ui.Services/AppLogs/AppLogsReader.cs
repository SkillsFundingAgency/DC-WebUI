﻿using System.Collections.Generic;
using System.Linq;
using DC.Web.Ui.Services.ViewModels;

namespace DC.Web.Ui.Services.AppLogs
{
    public class AppLogsReader : IAppLogsReader
    {
        private readonly AppLogsContext _context;

        // for testing
        public AppLogsReader()
        {
        }

        public AppLogsReader(AppLogsContext appLogsContext)
        {
            _context = appLogsContext;
        }

        public IEnumerable<AppLogViewModel> GetApplicationLogs(long jobId)
        {
            return _context.Logs.Where(x => x.JobId == jobId.ToString()).OrderByDescending(x => x.TimeStampUtc);
        }
    }
}