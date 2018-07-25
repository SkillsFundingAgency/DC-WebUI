using DC.Web.Ui.Base;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using ESFA.DC.DateTime.Provider.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DC.Web.Ui.Controllers
{
    public class ConfirmationController : BaseController
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public ConfirmationController(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}