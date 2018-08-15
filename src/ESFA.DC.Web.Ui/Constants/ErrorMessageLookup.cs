using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DC.Web.Ui.Constants
{
    public static class ErrorMessageLookup
    {
        private static readonly IReadOnlyDictionary<string, string> MessagesDictionary =
            new Dictionary<string, string>()
            {
                { ErrorMessageKeys.SubmissionOptions_OptionsFieldKey, "Choose an option from the list" }
            };

        public static string GetErrorMessage(string key)
        {
            return MessagesDictionary.ContainsKey(key) ? MessagesDictionary[key] : string.Empty;
        }
    }
}
