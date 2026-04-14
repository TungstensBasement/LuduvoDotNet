namespace LuduvoDotNet.Records;
public record Place
    (
        uint id,
        uint owner_id,
        string owner_username,
        string title,
        string description,
        string access,
        ushort max_Players,
        ulong visit_count,
        uint thumbs_up,
        uint thumbs_down,
        uint active_players,
        DateTime created_at,
        DateTime updated_at,
        Uri? thumbnail_url,
        bool has_map,
        Tag[] tags,
        bool is_owner
    );