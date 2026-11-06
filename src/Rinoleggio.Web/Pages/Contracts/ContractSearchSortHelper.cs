using System.Collections.Generic;

namespace Rinoleggio.Web.Pages.Contracts;

public static class ContractSearchSortHelper
{
    public static IReadOnlyList<(string Value, string Label)> SortOptions { get; } =
    [
        ("", "Scadenza (predefinito), poi canone"),
        ("EndDate asc", "Scadenza — prima le più vicine"),
        ("EndDate desc", "Scadenza — prima le più lontane"),
        ("MonthlyFee asc", "Canone — dal più basso"),
        ("MonthlyFee desc", "Canone — dal più alto"),
        ("RemainingKm desc", "Km residui — dal più alto"),
        ("RemainingKm asc", "Km residui — dal più basso"),
        ("RegistrationYear desc", "Anno immatricolazione — più recenti"),
        ("Title asc", "Titolo A–Z")
    ];
}
