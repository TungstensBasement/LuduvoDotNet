using System;

namespace LuduvoDotNet.Records;

/// <summary>
/// Represents a user profile returned by the Luduvo API.
/// </summary>
public record User
    (
        uint UserId,
        string Username,
        DateTime MemberSince,
        string DisplayName,
        string? Status,
        string? Bio,
        Avatar Avatar,
        List<Item> EquippedItems,
        List<Badge> Badges,
        ushort FriendCount,
        ushort PlaceCount,
        ushort ItemCount,
        DateTime? LastActive,
        bool AllowJoins,
        bool IsOwner
    );