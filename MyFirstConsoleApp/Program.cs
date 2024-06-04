// This is for learning purpose

Console.WriteLine("Welcome to my program using dotnet");

Console.WriteLine("First, let me ask your name...");
var yourName = Console.ReadLine();
var currentDate = DateTime.Now;

Console.WriteLine($"{Environment.NewLine}Hello, {yourName}, and right now is at {currentDate}");
Console.Write($"{Environment.NewLine}If you want to exit, just press any key..");

Console.ReadKey(true);