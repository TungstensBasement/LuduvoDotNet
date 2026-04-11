using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuduvoDotNet;

/// <summary>
/// Converts API hex color strings to <see cref="Color"/> and back.
/// The API format is <c>#RRGGBB</c>; alpha is not included when writing.
/// </summary>
public class HexColorJsonConverter:JsonConverter<Color>
{
    /// <summary>
    /// Reads a hex color string (for example <c>"#C8C8C8"</c>) into a <see cref="Color"/>.
    /// Both <c>#RRGGBB</c> and <c>RRGGBB</c> are accepted.
    /// </summary>
    /// <param name="reader">JSON reader positioned on a string value.</param>
    /// <param name="typeToConvert">Target type requested by the serializer.</param>
    /// <param name="options">Serializer options in use.</param>
    /// <returns>The parsed <see cref="Color"/> instance.</returns>
    /// <exception cref="JsonException">Thrown when the value is missing or not a 6-digit hex color.</exception>
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

    /// <summary>
    /// Writes a <see cref="Color"/> as a hex string in <c>#RRGGBB</c> format.
    /// </summary>
    /// <param name="writer">JSON writer used to emit the string.</param>
    /// <param name="value">Color value to serialize.</param>
    /// <param name="options">Serializer options in use.</param>
    /// <remarks>The alpha channel is ignored because the API only uses RGB values.</remarks>
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {        
        var hex = $"#{value.R:X2}{value.G:X2}{value.B:X2}";
        writer.WriteStringValue(hex);
    }
}