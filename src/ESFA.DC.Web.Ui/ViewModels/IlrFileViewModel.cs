using System;
using System.ComponentModel.DataAnnotations;

namespace DC.Web.Ui.ViewModels
{
    public class IlrFileViewModel
    {
        public string Filename { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm:ss.fff tt}")]
        public DateTime SubmissionDateTime { get; set; }

        public decimal FileSize { get; set; }

        public long JobId { get; set; }
    }
}
