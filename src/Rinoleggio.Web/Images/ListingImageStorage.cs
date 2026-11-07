using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Volo.Abp.BlobStoring;
using Rinoleggio.Listings;

namespace Rinoleggio.Web.Images;

public sealed record PreparedListingImage(byte[] Content, string Extension, string ContentType)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Url(Guid listingId) => $"/Listings/Image/{listingId:D}/{Id:N}.{Extension}";
    public string BlobName(Guid listingId) => $"{listingId:N}/{Id:N}.{Extension}";
}

public class ListingImageStorage
{
    public const int MaxFileBytes = 4 * 1024 * 1024;
    public const int MaxNewImages = 5;
    public const int MaxImagesPerListing = 10;
    private readonly IBlobContainer<ListingImageContainer> _container;

    public ListingImageStorage(IBlobContainer<ListingImageContainer> container) => _container = container;

    public async Task<List<PreparedListingImage>> PrepareAsync(IReadOnlyList<IFormFile> files)
    {
        var selected = files.Where(x => x is { Length: > 0 }).ToList();
        if (selected.Count > MaxNewImages) throw new InvalidOperationException($"Puoi caricare al massimo {MaxNewImages} foto per volta.");
        var result = new List<PreparedListingImage>();
        foreach (var file in selected)
        {
            if (file.Length > MaxFileBytes) throw new InvalidOperationException("Ogni immagine deve pesare al massimo 4 MB.");
            await using var stream = file.OpenReadStream();
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            var bytes = memory.ToArray();
            var (extension, contentType) = Detect(bytes);
            result.Add(new PreparedListingImage(bytes, extension, contentType));
        }
        return result;
    }

    private static (string Extension, string ContentType) Detect(byte[] bytes)
    {
        if (bytes.Length >= 3 && bytes[0] == 0xff && bytes[1] == 0xd8 && bytes[2] == 0xff) return ("jpg", "image/jpeg");
        if (bytes.Length >= 8 && bytes.Take(8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })) return ("png", "image/png");
        if (bytes.Length >= 12 && bytes[0] == 'R' && bytes[1] == 'I' && bytes[2] == 'F' && bytes[3] == 'F' && bytes[8] == 'W' && bytes[9] == 'E' && bytes[10] == 'B' && bytes[11] == 'P') return ("webp", "image/webp");
        throw new InvalidOperationException("Formato non supportato. Carica immagini JPEG, PNG o WebP.");
    }

    public async Task StoreAsync(Guid listingId, IEnumerable<PreparedListingImage> images)
    {
        foreach (var image in images) await _container.SaveAsync(image.BlobName(listingId), new MemoryStream(image.Content), overrideExisting: false);
    }

    public async Task DeleteAsync(Guid listingId, IEnumerable<string> urls)
    {
        foreach (var name in urls.Select(url => TryGetBlobName(listingId, url)).Where(name => name != null).Distinct())
            await _container.DeleteAsync(name!);
    }

    public static string? TryGetBlobName(Guid listingId, string url)
    {
        var prefix = $"/Listings/Image/{listingId:D}/";
        if (!url.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return null;
        var file = url[prefix.Length..];
        if (file.Contains('/') || file.Contains('\\') || file.Contains("..", StringComparison.Ordinal)) return null;
        var extension = Path.GetExtension(file).ToLowerInvariant();
        return extension is ".jpg" or ".png" or ".webp" ? $"{listingId:N}/{file}" : null;
    }
}
