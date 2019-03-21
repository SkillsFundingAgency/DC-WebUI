using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Services.Extensions;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Areas.Helpdesk.ViewComponents
{
    public class ReturnWindowViewComponent : ViewComponent
    {
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ReturnWindowViewComponent(ICollectionManagementService collectionManagementService, IDateTimeProvider dateTimeProvider)
        {
            _collectionManagementService = collectionManagementService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var collection = await _collectionManagementService.GetCollectionFromTypeAsync(CollectionTypes.ILR);

            if (collection != null)
            {
                var currentPeriod =
                    await _collectionManagementService.GetPeriodAsync(collection.CollectionTitle, _dateTimeProvider.GetNowUtc());

                if (currentPeriod != null)
                {
                    var returnWindow = new ReturnPeriodViewModel(currentPeriod.PeriodNumber)
                    {
                        DaysToClose = (currentPeriod.EndDateTimeUtc - _dateTimeProvider.GetNowUtc()).Days,
                        PeriodCloseDate = currentPeriod.EndDateTimeUtc.AddMinutes(-5).ToDateTimeDisplayFormat()
                    };

                    return View(returnWindow);
                }
            }

            return View(new ReturnPeriodViewModel(0));
        }
    }
}