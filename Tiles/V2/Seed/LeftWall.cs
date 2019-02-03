namespace Tiles.V2.Seed
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Models;

    public class LeftWall
    {

        private readonly int Bits;

        private readonly List<Tile> tiles;

        private const string GadgetName = "LeftWall";

        private readonly Tile BottomRightZ0 = new Tile(TileNames.LeftWallBottomRightZ0); 
        private readonly Tile BottomRightZ1 = new Tile(TileNames.LeftWallBottomRightZ1); 
        private readonly Tile BottomMiddle  = new Tile(TileNames.LeftWallBottomMiddle);
        private readonly Tile TopMiddle     = new Tile(TileNames.LeftWallTopMiddle); 
        private readonly Tile TopLeft       = new Tile(TileNames.LeftWallTopLeft);
        
        public LeftWall(CounterSettings settings)
        {
            Bits = settings.BitsRequiredToEncodeUpToBaseValueInBinary;
            tiles = new List<Tile>();
            Setup();
        }


        public IEnumerable<Tile> Tiles => tiles;
        

        private void Setup()
        {
            tiles.AddRange(new []{ BottomRightZ0, BottomRightZ1, BottomMiddle });
            BottomRightZ0.North = Glues.LeftWall;

            BottomRightZ0.Up = Bind(BottomRightZ0, BottomRightZ1);
            BottomRightZ1.Down = BottomRightZ0.Up;

            BottomRightZ1.West = Bind(BottomRightZ1, BottomMiddle);
            BottomMiddle.East = BottomRightZ1.West;
            BottomMiddle.North = Bind(0, 1);

            var height = Bits * 4;

            for (var i = 0; i <= height; i++)
            {
                var tile = Tile(i);
                tile.North = Bind(i, i + 1);
                tile.South = Bind(i - 1, i);
                tiles.Add(tile);
            }

            var lastAdded = tiles.Last();

            lastAdded.North = Bind(lastAdded, TopMiddle);
            TopMiddle.South = lastAdded.North;

            tiles.Add(TopMiddle);

            TopMiddle.West = Bind(TopMiddle, TopLeft);
            TopLeft.East = TopMiddle.West;

            tiles.Add(TopLeft);


            // 2n + 2 start north and south
            var leftSideHeight = height * 2 + 4;

            var id = Guid.NewGuid();
            TopLeft.Down = new Glue(id.ToString());

            for (var i = 0; i < leftSideHeight; i++)
            {
                var tile = Tile("LeftSide", i);
                tiles.Add(tile);

                tile.South = new Glue(id.ToString());
                id = Guid.NewGuid();
                tile.North = new Glue(id.ToString());

                if (i == 0)
                {
                    tile.Up    = tile.South;
                    tile.South = new Glue();
                } 

                if (i + 1 == leftSideHeight)
                {
                    tile.East  = tile.North;
                    tile.North = new Glue();
                }
            }

            var rightSideHeight = height + 2;

            for (var i = 0; i < rightSideHeight; i++)
            {
                var tile = Tile("RightSide", i);
                tiles.Add(tile);

                tile.North = new Glue(id.ToString());
                id         = Guid.NewGuid();
                tile.South = new Glue(id.ToString());


                if (i == 0)
                {
                    tile.West = tile.North;
                    tile.North = new Glue();
                }

                if (i + 1 == rightSideHeight)
                {
                    tile.South = Glues.PreReadLeft(Signals.First);
                    tile.Color = "yellow";
                }
            }
            

        }

        private Glue Bind(int a, int b) => new Glue($"{GadgetName} {a} -> {GadgetName} {b}");
        private Glue Bind(Tile a, Tile b) => new Glue($"{a.Name} -> {b.Name}");
        private Tile Tile(int tileIndex) => new Tile($"{GadgetName} {tileIndex}");
        private Tile Tile(string section, int tileIndex) => new Tile($"{GadgetName} {section} {tileIndex}");
    }
}
