using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Users;

namespace Rinoleggio.Web.Components.LayoutHooks;

public class HtmlShellClassViewComponent : AbpViewComponent
{
    private readonly ICurrentUser _currentUser;

    public HtmlShellClassViewComponent(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public IViewComponentResult Invoke()
    {
        return View("~/Components/LayoutHooks/Default.cshtml", _currentUser.IsAuthenticated);
    }
}
