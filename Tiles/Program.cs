namespace Tiles
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;
    using System.Security.Policy;

    using Gadgets;

    using IO;

    using Models;

    using Newtonsoft.Json;

    using V2;
    using V2.Seed;
    using V2.Write;

    using Hook = V2.Write.Hook;
    using Writer = V2.Write.Writer;


    internal class Program
    {

        public static void Main(string[] args)
        {
            const string stoppingValue = "1654545";

            //for (var i = 2; i <= 36; i++)
            //{
                var settings = new CounterSettings(15, int.Parse(stoppingValue));
                CreateTiles.Write(settings);
            //}

  
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

            var tileSetName = $"test";
            var names = new HashSet<string>();

            var path = Utils.GetPath();


            //foreach (var binaryString in settings.BinaryDigitPatterns)
            //{

            //    for (var i = binaryString.Length; i > 0; i--)
            //    {
            //        var carry = binaryString.Contains('0');

            //        var value = binaryString[i - 1];
            //        var bit   = value == '0' ? (AbstractBit)new ZeroBit(binaryString, i - 1, Modes.Increment, carry) : 
            //                                                new OneBit(binaryString, i - 1, Modes.Increment, carry);

            //        var signal = carry ? Signals.Carry : Signals.NoCarry;
            //        bit.SouthGlue = new Glue($"{previous.ToString()}, {signal}");
            //        // bit.NorthGlue 
            //        previous      = Guid.NewGuid();



            //        tiles.UnionWith(bit.Tiles());
            //    }

            //    var incremented = Convert.ToString(Convert.ToInt32(binaryString, 2) + 1, 2);

            //    var newBinary   = incremented.PadLeft(Math.Abs(incremented.Length - binaryString.Length) + incremented.Length, '0');
            //    var noCarryHoke = new Hook(newBinary, binaryString.Length * 4, false);

            //    var carryHook = new Hook(newBinary, binaryString.Length * 4, true);

            //    tiles.UnionWith(noCarryHoke.Tiles());
            //    tiles.UnionWith(carryHook.Tiles());

            //    var writerCarry   = new Writer(newBinary,    Signals.NoCarry);
            //    var writerNoCarry = new Writer(binaryString, Signals.NoCarry);
            //    tiles.UnionWith(writerCarry.Tiles());
            //    tiles.UnionWith(writerNoCarry.Tiles());
            //}
            

            

            tiles.UnionWith(GetAllGadgets());


            var options = new TdpOptions(tileSetName, seed.Start.Name);
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


        private static IEnumerable<Tile> GetAllGadgets()
        {
            var copyStopperCarry   = new CopyStopper(Signals.Carry);
            var copyStopperNoCarry = new CopyStopper(Signals.NoCarry);

            
            return copyStopperNoCarry.Tiles().Concat(copyStopperCarry.Tiles());
        }
    }
}
