using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DC.Web.Ui.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetConfigSection<T>(this IConfiguration configuration)
        {
            return  configuration.GetSection(typeof(T).Name).Get<T>();
        }

        public static T GetConfigSection<T>(this IConfiguration configuration,string sectionName)
        {
            return configuration.GetSection(sectionName).Get<T>();
        }
    }
}
