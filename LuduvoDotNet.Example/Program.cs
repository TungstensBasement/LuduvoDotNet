using LuduvoDotNet;
using Spectre.Console;

Luduvo luduvo = new();
displayMainMenu();
void displayMainMenu()
{
    var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
        .Title("Select api")
        .AddChoices("User API", "Quit")
    );
    if (option == "User API") showUserApiMenu();
    else if (option == "Quit") Environment.Exit(0);
}
void showUserApiMenu()
{
    var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
        .Title("Select option")
        .AddChoices("Get user by id", "Search users by username", "Back")
    );
    if(option=="Get user by id")
    {
        var result=AnsiConsole.Ask<uint>("User ID");
        AnsiConsole.Status().Start("Awaiting API response...", async ctx =>
        {
            var response = luduvo.GetUserByIdAsync(result).Result;
            AnsiConsole.WriteLine(response.ToString());
        });
        showUserApiMenu();
    }
    else if(option == "Search users by username")
    {
        throw new NotImplementedException();
    }
    else if(option=="Back")
    {
        displayMainMenu();
    }
}

