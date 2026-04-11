# LuduvoDotNet

`LuduvoDotNet` is a modern .NET wrapper around the Luduvo API.

## Installation

You can install the package via NuGet:

```bash
dotnet add package LuduvoDotNet
```

## Usage

```csharp
using LuduvoDotNet;

var client = new Luduvo();
var user = await client.GetUserByIdAsync(2);

Console.WriteLine(user.Username);
Console.WriteLine(user.Avatar.head_color);
```
