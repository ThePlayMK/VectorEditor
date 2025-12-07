namespace VectorEditor.Core
{
    public class CalculatorService
    {
        public int Add(int a, int b)
        {
            // This logic is now available to BOTH apps
            return a + b;
        }

        public string GetGreeting()
        {
            return "Hello from the Shared Core Library!";
        }
    }
}