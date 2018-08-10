using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IReportService
    {
        Task<Stream> GetReportStreamAsync(string fileName);

        Task<long> GetReportFileSizeAsync(string fileName);
    }
}
