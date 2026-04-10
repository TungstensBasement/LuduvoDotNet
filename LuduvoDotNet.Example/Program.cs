namespace LuduvoDotNet.Example;
using LuduvoDotNet;
class Program
{
    
    static void Main(string[] args)
    {
        Luduvo luduvo=new();
        var result=luduvo.GetUserByIdAsync(2).Result;
        Console.WriteLine(result.ToString());
    }
}