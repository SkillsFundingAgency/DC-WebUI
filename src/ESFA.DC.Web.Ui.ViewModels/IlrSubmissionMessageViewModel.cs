using System;

namespace ESFA.DC.Web.Ui.ViewModels
{
    public class IlrSubmissionMessageViewModel
    {
        public string Filename { get; set; }

        public string ContainerReference { get; set; }

        public Guid CorrelationId { get; set; }
    }
}