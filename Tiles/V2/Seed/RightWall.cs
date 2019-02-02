namespace Tiles.V2.Seed
{

    using System;
    using System.Collections.Generic;

    using Models;

    public class RightWall
    {

        public IEnumerable<Tile> Tiles => tiles; 
        private readonly List<Tile> tiles;
        private readonly int height;

        private readonly Tile RightBottom = new Tile(TileNames.RightWallTopRightBottom);
        private readonly Tile RightMiddle = new Tile(TileNames.RightWallTopRightMiddle);
        private readonly Tile RightTopZ0  = new Tile(TileNames.RightWallTopRightTopZ0);
        private readonly Tile RightTopZ1  = new Tile(TileNames.RightWallTopRightTopZ1);
        private readonly Tile LeftBottom  = new Tile(TileNames.RightWallTopLeftBottom);
        private readonly Tile LeftMiddle  = new Tile(TileNames.RightWallTopLeftMiddle);
        private readonly Tile LeftTopZ0   = new Tile(TileNames.RightWallTopLeftTopZ0);
        private readonly Tile LeftTopZ1   = new Tile(TileNames.RightWallTopLeftTopZ1);

        public RightWall(CounterSettings settings)
        {
            height = settings.BitsRequiredToEncodeUpToBaseValueInBinary;
            tiles = new List<Tile>();
            SetUp();
        }


        private void SetUp()
        {
            var id = Guid.NewGuid();
            
            
            for (var i = 0; i <= height * 4; i++)
            {
                var tile = new Tile($"RightWall {i}");

                if (i == 0)
                {
                    tile.West = Glues.RightWall;
                } else
                {
                    tile.South = G(id);
                }

                id = Guid.NewGuid();
                tile.North = G(id);
                tiles.Add(tile);
            }

            tiles.AddRange(new [] {RightBottom, RightMiddle, RightTopZ0, RightTopZ1, LeftBottom, LeftMiddle, LeftTopZ0, LeftTopZ1 });

            RightBottom.South = G(id);
            RightBottom.North = Bind(RightBottom, RightMiddle);
            RightMiddle.South = RightBottom.North;
            RightMiddle.North = Bind(RightMiddle, RightTopZ0);
            RightTopZ0.South  = RightMiddle.North;
            RightTopZ0.Up     = Bind(RightTopZ0, RightTopZ1);
            RightTopZ1.Down   = RightTopZ0.Up;

            RightTopZ1.West   = Bind(RightTopZ1, LeftTopZ1);
            LeftTopZ1.East    = RightTopZ1.West;

            LeftTopZ1.Down = Bind(LeftTopZ1, LeftTopZ0);
            LeftTopZ0.Up = LeftTopZ1.Down;

            LeftTopZ0.South = Bind(LeftTopZ0, LeftMiddle);
            LeftMiddle.North = LeftTopZ0.South;

            LeftMiddle.South = Bind(LeftMiddle, LeftBottom);
            LeftBottom.North = LeftMiddle.South;

            LeftBottom.South = Glues.PreReadRight(Signals.Carry);

        }

        Tile Tile(string name)    => new Tile(name);
        Glue Bind(Tile a, Tile b) => new Glue($"{a.Name} -> {b.Name}");
        Glue G(Guid id) => new Glue(id.ToString());
    }
}
