using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Rinoleggio.Leads;
using Rinoleggio.Listings;

namespace Rinoleggio.Leads;

public class LeadDto
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public string? ListingTitle { get; set; }
    public ListingBusinessStatus ListingBusinessStatus { get; set; }
    public Guid BuyerId { get; set; }
    public string? BuyerUserName { get; set; }
    public string? BuyerVisibleName { get; set; }
    public string? BuyerEmail { get; set; }
    public Guid SellerId { get; set; }
    public string? SellerVisibleName { get; set; }
    public string? SellerEmail { get; set; }
    public string? Title { get; set; }
    public string Message { get; set; } = null!;
    public string? Phone { get; set; }
    public LeadStatus Status { get; set; }
    public string? BackofficeNotes { get; set; }
    public DateTime CreationTime { get; set; }
}

public class CreateLeadDto
{
    [Required]
    public Guid ListingId { get; set; }

    [Required]
    [StringLength(LeadConsts.MaxMessageLength, MinimumLength = LeadConsts.MinMessageLength)]
    public string Message { get; set; } = null!;

    [StringLength(LeadConsts.MaxPhoneLength)]
    public string? Phone { get; set; }

    public bool PrivacyConsent { get; set; }
}

public class UpdateLeadStatusDto
{
    [Required]
    public LeadStatus Status { get; set; }

    public string? BackofficeNotes { get; set; }
}

public class GetLeadListInput : PagedAndSortedResultRequestDto
{
    public bool AsBackOffice { get; set; }
    public Guid? ListingId { get; set; }
}
