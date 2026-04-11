using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LuduvoDotNet
{
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
        public Task<User> GetUserAsync()
        {
            var client = new Luduvo();
            return client.GetUserByIdAsync(id);
        }
    }
}
