using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DC.Web.Ui.ViewModels
{
    public class InputFileViewModel
    {
        public IFormFile File { get; set; }
    }
}
