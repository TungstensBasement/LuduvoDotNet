using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuduvoDotNet.Records;

/// <summary>
/// Payload for updating the authenticated user's profile through <c>PUT /me/profile</c>.
/// </summary>
public sealed class UpdateMyProfileRequest
{
    /// <summary>
    /// User status text.
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// User bio text.
    /// </summary>
    public string? Bio { get; init; }

    /// <summary>
    /// Public display name.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Profile accent color.
    /// </summary>
    public Color? AccentColor { get; init; }

    /// <summary>
    /// Whether other users are allowed to join.
    /// </summary>
    public bool? AllowJoins { get; init; }

    /// <summary>
    /// Allows sending additional JSON fields that may be accepted by the API.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; init; }
}

