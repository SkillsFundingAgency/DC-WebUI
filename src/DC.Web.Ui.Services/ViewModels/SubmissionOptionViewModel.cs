using System.ComponentModel.DataAnnotations;

namespace DC.Web.Ui.Services.ViewModels
{
    public class SubmissionOptionViewModel
    {
        [Required]
        public string Name { get; set; }

        public string Title { get; set; }
    }
}
