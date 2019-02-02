namespace Tiles.Gadgets
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Models;

    public class BitReader : AbstractGadget
    {

        private readonly List<Tile> tiles;
        private readonly int BitsToRead;

        private readonly IEnumerable<string> binaryPatterns;

        public IEnumerable<Tile> Tiles { get; }
        private readonly string signal;

        public BitReader(CounterSettings settings, bool hasCarry)
        {
            signal = hasCarry ? "Carry" : "NoCarry";
            binaryPatterns = settings.BinaryDigitPatterns;
            tiles = new List<Tile> {
                new Tile(Name(A)) {
                    East = hasCarry ? Glues.ReaderCarry : Glues.ReaderNoCarry,
                    West = new Glue($"BitReader Test: 1 {signal}")
                }
            };

            Tiles = tiles;
            BitsToRead = settings.BitsRequiredToEncodeUpToBaseValueInBinary;
            AddTesters();
        }


        private void AddTesters()
        {
            foreach (var bitString in binaryPatterns)
            {
                List<char> bits = bitString.ToList();
                var tilesToReadThisBitString = new List<Tile>();
                var bitData = "";
                for (var i = 0; i < bits.Count; i++)
                {
                    var bit = bits[i];
                    bitData += bit;

                    var bottomLeft = new Tile($"BottomLeft: {bitData}, Bottom, {signal}") {
                        East  = new Glue($"Index: {i}, Result={bit}, BitData={bitData}, Bottom, {signal}"),
                        North = new Glue($"Index: {i}, Result={bit}, BitData={bitData}, {signal}"),
                    };

                    var topLeft    = new Tile($"TopLeft: {bitData}, , Top, {signal}") {
                        South = bottomLeft.North,
                        East = new Glue($"Index: {i}, Result={bit}, BitData={bitData}, Top, {signal}"),
                    };

                    var topRightZ1 = new Tile($"TopRightZ1: {bitData}, {signal}") {
                        West = topLeft.East,
                        Down = new Glue($"Index: {i}, Result={bit}, BitData={bitData}, {signal}")
                    };

                    var topRightZ0 = new Tile($"TopRightZ0: {bitData}, {signal}")
                    {
                        Up    = topRightZ1.Down,
                        North = new Glue($"Test: {i + 1}, BitData={bitData}, {signal}")
                    };

                    tilesToReadThisBitString.AddRange(new[] { bottomLeft, topLeft, topRightZ1, topRightZ0 });

                    if (bitData.Length == bits.Count)
                    {
                        var increment = signal == "Carry" ? 1 : 0;

                        var nextCarrySignal = signal == "NoCarry" ? "NoCarry" : "Carry";


                        var value = Convert.ToString(Convert.ToInt32(bitData, 2) + increment, 2);

                        if (value.Length < bitData.Length)
                        {
                            var leadingZeroes = bitData.Length - value.Length;
                            value = value.PadLeft(value.Length + leadingZeroes, '0');
                        }

                        var writer = new Tile($"{Writer.Type} {value} {nextCarrySignal}") {
                            South = topRightZ0.North,
                            North = new Glue($"{Writer.Type} {value} {nextCarrySignal}"),
                            Color = "green",
                        };

                        tilesToReadThisBitString.AddRange(new[] { writer });
                    } else
                    {

                        var testA = new Tile($"Test: {i + 1}a, BitData={bitData}, {signal}")
                        {
                            South = topRightZ0.North,
                            Up    = new Glue($"Test: {i  + 1}, BitData={bitData}, {signal}"),
                            West  = new Glue($"Index: {i + 1}, Result=1, BitData={bitData}1, Bottom, {signal}")
                        };

                        var testB = new Tile($"Test: {i + 1}b, BitData={bitData}, {signal}")
                        {
                            Down = testA.Up,
                            West = new Glue($"Index: {i + 1}, Result=0, BitData={bitData}0, Bottom, {signal}")
                        };
                        tilesToReadThisBitString.AddRange(new []{ testA, testB });
                    }
                }

                tiles.AddRange(tilesToReadThisBitString);
            }
            var testZ0 = new Tile($"Test: 0a, BitData=, {signal}") {
                West = new Glue($"Index: 0, Result=1, BitData=0, Bottom, {signal}"),
                Up   = new Glue($"Test: 0, BitData=, {signal}"),
                East = tiles.First().West
            };

            var testZ1 = new Tile($"Test: 0b, BitData=, {signal}") {
                Down = testZ0.Up,
                West = new Glue($"Index: 0, Result=0, BitData=0, Bottom, {signal}"),
            };

            tiles.AddRange(new[] {testZ1, testZ0 });

        }


        string BitResult(int testNumber, int testedBitValue) => $"BitReader Test: {testNumber} {signal} = {testedBitValue}";

        string Name(string value) => $"BitReader {value} {signal}";

        const string Init = "Index";

        string Bind(string a, string b) => $"BitReader {a} {signal} -> {b} {signal}";
    }

    public class RightToLeftBitTest
    {



    }
}
