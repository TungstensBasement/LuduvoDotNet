using System.Drawing;

namespace LuduvoDotNet;

/// <summary>
/// Avatar appearance returned by the API.
/// Color values are represented as <see cref="Color"/> and are serialized via <see cref="HexColorJsonConverter"/>.
/// </summary>
public record Avatar
(
    Color head_color,
    Color torso_color,
    Color left_arm_color,
    Color right_arm_color,
    Color left_leg_color,
    Color right_leg_color
    );