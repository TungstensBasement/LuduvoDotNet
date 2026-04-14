# LuduvoDotNet

![LuduvoDotNet logo](https://raw.githubusercontent.com/TungstensBasement/LuduvoDotNet/refs/heads/master/wordmark.svg)
[![NuGet](https://img.shields.io/nuget/v/LuduvoDotNet.svg)](https://www.nuget.org/packages/LuduvoDotNet/)
[![Documentation](https://img.shields.io/badge/docs-latest-blue.svg)](https://tungstensbasement.github.io/LuduvoDotNet/)
[![Deploy Docs](https://github.com/TungstensBasement/LuduvoDotNet/actions/workflows/docs.yml/badge.svg)](https://github.com/TungstensBasement/LuduvoDotNet/actions/workflows/docs.yml)
[![Publish NuGet Package](https://github.com/TungstensBasement/LuduvoDotNet/actions/workflows/publish-nuget.yml/badge.svg)](https://github.com/TungstensBasement/LuduvoDotNet/actions/workflows/publish-nuget.yml)


`LuduvoDotNet` is a modern .NET wrapper for the Luduvo API.

> [!CAUTION]
> This library and the Luduvo API are in early development and may introduce breaking changes.s

## Links

- Documentation: https://tungstensbasement.github.io/LuduvoDotNet/
- NuGet package: https://www.nuget.org/packages/LuduvoDotNet/

## Installation

```bash
dotnet add package LuduvoDotNet
```

## Quick start

```csharp
using LuduvoDotNet;

var client = new Luduvo();
var user = await client.GetUserByIdAsync(2);

Console.WriteLine($"User: {user.Username}");
Console.WriteLine($"Head color: {user.Avatar.head_color}");
```

## Disclaimer

This library is an unofficial wrapper around the Luduvo API and is not affiliated with or endorsed by Luduvo.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
