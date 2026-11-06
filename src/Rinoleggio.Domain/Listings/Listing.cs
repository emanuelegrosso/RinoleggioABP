using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rinoleggio.Lookups;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Rinoleggio.Listings;

public class Listing : FullAuditedAggregateRoot<Guid>
{
    protected Listing()
    {
    }

    public Listing(Guid id) : base(id)
    {
    }

    public Guid OwnerId { get; set; }

    public string Title { get; set; } = null!;

    public string? DescriptionHtml { get; set; }

    public string SlugPath { get; set; } = null!;

    public Guid ProvinceId { get; set; }

    public TransferContributionType TransferContributionType { get; set; } = TransferContributionType.None;

    public decimal TransferContributionAmount { get; set; }

    public ListingBusinessStatus BusinessStatus { get; set; } = ListingBusinessStatus.Active;

    public string? StatusReason { get; set; }

    public StatusChangedBy? StatusChangedBy { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public Province? Province { get; set; }

    public ListingContract Contract { get; set; } = null!;

    public ListingVehicle Vehicle { get; set; } = null!;

    public ListingServices Services { get; set; } = null!;

    public ICollection<ListingImage> Images { get; set; } = new Collection<ListingImage>();

    public bool CanBeEdited()
    {
        return BusinessStatus is not (ListingBusinessStatus.Closed or ListingBusinessStatus.Deleted);
    }

    public void SaveAsDraft()
    {
        if (BusinessStatus is ListingBusinessStatus.Closed or ListingBusinessStatus.Deleted)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingNotEditable);
        }

        IsPublished = false;
        if (BusinessStatus is ListingBusinessStatus.Withdrawn or ListingBusinessStatus.Reserved)
        {
            BusinessStatus = ListingBusinessStatus.Active;
            StatusReason = null;
            StatusChangedBy = null;
        }
    }

    public void Publish(DateTime now, DateOnly today)
    {
        EnsureEligibleForSearchVisibility(today);
        IsPublished = true;
        PublishedAt = now;
        BusinessStatus = ListingBusinessStatus.Active;
        StatusReason = null;
        StatusChangedBy = null;
    }

    public void UnpublishFromSearch()
    {
        if (BusinessStatus is ListingBusinessStatus.Closed or ListingBusinessStatus.Deleted or ListingBusinessStatus.Suspended)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingNotPublishable);
        }

        IsPublished = false;
    }

    public void PublishToSearch(DateTime now, DateOnly today)
    {
        EnsureEligibleForSearchVisibility(today);
        IsPublished = true;
        PublishedAt ??= now;
    }

    public void RepublishBySeller(DateTime now, DateOnly today)
    {
        if (BusinessStatus != ListingBusinessStatus.Withdrawn)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingCannotRepublish);
        }

        if (StatusChangedBy != Listings.StatusChangedBy.Seller)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingCannotRepublish);
        }

        Publish(now, today);
    }

    private void EnsureEligibleForSearchVisibility(DateOnly today)
    {
        if (BusinessStatus is ListingBusinessStatus.Closed or ListingBusinessStatus.Deleted or ListingBusinessStatus.Suspended)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingNotPublishable);
        }

        if (Contract.EndDate <= today)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingEndDateMustBeFuture);
        }

        ValidateTransferContribution();
    }

    public void Suspend(string reason, StatusChangedBy changedBy)
    {
        Check.NotNullOrWhiteSpace(reason, nameof(reason));
        BusinessStatus = ListingBusinessStatus.Suspended;
        StatusReason = reason;
        StatusChangedBy = changedBy;
        IsPublished = false;
    }

    public void Reactivate()
    {
        if (BusinessStatus != ListingBusinessStatus.Suspended)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingNotPublishable);
        }

        BusinessStatus = ListingBusinessStatus.Active;
        StatusReason = null;
        StatusChangedBy = null;
    }

    public void MarkReserved(StatusChangedBy changedBy)
    {
        if (BusinessStatus != ListingBusinessStatus.Active)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingCannotReserve);
        }

        BusinessStatus = ListingBusinessStatus.Reserved;
        StatusChangedBy = changedBy;
        StatusReason = null;
    }

    public void ClearReservation(StatusChangedBy changedBy)
    {
        if (BusinessStatus != ListingBusinessStatus.Reserved)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingCannotClearReservation);
        }

        BusinessStatus = ListingBusinessStatus.Active;
        StatusChangedBy = changedBy;
        StatusReason = null;
    }

    public void Withdraw(string reason, StatusChangedBy changedBy)
    {
        Check.NotNullOrWhiteSpace(reason, nameof(reason));
        if (BusinessStatus is ListingBusinessStatus.Closed or ListingBusinessStatus.Deleted)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingNotPublishable);
        }

        BusinessStatus = ListingBusinessStatus.Withdrawn;
        StatusReason = reason;
        StatusChangedBy = changedBy;
        IsPublished = false;
    }

    public void Close(StatusChangedBy changedBy, string? reason = null)
    {
        if (BusinessStatus is ListingBusinessStatus.Closed or ListingBusinessStatus.Deleted)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingNotPublishable);
        }

        BusinessStatus = ListingBusinessStatus.Closed;
        StatusReason = reason;
        StatusChangedBy = changedBy;
        IsPublished = false;
    }

    public void MarkDeleted(string reason, StatusChangedBy changedBy)
    {
        Check.NotNullOrWhiteSpace(reason, nameof(reason));
        BusinessStatus = ListingBusinessStatus.Deleted;
        StatusReason = reason;
        StatusChangedBy = changedBy;
        IsPublished = false;
    }

    public void Restore()
    {
        if (BusinessStatus != ListingBusinessStatus.Deleted)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingCannotRestore);
        }

        BusinessStatus = ListingBusinessStatus.Active;
        StatusReason = null;
        StatusChangedBy = null;
        IsPublished = false;
    }

    public void ValidateTransferContribution()
    {
        if (TransferContributionType == TransferContributionType.None)
        {
            if (TransferContributionAmount != 0)
            {
                throw new BusinessException(RinoleggioDomainErrorCodes.ListingInvalidTransferContribution);
            }

            return;
        }

        if (TransferContributionAmount <= 0)
        {
            throw new BusinessException(RinoleggioDomainErrorCodes.ListingInvalidTransferContribution);
        }
    }
}
