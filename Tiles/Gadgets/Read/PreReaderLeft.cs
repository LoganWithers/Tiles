namespace Tiles.Gadgets.Read
{

    using System;
    using System.Collections.Generic;

    using Models;

    public class PreReaderLeft
    {

        private readonly int height;

        private readonly List<Tile> tiles;

        private const string GadgetName = "PreReaderLeft";
        public IEnumerable<Tile> Tiles => tiles;


        private readonly string signal;
        public PreReaderLeft(CounterSettings settings, bool isFirst)
        {
            height = settings.BitsRequiredToEncodeUpToBaseValueInBinary * 4;
            tiles = new List<Tile>();
            signal = isFirst ? Signals.First : Signals.Nth;
            SetUp();
        }


        private void SetUp()
        {
            var id = Guid.NewGuid();

            for (var i = 0; i < height; i++)
            {
                var tile = Tile(i);

                tile.North = i == 0 ? GlueFactory.PreReadLeft(signal) : G(id);

                id = Guid.NewGuid();

                if (i + 1 == height)
                {
                    tile.Up = G(id);
                    tile.South = GlueFactory.RightTurn();
                } else
                {
                    tile.South = G(id);
                }

                tiles.Add(tile);
            }

            var leftZ1  = Tile(height);
            leftZ1.Down = G(id);
            id = Guid.NewGuid();

            leftZ1.East = G(id); 

            var rightZ1   = Tile(height + 1);
            rightZ1.West  = leftZ1.East;
            rightZ1.East  = GlueFactory.Reader(signal);
            tiles.Add(leftZ1);
            tiles.Add(rightZ1);
            
        }


        private Glue G(Guid id) => new Glue(id.ToString());
        private Tile Tile(int tileIndex) => new Tile($"{GadgetName} {tileIndex}, S={signal}");
    }
}
