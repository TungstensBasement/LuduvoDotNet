namespace LuduvoDotNet;

/// <summary>
/// Represents a user profile returned by the Luduvo API.
/// </summary>
public record User
    (
        uint UserId,
        string Username,
        ulong MemberSince,
        string DisplayName,
        string? Status,
        string? Bio,
        Avatar Avatar,
        List<Item> EquippedItems,
        List<Badge> Badges,
        ushort FriendCount,
        ushort PlaceCount,
        ushort ItemCount,
        ulong LastActive,
        bool AllowJoins,
        bool IsOwner
    );