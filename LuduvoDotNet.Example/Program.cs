using LuduvoDotNet;
using Spectre.Console;

Luduvo luduvo = new();
await DisplayMainMenuAsync();

async Task DisplayMainMenuAsync()
{
    var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
        .Title("Select api")
        .AddChoices("User API", "Quit")
    );

    if (option == "User API")
    {
        await ShowUserApiMenuAsync();
        await DisplayMainMenuAsync();
    }
}

async Task ShowUserApiMenuAsync()
{
    while (true)
    {
        var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select option")
            .AddChoices("Get user by id", "Search users by username","Show recent users", "Back")
        );

        if (option == "Get user by id")
        {
            var result = AnsiConsole.Ask<uint>("User ID:");
            await AnsiConsole.Status().StartAsync("Awaiting API response...", async _ =>
            {
                try
                {
                    var response = await luduvo.GetUserByIdAsync(result);
                    AnsiConsole.WriteLine(response.ToString());
                }
                catch (UserNotFoundException)
                {
                    AnsiConsole.MarkupLine("[red]User was not found.[/]");
                }
                catch (TooManyRequestsException)
                {
                    AnsiConsole.MarkupLine("[yellow]Rate limit reached. Please try again in a moment.[/]");
                }
                catch (HttpRequestException ex)
                {
                    AnsiConsole.MarkupLine($"[red]Network/API error: {ex.Message}[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Unexpected error: {ex.Message}[/]");
                }
            });
        }
        else if (option == "Search users by username")
        {
            var result =AnsiConsole.Ask<string?>("Username:");
            await AnsiConsole.Status().StartAsync("Awaiting API response...", async _ =>
            {
                try
                {
                    var response = await luduvo.SearchUsersAsync(result);
                    foreach (var partialUser in response)
                    {
                        AnsiConsole.WriteLine(partialUser.ToString());
                    }
                }
                catch (TooManyRequestsException)
                {
                    AnsiConsole.MarkupLine("[yellow]Rate limit reached. Please try again in a moment.[/]");
                }
                catch (HttpRequestException ex)
                {
                    AnsiConsole.MarkupLine($"[red]Network/API error: {ex.Message}[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Unexpected error: {ex.Message}[/]");
                }
            });
        }
        else if (option == "Show recent users")
        {
            await AnsiConsole.Status().StartAsync("Awaiting API response...", async _ =>
            {
                try
                {
                    var response = await luduvo.SearchUsersAsync(null,100,0);
                    foreach (var partialUser in response)
                    {
                        AnsiConsole.WriteLine(partialUser.ToString());
                    }
                }
                catch (TooManyRequestsException)
                {
                    AnsiConsole.MarkupLine("[yellow]Rate limit reached. Please try again in a moment.[/]");
                }
                catch (HttpRequestException ex)
                {
                    AnsiConsole.MarkupLine($"[red]Network/API error: {ex.Message}[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Unexpected error: {ex.Message}[/]");
                }
            });
        }
        else if (option == "Back")
        {
            return;
        }
    }
}
