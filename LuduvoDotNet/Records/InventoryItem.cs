namespace LuduvoDotNet.Records;

public record InventoryItem
    (
        uint Id,
        uint ItemId,
        Uri? ThumbnailUrl,
        uint Price,
        uint CategoryId,
        string CategoryName,
        string CategorySlug,
        DateTime AcquiredAt
    );