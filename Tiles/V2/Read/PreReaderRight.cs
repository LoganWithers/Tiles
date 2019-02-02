namespace Tiles.V2.Read
{

    using System;
    using System.Collections.Generic;

    using Models;

    public class PreReaderRight
    {

        private readonly int height;

        private readonly List<Tile> tiles;

        private const string GadgetName = "PreReaderRight";
        public IEnumerable<Tile> Tiles => tiles;


        private readonly string signal;
        public PreReaderRight(CounterSettings settings, bool carry)
        {
            height = settings.BitsRequiredToEncodeUpToBaseValueInBinary * 4;
            tiles = new List<Tile>();
            signal = carry ? Signals.Carry : Signals.NoCarry;
            SetUp();
        }


        private void SetUp()
        {
            var id = Guid.NewGuid();

            for (var i = 0; i < height; i++)
            {
                var tile = Tile(i);

                tile.North = i == 0 ? Glues.PreReadRight(signal) : G(id);

                id = Guid.NewGuid();

                if (i + 1 == height)
                {
                    tile.West = G(id);
                } else
                {
                    tile.South = G(id);
                }

                tiles.Add(tile);
            }

            var right = Tile(height);
            right.East = G(id);
            id = Guid.NewGuid();
            right.West = G(id);
            tiles.Add(right);

            var left = Tile(height + 1);
            left.East = right.West;

            left.West = Glues.Reader(signal);

            tiles.Add(left);
        }


        private Glue G(Guid id) => new Glue(id.ToString());
        private Tile Tile(int tileIndex) => new Tile($"{GadgetName} {tileIndex}, S={signal}");
    }
}
