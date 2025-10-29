using Microsoft.AspNetCore.Mvc;
using PatientPortal.Extensions;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.ViewComponents
{
    public class InboxIconViewComponent : ViewComponent
    {
        private readonly IMessagingService _messagingService;

        public InboxIconViewComponent(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public IViewComponentResult Invoke()
        {
            int? linkId = UserClaimsPrincipal.GetMessageLinkId();
            if (!linkId.HasValue)
                return View(0);

            MessagingLink? link = _messagingService.GetMessagingLink(linkId.Value);
            int count = link != null ? _messagingService.GetUnreadTotalCount(link) : 0;

            return View(count);
        }
    }
}
