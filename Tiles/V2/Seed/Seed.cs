namespace Tiles.V2.Seed
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Models;

    using Write;

    public class Seed
    {

        public readonly Tile Start;

        
        public readonly List<Tile> Tiles = new List<Tile>();

        private readonly int Height;

        private readonly int Digits;

        public Seed(CounterSettings settings)
        {
            Height = (settings.BitsRequiredToEncodeUpToBaseValueInBinary * 4) + 1;
            Digits = settings.HorizontalDigitsPerRow;

            SetUp();

            Start = Tiles.First();
        }


        private void SetUp()
        {
            CreateLeftWall();

        }

        private Glue Bind(Tile a, Tile b) => new Glue($"{a.Name} -> {b.Name}");

        private Tile Tile(string name) => new Tile($"Seed {name}");


        private void CreateLeftWall()
        {
            for (var i = 0; i < Height * 2; i++)
            {
                var tile = i == 0 ? new Tile("Seed") : Tile(i.ToString());

                Tiles.Add(tile);
            }

            var previous = Guid.NewGuid();
            var seed     = Tiles[0];
            seed.North = new Glue(previous.ToString());

            for (var i = 1; i < Height; i++)
            {
                var current = Tiles[i];
                current.South = new Glue(previous.ToString());

                previous = Guid.NewGuid();

                var isTopCorner = i + 1 == Height;

                if (isTopCorner)
                {
                    current.East = new Glue(previous.ToString());
                }
                else
                {
                    current.North = new Glue(previous.ToString());
                }
            }

            for (var i = Height; i < Height * 2; i++)
            {
                var current = Tiles[i];

                var isTopCorner = i == Height;

                if (isTopCorner)
                {
                    current.West = new Glue(previous.ToString());
                }
                else
                {
                    current.North = new Glue(previous.ToString());
                }

                previous      = Guid.NewGuid();
                current.South = new Glue(previous.ToString());
            }

            var left = new Tile($"Seed {Tiles.Count}") {
                North = Tiles.Last().South,
            };
            Tiles.Add(left);

            var middle = new Tile($"Seed {Tiles.Count}");  
            Tiles.Add(middle);
            left.East = Bind(left, middle);
            middle.West = left.East;

            var right = new Tile($"Seed {Tiles.Count}"); 
            Tiles.Add(right);

            middle.East = Bind(middle, right);
            right.West = middle.East;
            right.North = new Glue(Guid.NewGuid().ToString());

            var previousGlue = Guid.NewGuid();
            for (var i = 0; i < Digits; i++)
            {
                var digit = CreateDigits(i);

                if (i == 0)
                {
                    digit.RemoveAt(0);
                    digit[0].South = right.North;
                }

                Tiles.AddRange(digit);
            }
        }

        private List<Tile> CreateDigits(int offset)
        {
            var bitsPerDigit = (Height - 1) / 4;
            var binary = string.Concat(Enumerable.Repeat("0", bitsPerDigit));
            var left = new Tile($"Seed {offset}, Left") {
                West = new Glue($"Seed {offset - 1}"), 
                East = new Glue($"Seed {offset}, Left -> Middle")
            };


            var middle = new Tile($"Seed {offset}, Middle") {
                West  = left.East,
                East = new Glue($"Seed {offset}, Middle -> Right")
            };

            var id = Guid.NewGuid();

            var right = new Tile($"Seed {offset}, Right") {
                West = middle.East,
                North = new Glue(id.ToString())
            };
      

            var bits = new List<Tile>();

            for (var i = 0; i < bitsPerDigit; i++)
            {
                var zero = new ZeroBit(binary, i, $"Seed {offset}, Bit {i} ");

                var tilesZ = zero.Tiles().ToList();

                tilesZ.Last().South = new Glue(id.ToString());

                id = Guid.NewGuid();

                tilesZ.First().North = new Glue(id.ToString());

                bits.AddRange(tilesZ);
            }


            var hook = new Hook($"Seed {offset}, Hook", Height - 1, true) {
                NorthGlue = new Glue(id.ToString())
            };


            var hooktiles = hook.Tiles();
            hook.AttachTop(new Glue(id.ToString()));
            hook.AttachEast(new Glue($"Seed {offset}"));
            var hookTiles = hooktiles.ToList();
            
            hookTiles.RemoveAt(0);

            var tileToConcat   = hookTiles.FirstOrDefault(t => t.South is null);

            if (tileToConcat != null)
            {
                tileToConcat.South = new Glue();
            }

            
            var tiles = new List<Tile> {left, middle, right }; 
            tiles.AddRange(hookTiles);
            tiles.AddRange(bits);

            return tiles;
        } 
    }
}
