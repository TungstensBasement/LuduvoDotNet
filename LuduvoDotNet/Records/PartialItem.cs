namespace LuduvoDotNet.Records;

public record PartialItem
    (
        uint ItemId,
        string Name,
        string category_slug,
        Uri? ThumbnailUrl
    );