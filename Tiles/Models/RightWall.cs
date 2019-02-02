namespace Tiles.Models
{

    using System.Collections.Generic;
    using System.Linq;

    using Gadgets;

    public class RightWall
    {
        private readonly int height;

        public RightWall(CounterSettings settings)
        {
            height = settings.ActualHeightPerDigit;
        }
        
        public IEnumerable<Tile> Tiles()
        {
            var tiles = new List<Tile>();
            var side = "RightSide";

            string Name(int value) => $"RightWall {value} {side}";

            var i = 0;
            Glue BindIndexes(int a, int b) => new Glue($"RightWall {side} {a} -> RightWall {side} {b}");
            IEnumerable<Tile> RightSide()
            {
                var results = new List<Tile>();
                for (; i <= height; i++)
                {
                    Tile tile;

                    if (i == 0)
                    {
                        tile = new Tile(Name(i)) {
                            West = Glues.RightWall,
                            North = BindIndexes(i, i + 1)
                        };
                    }
                    else if (i < height)
                    {
                        tile = new Tile(Name(i)){
                            South = BindIndexes(i - 1, i),
                            North = BindIndexes(i, i + 1),
                        };}
                    else
                    {
                        tile = new Tile(Name(i))
                        {
                            South = BindIndexes(i - 1, i),
                            West = BindIndexes(i, i + 1)
                        };
                    }

                    results.Add(tile);
                }

                var secondFromRightCorner = results.Last();
                secondFromRightCorner.North = new Glue("RightWall TopRight A");
                secondFromRightCorner.Name = "RightWall TopRight A";

                var topRightCornerZ0 = new Tile("RightWall TopRight B")
                {
                    South = secondFromRightCorner.North,
                    Up = new Glue("RightWall TopRight B -> C")
                };

                var topRightCornerZ1 = new Tile("RightWall TopRight C")
                {
                    Down = topRightCornerZ0.Up,
                    West = new Glue("RightWall TopRight C -> TopLeft A")
                };

                var topLeftCornerZ1 = new Tile("RightWall TopLeft A")
                {
                    East = topRightCornerZ1.West,
                    Down = new Glue("RightWall TopLeft A -> B")
                };

                var topLeftCornerZ0 = new Tile("RightWall TopLeft B")
                {
                    Up = topLeftCornerZ1.Down,
                    South = new Glue("RightWall TopLeft B -> C")
                };

                var secondFromTopLeft = new Tile("RightWall TopLeft C")
                {
                    North = topLeftCornerZ0.South,
                    South = new Glue("RightWall TopLeft C")
                };

                results.AddRange(new[] { topLeftCornerZ0, topLeftCornerZ1, topRightCornerZ0, topRightCornerZ1, secondFromTopLeft });
                return results;
            }

            tiles.AddRange(RightSide());

            side = "LeftSide";

            for (var j = 0; j < height - 1; j++, i++)
            {
                var tile = new Tile(Name(i))
                {
                    North = j == 0 ? tiles.Last().South : BindIndexes(i - 1, i),
                    South = BindIndexes(i, i + 1)
                };

                if (j + 1 == height - 1)
                {
                    tile.South = new Glue();
                    tile.West = Glues.ReaderCarry;
                }

                tiles.Add(tile);
            }

            return tiles;
        }
    }
}
