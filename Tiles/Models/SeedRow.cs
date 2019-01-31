namespace Tiles.Models
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gadgets;

    using Newtonsoft.Json;

    public class SeedRow
    {

        private readonly CounterSettings settings;

        private int height; 
        private readonly int width;

        private int Bits;
        public SeedRow(CounterSettings settings)
        {
            this.settings = settings;
            width  = settings.HorizontalDigitsPerRow;
            height = settings.ActualHeightPerDigit;
            Bits = settings.BitsRequiredToEncodeUpToBaseValueInBinary;

            // var jsonString = JsonConvert.SerializeObject(settings, Formatting.Indented);
            // Console.WriteLine(jsonString);
        }


        public List<Tile> Tiles()
        {
            var leftWall            = CreateLeftWall();
            var zeroes              =  CreateZeroBits();
            var rightWallAttachment = CreateRightWallAttachment();

            var tiles = new List<Tile>();
            tiles.AddRange(leftWall);
            tiles.AddRange(zeroes);
            tiles.AddRange(rightWallAttachment);

            return tiles;
        }

        /// <summary>
        /// The left wall should always have  2 x DigitHeight + 4
        ///  
        /// </summary>
        private List<Tile> CreateLeftWall()
        {
            var tiles = new List<Tile>();
            var currentId = Guid.NewGuid();

            var side = "LeftSide";
            var seed = new Tile("LeftWallLeftSide") {
                North = Glue(1, 2, currentId)
            };

            tiles.Add(seed);
            var i = 2;
            var previousId = currentId;
            
            for (; i <= height; i++)
            {
                currentId = Guid.NewGuid();
                Tile tile;

                if (i < height)
                {
                    tile = new Tile(Name(i, side)) {
                        South = Glue(i - 1, i, previousId),
                        North = Glue(i, i + 1, currentId),
                    };
                } else
                {
                    tile = new Tile(Name(i, side)) {
                        South = Glue(i - 1, i , previousId),
                        East = Glue(i, i + 1, currentId) 
                    };
                }

                tiles.Add(tile);
                previousId = currentId;
            }

            var heightDown = height * 2;
            side = "RightSide";

            for (; i <= heightDown; i++)
            {
                currentId = Guid.NewGuid();
                

                var tile = new Tile(Name(i, side)) {
                    South = Glue(i, i + 1, currentId)
                }; 

                var parentGlue = Glue(i - 1, i, previousId);
                var isTopRightCorner = i - 1 == height;

                if (isTopRightCorner)
                {
                    tile.West = parentGlue;
                } else
                {
                    tile.North = parentGlue;
                }

                tiles.Add(tile);
                previousId = currentId;
            }

            currentId = Guid.NewGuid();
            var topLeft = tiles.Last();
            
            var leftCorner = new Tile(Name(i + 1, "Dip")) {
                North = topLeft.South,
                East = Glue(i + 1, i + 2, currentId)
            };

            var middle = new Tile(Name(i + 2, "Dip")) {
                West = leftCorner.East,
                East  = Glue(i + 2, i + 3, currentId)
            };

            var bottomRight = new Tile(Name(i + 3, "Dip")) {
                West = middle.East,
                North = Glue(i + 3, i + 4, currentId)
            };

            var topRight = new Tile(Name(i + 4, "Dip")) {
                South  = bottomRight.North,
                East   = new Glue("SeedRow Digit 0")
            };

            tiles.AddRange(new []{ leftCorner, middle, bottomRight, topRight });

            return tiles;
        }


        private static string Name(int index, string unique) => $"LeftWall {unique} {index}";

        private static Glue Glue(int a, int b, Guid id) => new Glue($"{a} -> {b} {id.ToString()}");


        private List<Tile> CreateRightWallAttachment()
        {
            var left = new Tile("SeedRow BottomLeft"){
                West = new Glue($"SeedRow Bit: {width} RightRight {width - 1} -> LeftLeft {width}"),
                East = Glues.RightWall,
            };

            //var right = new Tile("SeedRow BottomRight") {
            //    West  = left.East,
            //    East = Glues.RightWall,
            //};
    
            return new List<Tile> {left};
        }

        private List<Tile> CreateZeroBits()
        {
            var tiles = new List<Tile>();
            
            for (var i = 0; i < width; i++)
            {
                Glue BindDigitRow(string a, string b) => new Glue($"SeedRow Bit: {i} {a} -> {b}");

                string Name(string name) => $"SeedRow Bit: {i} {name}";

                var leftLeft = new Tile(Name("LeftLeft")) {
                    West = new Glue($"SeedRow Bit: {i} RightRight {i-1} -> LeftLeft {i}"),
                    East = BindDigitRow("LeftLeft", "LeftRight")
                };
                
                var leftRight  = new Tile(Name("LeftRight")) {
                    West = leftLeft.East,
                    East = BindDigitRow("LeftRight", "MiddleLeft")
                };

                var bitId = Guid.NewGuid();
                var middleLeft = new Tile(Name("MiddleLeft")) {
                    West  = leftRight.East,
                    North = new Glue($"SeedRow Bit: {i} MiddleLeft -> {bitId.ToString()}")
                };

                var middleRight = new Tile(Name("MiddleRight")) {
                    Up = BindDigitRow("HookBottomLeft", "MiddleRight"),
                    East = BindDigitRow("MiddleRight", "RightLeft")
                };

                var rightLeft = new Tile(Name("RightMiddle")) {
                    West = middleRight.East,
                    East = BindDigitRow("RightLeft", "RightRight")
                };
                var rightRight  = new Tile(Name("RightRight")) {
                    West = rightLeft.East,
                    East = new Glue($"SeedRow Bit: {i + 1} RightRight {i} -> LeftLeft {i + 1}"),
                };

                if (i != 0)
                {
                    tiles.AddRange(new [] { leftLeft, leftRight, middleLeft, middleRight, rightLeft, rightRight});
                } else
                {   
                    middleLeft.West = new Glue("SeedRow Digit 0");
                    tiles.AddRange(new[] {  middleLeft, middleRight, rightLeft, rightRight });
                }
                
                var bits = new List<Bit>();
                var currentId = Guid.NewGuid();
                var previousId = currentId;

                var topKey = Guid.NewGuid();
                for (var j = 0; j < Bits; j++)
                {
                    var bit = new Bit(0, j);
                    bit.BottomLeft.South = j == 0 ? middleLeft.North : Glue(j + i - 1, j + i, previousId);

                    bit.TopLeft.North = Glue(j + i, j + i + 1, currentId);
                    
                    if (j + 1 == Bits)
                    {
                        bit.TopLeft.North = new Glue(topKey.ToString());
                    }

                    foreach (var tile in bit.Tiles)
                    {
                        tile.Name = $"SeedRow Bit: {i} Tile: {j} {tile.Name}";

                        foreach (var glue in tile.Glues)
                        {
                            if (glue.Bind == 1)
                            {
                                glue.Label = $"SeedRow Bit: {i} Tile: {j} {glue.Label}";
                            }
                        }
                    }

                    bits.Add(bit);

                    previousId = currentId;
                }

                var topLeft = new Tile(Name("TopLeft")) {
                    South = new Glue(topKey.ToString()),
                    Up = BindDigitRow("TopLeft", "HookLeftLeft")
                };

                tiles.AddRange(bits.SelectMany(bit => bit.Tiles));
                tiles.Add(topLeft);

                var hookLeftLeft = new Tile(Name("HookLeftLeft")) {
                    Down = topLeft.Up,
                    East = BindDigitRow("HookLeftLeft", "HookLeftRight")
                };

                var hookLeftRight = new Tile(Name("HookLeftRight"))
                {
                    West = hookLeftLeft.East,
                    East = BindDigitRow("HookLeftRight","HookRightLeft")
                };

                var hookRightLeft = new Tile(Name("HookRightLeft")) {
                    West = hookLeftRight.East,
                    East = BindDigitRow("HookRightLeft", "HookRightRight")
                };
 
                var hookRightRight = new Tile(Name("HookRightRight")) {
                    West = hookRightLeft.East,
                    South = BindDigitRow("HookRightRight", "HookDown")
                };

                var hookTiles = new List<Tile> { hookLeftLeft, hookLeftRight, hookRightLeft, hookRightRight };

                for (var j = 0; j < Bits * 2; j++)
                {
                    var tile = new Tile(Name($"HookDown {j}")) {
                        North = j == 0 ? hookRightRight.South : BindDigitRow($"HookDown {j - 1}", $"HookDown {j}"),
                        South = BindDigitRow($"HookDown {j}", $"HookDown {j + 1}")
                    };
                    hookTiles.Add(tile);
                }

                var hookBottomRight = new Tile(Name("HookBottomRight")) {
                    North = hookTiles.Last().South,
                    West  = BindDigitRow("HookBottomRight", "HookBottomMiddle")
                };

                var hookBottomMiddle = new Tile(Name("HookBottomMiddle")) {
                    East = hookBottomRight.West,
                    West = BindDigitRow("HookBottomMiddle", "HookBottomLeft")
                };

                var hookBottomLeft = new Tile(Name("HookBottomLeft"))  {
                    East = hookBottomMiddle.West,
                    Down = BindDigitRow("HookBottomLeft", "MiddleRight")
                };

                hookTiles.AddRange(new []{ hookBottomRight, hookBottomMiddle, hookBottomLeft });

                tiles.AddRange(hookTiles);
            }

            return tiles;
        }


    }
}
