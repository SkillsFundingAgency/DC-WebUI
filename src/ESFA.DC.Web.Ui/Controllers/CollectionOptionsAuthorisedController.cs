using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.ILR.Controllers
{
    public class CollectionOptionsAuthorisedController : BaseAuthorisedController
    {
        private readonly ICollectionManagementService _collectionManagementService;

        public CollectionOptionsAuthorisedController(ICollectionManagementService collectionManagementService, ILogger logger)
            : base(logger)
        {
            _collectionManagementService = collectionManagementService;
        }

        public async Task<IActionResult> Index(string collectionType)
        {
            Logger.LogInfo($"Ukprn : {User.Ukprn()},page load with colletion type : {collectionType} ");

            var data = await _collectionManagementService.GetAvailableCollectionsAsync(User.Ukprn(), collectionType);

            if (data.Any())
            {
                Logger.LogInfo($"Ukprn : {User.Ukprn()}, returned {data.Count()} available collections");

                //if there is only one then redirect to submission page
                if (data.Count() == 1)
                {
                   return RedirectToAction("Index", "SubmissionAuthorised", new { area= collectionType.ToLower(), data.First().CollectionName });
                }

                return View(data);
            }

            return RedirectToAction("Index", "ReturnWindowClosedAuthorised", new { area = collectionType.ToLower() });
        }
    }
}