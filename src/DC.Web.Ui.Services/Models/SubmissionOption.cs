using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DC.Web.Ui.Services.Models
{
    public class SubmissionOption
    {
        [Required]
        public string Name { get; set; }

        public string Title { get; set; }
    }
}
