namespace Tiles.V2.Seed
{

    using System.Collections.Generic;

    using Models;

    public class IncrementStopper
    {

        public IEnumerable<Tile> Tiles() => tiles;
        private List<Tile> tiles = new List<Tile>();

        private const string GadgetName = "IncrementStopper";

        private readonly string signal;
        private readonly string binaryValue;

        private readonly bool isFirst;
        public IncrementStopper(string binaryString, string signal)
        {
            this.signal      = signal;
            this.binaryValue = binaryString;
            isFirst = signal == Signals.First;
            SetUp();
        }


        private void SetUp()
        {
            if (isFirst)
            {
                First();
            } else
            {
                Nth();
            }
        }


        private void First()
        {
            var connector = Tile(0);
            connector.South = Glues.IncrementStopper(binaryValue, signal);

            var middleZ0  = Tile(1);
            connector.North = Bind(connector, middleZ0);
            middleZ0.South = connector.North;


            var middleZ1  = Tile(2);
            middleZ0.Up = Bind(middleZ0, middleZ1);
            middleZ1.Down = middleZ0.Up;


            var rightZ1   = Tile(3);
            middleZ1.East = Bind(middleZ1, rightZ1);
            rightZ1.West = middleZ1.East;

            var rightZ0   = Tile(4);
            rightZ1.Down  = Bind(rightZ1, rightZ0);
            rightZ0.Up    = rightZ1.Down;
            rightZ0.North = Glues.Copier(binaryValue, signal);
            rightZ0.Color = "green";
            tiles.AddRange(new []{ connector, middleZ0, middleZ1, rightZ0, rightZ1 });
        }


        private void Nth()
        {
            var connector = Tile(0);
            connector.South = Glues.IncrementStopper(binaryValue, signal);
            var middleZ0  = Tile(1);
            var leftZ0    = Tile(2);
            var leftZ1    = Tile(3);
            var middleZ1  = Tile(4);
            var rightZ1   = Tile(5);
            var rightZ0   = Tile(6);

            tiles.AddRange(new[] { connector, middleZ0, leftZ0, leftZ1, middleZ1, rightZ1, rightZ0 });

        }

        private Tile Tile(int tileIndex) => new Tile($"{GadgetName}, {tileIndex}, {signal}, {binaryValue}");
        private Glue Bind(Tile a, Tile b) => new Glue($"{a.Name} -> {b.Name}");
     }
}
