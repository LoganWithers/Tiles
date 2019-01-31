namespace Tiles
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Models;

    using Newtonsoft.Json;


    internal class Program
    {

        public static void Main(string[] args)
        {
            var input = " ";

            while (input != "-e" && !string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Base?");

                input =  Console.ReadLine();
                //input = "4";

                if (!int.TryParse(input, out var @base))
                {
                    continue;
                }

                Console.WriteLine("Stopping value?");
                //input = "15";

                input = Console.ReadLine();

                var stoppingValue = int.Parse(input);

                var settings = new CounterSettings(@base, stoppingValue);
                CreateTiles.Write(settings);
            }

            Console.WriteLine("exiting...");
            // LeftWall LeftSide 9  => east = 9 -> 10 818bd6af-f2dd-44b9-ad78-31960ae929ac
            // LeftWall RightSide 9 => west =
        }

    }

    public static class CreateTiles
    {
        public static void Write(CounterSettings settings)
        {
            var digits = new List<Bit>();

            foreach (var binaryValue in settings.BinaryDigitPatterns)
            {
                digits.AddRange(BitStringEncoder.Encode(binaryValue));
            }

            var tiles = new List<Tile>();
            Console.WriteLine(JsonConvert.SerializeObject(settings, Formatting.Indented));

            var seedTiles = new SeedRow(settings).Tiles();
            var rightWall = new RightWall(settings).Tiles();

            tiles.AddRange(seedTiles);
            tiles.AddRange(rightWall);


            var tileSetName = $"base-{settings.BaseK}-counter";
            var options = new TdpOptions(tileSetName, "LeftWallLeftSide");


            var path = Utils.GetPath();

            WriteOptions($"{path}{tileSetName}.tdp", options);

            WriteDefinitions($"{path}{tileSetName}.tds", tiles);
        }

        private static void WriteOptions(string path, TdpOptions options)
        {
            File.WriteAllText(path, options.ToString());
            Console.WriteLine($".tdp file can be found at {path}");
        }

        private static void WriteDefinitions(string path, IEnumerable<Tile> tiles)
        {
            File.WriteAllText(path, string.Join("\n", tiles.Select(t => t.ToString())));
            Console.WriteLine($".tds file can be found at {path}");
        }
    }
}
