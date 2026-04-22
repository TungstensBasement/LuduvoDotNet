using LuduvoDotNet;
using Spectre.Console;

var luduvo = new Luduvo();

while (true)
{
    var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
        .Title("Select API")
        .AddChoices("User API", "Places API", "Store API", "Quit"));

    if (option == "Quit")
        return;

    if (option == "User API")
        await ShowUserApiMenuAsync();
    else if (option == "Places API")
        await ShowPlacesMenuAsync();
    else if (option == "Store API")
        await ShowStoreMenuAsync();
}

async Task ShowUserApiMenuAsync()
{
    while (true)
    {
        var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select option")
            .AddChoices("Get user by id", "Search users by username", "Get user inventory", "Back"));

        if (option == "Back")
            return;

        if (option == "Get user by id")
        {
            var id = AnsiConsole.Ask<uint>("User ID:");
            await RunWithHandlingAsync(async () =>
            {
                var response = await luduvo.GetUserByIdAsync(id);
                AnsiConsole.WriteLine(response.ToString());
            });
        }
        else if (option == "Search users by username")
        {
            var username = AnsiConsole.Ask<string?>("Username:");
            await RunWithHandlingAsync(async () =>
            {
                var response = await luduvo.SearchUsersAsync(username);
                foreach (var partialUser in response)
                    AnsiConsole.WriteLine(partialUser.ToString());
            });
        }
        else if (option == "Get user inventory")
        {
            var id = AnsiConsole.Ask<int>("User ID:");
            var limit = AskOptionalInt("Limit (Enter for none):");
            var offset = AskOptionalInt("Offset (Enter for none):");

            await RunWithHandlingAsync(async () =>
            {
                var response = await luduvo.GetUserInventotoryAsync(id, limit, offset);
                foreach (var inventoryItem in response)
                    AnsiConsole.WriteLine(inventoryItem.ToString());
            });
        }
    }
}

async Task ShowPlacesMenuAsync()
{
    while (true)
    {
        var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select option")
            .AddChoices("Get place by id", "Search places", "Back"));

        if (option == "Back")
            return;

        if (option == "Get place by id")
        {
            var id = AnsiConsole.Ask<uint>("Place ID:");
            await RunWithHandlingAsync(async () =>
            {
                var response = await luduvo.GetPlaceByIdAsync(id);
                AnsiConsole.WriteLine(response.ToString());
            });
        }
        else if (option == "Search places")
        {
            var query = AnsiConsole.Ask<string>("Search places:");
            var limit = AskOptionalInt("Limit (Enter for none):");
            var offset = AskOptionalInt("Offset (Enter for none):");

            await RunWithHandlingAsync(async () =>
            {
                var response = await luduvo.SearchPlacesAsync(query, limit, offset);
                foreach (var place in response)
                    AnsiConsole.WriteLine(place.ToString());
            });
        }
    }
}

async Task ShowStoreMenuAsync()
{
    while (true)
    {
        var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select option")
            .AddChoices("Get store item by id", "Search store", "Back"));

        if (option == "Back")
            return;

        if (option == "Get store item by id")
        {
            var id = AnsiConsole.Ask<uint>("Item ID:");
            await RunWithHandlingAsync(async () =>
            {
                var response = await luduvo.GetStoreItemByIdAsync(id);
                AnsiConsole.WriteLine(response.ToString());
            });
        }
        else if (option == "Search store")
        {
            var query = AnsiConsole.Ask<string?>("Query:");
            var limit = AskOptionalInt("Limit (Enter for none):");
            var offset = AskOptionalInt("Offset (Enter for none):");

            await RunWithHandlingAsync(async () =>
            {
                var response = await luduvo.SearchStoreAsync(query, limit, offset);
                foreach (var storeItem in response)
                    AnsiConsole.WriteLine(storeItem.ToString());
            });
        }
    }
}

static int? AskOptionalInt(string prompt)
{
    var value = AnsiConsole.Ask<string>(prompt);
    if (string.IsNullOrWhiteSpace(value))
        return null;
    return int.Parse(value);
}

static async Task RunWithHandlingAsync(Func<Task> action)
{
    await AnsiConsole.Status().StartAsync("Awaiting API response...", async _ =>
    {
        try
        {
            await action();
        }
        catch (UserNotFoundException)
        {
            AnsiConsole.MarkupLine("[red]User was not found.[/]");
        }
        catch (PlaceNotFoundException)
        {
            AnsiConsole.MarkupLine("[red]Place was not found.[/]");
        }
        catch (StoreItemNotFoundException)
        {
            AnsiConsole.MarkupLine("[red]Store item was not found.[/]");
        }
        catch (TooManyRequestsException)
        {
            AnsiConsole.MarkupLine("[yellow]Rate limit reached. Please try again in a moment.[/]");
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Network/API error: {ex.Message}[/]");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            AnsiConsole.MarkupLine($"[red]Invalid arguments: {ex.Message}[/]");
        }
        catch (FormatException)
        {
            AnsiConsole.MarkupLine("[red]Please enter a valid integer for limit/offset.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Unexpected error: {ex.Message}[/]");
        }
    });
}
