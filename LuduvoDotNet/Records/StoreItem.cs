namespace LuduvoDotNet.Records;

/// <summary>
/// Represents an item returned by store endpoints.
/// </summary>
public record StoreItem
(
    uint Id,
    uint CreatorId,
    string CreatorUsername,
    uint CategoryId,
    string CategoryName,
    string CategorySlug,
    string Name,
    string Description,
    uint Price,
    int IsLimited,
    string AssetType,
    Uri? ThumbnailUrl,
    uint SalesCount,
    uint ThumbsUp,
    uint ThumbsDown,
    DateTime CreatedAt,
    bool Owned,
    int? IsOnSale = null,
    int? Stock = null,
    string? Status = null,
    string? RigStructuralHash = null,
    Uri? ModelUrl = null,
    DateTime? UpdatedAt = null
);

