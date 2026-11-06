using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rinoleggio.Leads;

namespace Rinoleggio.Web.Pages.Leads;

public static class LeadFormHelper
{
    public static IReadOnlyList<(LeadStatus Value, string Label)> Statuses { get; } =
    [
        (LeadStatus.New, "Nuovo"),
        (LeadStatus.Contacted, "Contattato"),
        (LeadStatus.Closed, "Chiuso")
    ];

    public static string GetStatusLabel(LeadStatus status)
    {
        foreach (var item in Statuses)
        {
            if (item.Value == status)
            {
                return item.Label;
            }
        }

        return status.ToString();
    }

    public static IEnumerable<SelectListItem> GetStatusItems()
    {
        foreach (var item in Statuses)
        {
            yield return new SelectListItem(item.Label, ((int)item.Value).ToString());
        }
    }
}
