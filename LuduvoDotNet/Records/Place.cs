namespace LuduvoDotNet.Records;
public record Place
    (
        uint Id,
        uint OwnerId,
        string OwnerUsername,
        string Title,
        string Description,
        string Access,
        ushort MaxPlayers,
        ulong VisitCount,
        uint ThumbsUp,
        uint ThumbsDown,
        uint ActivePlayers,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        Uri? ThumbnailUrl,
        bool HasMap,
        Tag[] Tags,
        bool IsOwner
    );