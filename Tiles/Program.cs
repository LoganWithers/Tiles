namespace Tiles
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using IO;

    using Models;

    using Newtonsoft.Json;

    using V2;
    using V2.Read;
    using V2.Seed;
    using V2.Write;

    using RightWall = V2.Seed.RightWall;
    using Writer = V2.Write.Writer;


    internal class Program
    {

        public static void Main(string[] args)
        {
            const string stoppingValue = "1654545";

            var settings = new CounterSettings(3, int.Parse(stoppingValue));
            CreateTiles.Write(settings);
  
        }

    }

    public static class CreateTiles
    {
        public static void Write(CounterSettings settings)
        {
            var tiles = new HashSet<Tile>();

            Console.WriteLine(JsonConvert.SerializeObject(settings, Formatting.Indented));

            var seed = new Seed(settings);
            tiles.UnionWith(seed.Tiles);

            var rightWall = new RightWall(settings);
            tiles.UnionWith(rightWall.Tiles);

            var rightPreReaderRightCarry   = new PreReaderRight(settings, true);
            tiles.UnionWith(rightPreReaderRightCarry.Tiles);

            var rightPreReaderRightNoCarry = new PreReaderRight(settings, false);
            tiles.UnionWith(rightPreReaderRightNoCarry.Tiles);


            var tileSetName = $"test";
            var path = Utils.GetPath();

            var readerTiles = AddReaders(settings.BinaryDigitPatterns);
            tiles.UnionWith(readerTiles);

            var hookTiles = AddHooks(settings.BinaryDigitPatterns);
            tiles.UnionWith(hookTiles);

            var writerTiles = AddWriters(settings.BinaryDigitPatterns);
            tiles.UnionWith(writerTiles);
            tiles.UnionWith(GetAllGadgets());

            var options = new TdpOptions(tileSetName, seed.Start.Name);
            WriteOptions($"{path}{tileSetName}.tdp", options);

            WriteDefinitions($"{path}{tileSetName}.tds", tiles);
        }

        private static IEnumerable<Tile> AddHooks(IEnumerable<string> binaryStrings)
        {
            var results = new List<Tile>();

            IEnumerable<string> encodedDigits = binaryStrings.ToList();

            foreach (var shouldCarry in new[] { true, false })
            {
                foreach (var binaryString in encodedDigits)
                {
                    var hook = new Hook(binaryString, binaryString.Length * 4, shouldCarry);

                    results.AddRange(hook.Tiles());
                }
            }

            return results;
        }

        private static IEnumerable<Tile> AddReaders(IEnumerable<string> binaryStrings)
        {
            var results = new List<Tile>();

            IEnumerable<string> encodedDigits = binaryStrings.ToList();

            foreach (var signal in new[] {Signals.Carry, Signals.NoCarry})
            {
                foreach (var binaryString in encodedDigits)
                {
                    for (var i = 0; i < binaryString.Length; i++)
                    {
                        var information = binaryString.Substring(0, i);

                        var reader = new Reader(information, signal, binaryString.Length);

                        results.AddRange(reader.Tiles());
                    }
                }
            }
            return results;
        }


        private static IEnumerable<Tile> AddWriters(IEnumerable<string> binaryStrings)
        {
            var results = new List<Tile>();

            IEnumerable<string> encodedDigits = binaryStrings.ToList();

            foreach (var signal in new[] { Signals.Carry, Signals.NoCarry })
            {
                foreach (var binaryString in encodedDigits)
                {
                   

                        var reader = new Writer(binaryString, signal);

                        results.AddRange(reader.Tiles());
                    
                }
            }
            return results;
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


        private static IEnumerable<Tile> GetAllGadgets()
        {
            var copyStopperCarry   = new CopyStopper(Signals.Carry);
            var copyStopperNoCarry = new CopyStopper(Signals.NoCarry);

            
            return copyStopperNoCarry.Tiles().Concat(copyStopperCarry.Tiles());
        }
    }
}
