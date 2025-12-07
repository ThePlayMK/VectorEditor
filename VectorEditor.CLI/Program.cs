using VectorEditor.Core;

var service = new CalculatorService();

Console.WriteLine("--- Console Application ---");
Console.WriteLine(service.GetGreeting());

int result = service.Add(10, 50);
Console.WriteLine($"Result from Core Logic: {result}");