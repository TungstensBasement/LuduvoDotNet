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

## Disclaimer
This library is an unofficial wrapper around the Luduvo API. It is not affiliated with or endorsed by Luduvo.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.