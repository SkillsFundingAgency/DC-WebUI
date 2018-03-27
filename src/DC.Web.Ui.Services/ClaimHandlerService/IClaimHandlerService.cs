using System.Collections.Generic;
using DC.Web.Ui.Services.Models;

namespace DC.Web.Ui.Services.ClaimHandlerService
{
    public interface IUkprnClaimHandlerService
    {
        bool ClaimAccepted(IEnumerable<IdamsClaim> claims);
    }
}
