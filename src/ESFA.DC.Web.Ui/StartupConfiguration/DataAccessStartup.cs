using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.ClaimTypes;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Settings.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DC.Web.Ui.StartupConfiguration
{
    public static class DataAccessStartup
    {

        public static void AddAndConfigureDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionStrings = configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
            services.AddDbContext<AppLogsContext>(options => options.UseSqlServer(connectionStrings.AppLogs));

        }

    }
}
