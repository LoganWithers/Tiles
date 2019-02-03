namespace Tiles
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Gadgets;
    using Gadgets.Read;
    using Gadgets.Seed;
    using Gadgets.Write;

    using Models;

    public static class TileGenerator
    {
        public static void Write(CounterSettings settings)
        {
            var tiles = new HashSet<Tile>();     

            var seed = new Seed(settings);
            tiles.UnionWith(seed.Tiles);

            var rightWall = new RightWall(settings);
            tiles.UnionWith(rightWall.Tiles);

            var leftWall = new LeftWall(settings);
            tiles.UnionWith(leftWall.Tiles);

            var rightTurn = new RightTurn(settings);
            tiles.UnionWith(rightTurn.Tiles);

            var rightPreReaderRightCarry = new PreReaderRight(settings, true);
            tiles.UnionWith(rightPreReaderRightCarry.Tiles);

            var rightPreReaderRightNoCarry = new PreReaderRight(settings, false);
            tiles.UnionWith(rightPreReaderRightNoCarry.Tiles);

            var leftPreReaderFirst = new PreReaderLeft(settings, true);
            tiles.UnionWith(leftPreReaderFirst.Tiles);


            var leftPreReaderNth = new PreReaderLeft(settings, false);
            tiles.UnionWith(leftPreReaderNth.Tiles);


            var readerTiles = AddReaders(settings.BinaryDigitPatterns, settings.BaseK);
            tiles.UnionWith(readerTiles);

            var hookTiles = AddHooks(settings.BinaryDigitPatterns);
            tiles.UnionWith(hookTiles);

            var writerTiles = AddWriters(settings.BinaryDigitPatterns, Convert.ToInt32(settings.StoppingValue));
            tiles.UnionWith(writerTiles);

            tiles.UnionWith(GetAllGadgets());

            var tileSetName = $"base-{settings.BaseK}-counter-to-{settings.StoppingValue}";
            var options     = new TdpOptions(tileSetName, seed.Start.Name);

            var path = Utils.GetPath();
            tiles = tiles.DistinctBy(t => t.Name).ToHashSet();

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
                    var hook = new RightHook(binaryString, binaryString.Length * 4, shouldCarry);

                    results.AddRange(hook.Tiles());
                }
            }

            var sample   = encodedDigits.First();
            var leftHook = new LeftHook(sample, sample.Length);
            results.AddRange(leftHook.Tiles());

            return results;
        }

        
        private static IEnumerable<Tile> AddReaders(IEnumerable<string> binaryStrings, int @base)
        {
            var results = new List<Tile>();

            var encodedDigits = binaryStrings.ToList();

            foreach (var signal in new[] {Signals.Carry, Signals.NoCarry})
            {
                foreach (var binaryString in encodedDigits)
                {
                    for (var i = 0; i <= binaryString.Length; i++)
                    {
                        var information = binaryString.Substring(0, i);

                        var reader = new RightReader(information, signal, binaryString.Length, @base);

                        results.AddRange(reader.Tiles());
                    }
                }
            }

            
            foreach (var signal in new[] { Signals.First, Signals.Nth })
            {
                for (var i = 0; i < encodedDigits.Count; i++)
                {
                    var binaryString = Utils.Reverse(encodedDigits[i]);

                    for (var j = 0; j <= binaryString.Length; j++)
                    {
                        var information = binaryString.Substring(0, j);

                        var reader = new LeftReader(information, signal, binaryString.Length);

                        results.AddRange(reader.Tiles());
                    }
                }
            }


            return results;
        }

        
        private static IEnumerable<Tile> AddWriters(IEnumerable<string> binaryStrings, int stoppingValue)
        {
            var results = new List<Tile>();

            var encodedDigits = binaryStrings.ToList();

            foreach (var signal in new[] { Signals.Carry, Signals.NoCarry })
            {
                foreach (var binaryString in encodedDigits)
                {
                    var writer = new Writer(binaryString, signal, stoppingValue);

                    results.AddRange(writer.Tiles());
                    
                }
            }

            foreach (var signal in new[] { Signals.First, Signals.Nth })
            {
                for (var i = 0; i < encodedDigits.Count; i++)
                {
                    var binaryString = encodedDigits[i];

                    var copier = new Copier(binaryString, signal);

                    var tiles = copier.Tiles().ToList();

                    results.AddRange(tiles);
                    var incrementStopper = new IncrementStopper(binaryString, signal);
                    results.AddRange(incrementStopper.Tiles());
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
            Console.WriteLine($".tds file can be found at {path}\n\n");
        }


        private static IEnumerable<Tile> GetAllGadgets()
        {
            var copyStopperCarry   = new CopyStopper(Signals.Carry);
            var copyStopperNoCarry = new CopyStopper(Signals.NoCarry);

            
            return copyStopperNoCarry.Tiles().Concat(copyStopperCarry.Tiles());
        }
    }

}