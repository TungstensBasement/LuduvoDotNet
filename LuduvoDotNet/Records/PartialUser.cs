using System.Drawing;

namespace LuduvoDotNet.Records
{
    /// <summary>
    /// Lightweight user data returned by user search endpoints.
    /// </summary>
    /// <param name="id">Unique user identifier.</param>
    /// <param name="username">Account username.</param>
    /// <param name="head_color">Avatar head color.</param>
    /// <param name="torso_color">Avatar torso color.</param>
    /// <param name="display_name">Public display name.</param>
    /// <param name="role">User role label returned by the API.</param>
    /// <param name="created_at">Unix timestamp indicating when the account was created.</param>
    public record PartialUser
    (
        uint id,
        string username,
        Color head_color,
        Color torso_color,
        string display_name,
        string role,
        ulong created_at
    )
    {
        /// <summary>
        /// Retrieves the full profile for this partial user entry.
        /// </summary>
        /// <returns>A full <see cref="User"/> profile.</returns>
        public Task<User> GetUserAsync()
        {
            var client = new Luduvo();
            return client.GetUserByIdAsync(id);
        }
    }
}
