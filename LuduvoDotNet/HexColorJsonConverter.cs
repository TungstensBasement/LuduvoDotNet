using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuduvoDotNet;


internal class HexColorJsonConverter:JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var hex = reader.GetString();
        if (string.IsNullOrEmpty(hex))
        {throw new JsonException("Invalid color format");}
        hex=hex.TrimStart('#');
        if (hex.Length != 6)
        {throw new JsonException("Invalid color format");}
        var r = Convert.ToByte(hex.Substring(0, 2), 16);
        var g = Convert.ToByte(hex.Substring(2, 2), 16);
        var b = Convert.ToByte(hex.Substring(4, 2), 16);
        return Color.FromArgb(r, g, b);
    }
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {        
        var hex = $"#{value.R:X2}{value.G:X2}{value.B:X2}";
        writer.WriteStringValue(hex);
    }
}