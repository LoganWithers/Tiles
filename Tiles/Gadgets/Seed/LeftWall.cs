﻿namespace Tiles.Gadgets.Seed
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Models;

    public class LeftWall
    {

        private readonly int bits;

        private readonly List<Tile> tiles;

        private const string GadgetName = "LeftWall";

        private readonly Tile bottomRightZ0 = new Tile(TileNames.LeftWallBottomRightZ0); 
        private readonly Tile bottomRightZ1 = new Tile(TileNames.LeftWallBottomRightZ1); 
        private readonly Tile bottomMiddle  = new Tile(TileNames.LeftWallBottomMiddle);
        private readonly Tile topMiddle     = new Tile(TileNames.LeftWallTopMiddle); 
        private readonly Tile topLeft       = new Tile(TileNames.LeftWallTopLeft);
        
        public LeftWall(CounterSettings settings)
        {
            bits = settings.BitsRequiredToEncodeUpToBaseValueInBinary;
            tiles = new List<Tile>();
            Setup();
        }


        public IEnumerable<Tile> Tiles => tiles;
        

        private void Setup()
        {
            tiles.AddRange(new []{ bottomRightZ0, bottomRightZ1, bottomMiddle });
            bottomRightZ0.North = GlueFactory.LeftWall;

            bottomRightZ0.Up = Bind(bottomRightZ0, bottomRightZ1);
            bottomRightZ1.Down = bottomRightZ0.Up;

            bottomRightZ1.West = Bind(bottomRightZ1, bottomMiddle);
            bottomMiddle.East = bottomRightZ1.West;
            bottomMiddle.North = Bind(0, 1);

            var height = bits * 4;

            for (var i = 0; i <= height; i++)
            {
                var tile = Tile(i);
                tile.North = Bind(i, i + 1);
                tile.South = Bind(i - 1, i);
                tiles.Add(tile);
            }

            var lastAdded = tiles.Last();

            lastAdded.North = Bind(lastAdded, topMiddle);
            topMiddle.South = lastAdded.North;

            tiles.Add(topMiddle);

            topMiddle.West = Bind(topMiddle, topLeft);
            topLeft.East = topMiddle.West;

            tiles.Add(topLeft);


            // 2n + 2 start north and south
            var leftSideHeight = height * 2 + 4;

            var id = Guid.NewGuid();
            topLeft.Down = new Glue(id.ToString());

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
                    tile.South = GlueFactory.PreReadLeft(Signals.First);
                }
            }
            

        }

        private Glue Bind(int a, int b) => new Glue($"{GadgetName} {a} -> {GadgetName} {b}");
        private Glue Bind(Tile a, Tile b) => new Glue($"{a.Name} -> {b.Name}");
        private Tile Tile(int tileIndex) => new Tile($"{GadgetName} {tileIndex}");
        private Tile Tile(string section, int tileIndex) => new Tile($"{GadgetName} {section} {tileIndex}");
    }
}
