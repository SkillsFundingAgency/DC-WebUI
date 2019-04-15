using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Ui.ViewModels.HelpDesk
{
    public class ProviderCollectionsViewModel
    {
        public ProviderCollectionsViewModel()
        {
            History = new ProviderHistoryViewModel();
        }
        public long Ukprn { get; set; }

        public int Upin { get; set; }

        public string Name { get; set; }

        public List<SubmissionOptionViewModel> SubmissionOptionViewModels { get; set; }

        public ProviderHistoryViewModel History { get; set; }
    }
}
