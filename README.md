# LuduvoDotNet

`LuduvoDotNet` is a small .NET wrapper around the Luduvo API.

## Installation

Add the project reference or package reference as appropriate for your solution.

## Usage

```csharp
using LuduvoDotNet;

var client = new Luduvo();
var user = await client.GetUserByIdAsync(2);

Console.WriteLine(user.Username);
Console.WriteLine(user.Avatar.head_color);
```

## JSON mapping

The API returns avatar colors as hex strings:

```json
{
  "avatar": {
    "head_color": "#C8C8C8",
    "torso_color": "#C8C8C8",
    "left_arm_color": "#C8C8C8",
    "right_arm_color": "#C8C8C8",
    "left_leg_color": "#C8C8C8",
    "right_leg_color": "#C8C8C8"
  }
}
```

Those values are deserialized into `System.Drawing.Color` by `HexColorJsonConverter`, which is registered automatically by `Luduvo`.

### Accepted color format

- Read: `#RRGGBB` or `RRGGBB`
- Write: `#RRGGBB`
- Alpha is ignored because the API only uses RGB values

## Notes

- `User` represents the profile returned by `/users/{id}/profile`.
- `Avatar` stores the avatar body-part colors as `Color` values.

