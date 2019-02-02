namespace Tiles.Models
{

    using System.Collections.Generic;
    using System.Linq;

    public class Writer
    {

        public static string Type => "WRITER";
        private readonly List<Tile> tiles;


        public IEnumerable<Tile> Tiles => tiles;

        private readonly bool NextReaderSignal;

        private readonly string Bits;

        private readonly string StartKey;

        private readonly int HookHeight;

        private readonly string BitPattern;

        private Tile TopLeftCorner;

        public Writer(CounterSettings settings, string value, string signal)
        {
            HookHeight = settings.BitsRequiredToEncodeUpToBaseValueInBinary * 2;
            tiles = new List<Tile>();
            BitPattern = value;
            StartKey = $"{Type} {value} {signal}";
            TopLeftCorner = new Tile($"{Type} {value} TopLeftZ0");
        }


        private void HookTiles()
        {
            var hookLeftLeft = new Tile(Name("HookLeftLeft"))
            {
                East = BindDigitRow("HookLeftLeft", "HookLeftRight")
            };

            var hookLeftRight = new Tile(Name("HookLeftRight"))
            {
                West = hookLeftLeft.East,
                East = BindDigitRow("HookLeftRight", "HookRightLeft")
            };

            var hookRightLeft = new Tile(Name("HookRightLeft"))
            {
                West = hookLeftRight.East,
                East = BindDigitRow("HookRightLeft", "HookRightRight")
            };

            var hookRightRight = new Tile(Name("HookRightRight"))
            {
                West = hookRightLeft.East,
                South = BindDigitRow("HookRightRight", "HookDown")
            };

            var hookTiles = new List<Tile> { hookLeftLeft, hookLeftRight, hookRightLeft, hookRightRight };

            for (var j = 0; j < HookHeight; j++)
            {
                var tile = new Tile(Name($"HookDown {j}"))
                {
                    North = j == 0 ? hookRightRight.South : BindDigitRow($"HookDown {j - 1}", $"HookDown {j}"),
                    South = BindDigitRow($"HookDown {j}", $"HookDown {j + 1}")
                };
                hookTiles.Add(tile);
            }

            var hookBottomRight = new Tile(Name("HookBottomRight"))
            {
                North = hookTiles.Last().South,
                West = BindDigitRow("HookBottomRight", "HookBottomMiddle")
            };

            var hookBottomMiddle = new Tile(Name("HookBottomMiddle"))
            {
                East = hookBottomRight.West,
                West = BindDigitRow("HookBottomMiddle", "HookBottomLeft")
            };

            var hookBottomLeft = new Tile(Name("HookBottomLeft"))
            {
                East = hookBottomMiddle.West,
                Down = BindDigitRow("HookBottomLeft", "MiddleRight")
            };

            hookTiles.AddRange(new[] { hookBottomRight, hookBottomMiddle, hookBottomLeft });

            
        }


        private Glue BindDigitRow(string a, string b) => new Glue($"{Type} {BitPattern} {a} -> {b}");
        private string Name(string s) => $"{StartKey} {s}";

    }

}
