namespace Tiles
{

    using System;

    using Models;

    internal class Program
    {

        public static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Base value?");
                var input = Console.ReadLine();

                bool IsExitCommand() => input == "-e" || string.IsNullOrEmpty(input);
                
                if (int.TryParse(input, out var baseK) && baseK >= 2 && baseK <= 36)
                {
                    Console.WriteLine("Stopping value?");
                    input = Console.ReadLine();

                    if (int.TryParse(input, out var stoppingValue))
                    {
                        var settings = new CounterSettings(baseK, stoppingValue);
                        TileGenerator.Write(settings);
                        continue;
                    }

                    if (IsExitCommand())
                    {
                        break;
                    }

                    Error($"Error parsing {input}... make sure the stopping value is a number");
                    continue;
                }

                if (IsExitCommand())
                {
                    break;
                }

                Error($"Error parsing {input}... make sure the base is a number between 2 and 36.");

            }

        }


        private static void Error(string message)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }
    }

}
