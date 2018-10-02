using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class FileUploadConfirmationViewModel
    {
        public long JobId { get; set; }

        public string FileName { get; set; }

        public string SubmittedBy { get; set; }

        public string SubmittedAt { get; set; }

        public string PeriodName { get; set; }

        public string HeaderMessage { get; set; }

        public JobType JobType { get; set; }

    }
}
