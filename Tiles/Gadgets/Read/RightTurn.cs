namespace Tiles.Gadgets.Read
{

    using System;
    using System.Collections.Generic;

    using Models;

    public class RightTurn
    {

        public IEnumerable<Tile> Tiles => tiles;

        private readonly List<Tile> tiles;

        private const string GadgetName = "RightTurn";

        private readonly int height;

        public RightTurn(CounterSettings settings)
        {
            tiles = new List<Tile>();
            height = settings.BitsRequiredToEncodeUpToBaseValueInBinary * 4;

            SetUp();
        }


        private void SetUp()
        {
            var middleBottom = Tile("MiddleBottom");
            var leftBottom   = Tile("LeftBottom");
            var leftMiddle   = Tile("LeftMiddle");
            var leftTopZ0    = Tile("LeftTopZ0");
            var leftTopZ1    = Tile("LeftTopZ1");
            var middleTop    = Tile("MiddleTopZ1");
            var rightTop     = Tile("RightTop");

            tiles.AddRange(new[] {middleBottom, leftBottom, leftMiddle, leftTopZ0, leftTopZ1, middleTop, rightTop});

            middleBottom.North = GlueFactory.RightTurn();
            middleBottom.West  = Bind(middleBottom, leftBottom);
            leftBottom.East    = middleBottom.West;


            leftBottom.North = Bind(leftBottom, leftMiddle);
            leftMiddle.South = leftBottom.North;

            leftMiddle.North = Bind(leftMiddle, leftTopZ0);
            leftTopZ0.South  = leftMiddle.North;

            leftTopZ0.Up   = Bind(leftTopZ0, leftTopZ1);
            leftTopZ1.Down = leftTopZ0.Up;

            leftTopZ1.East = Bind(leftTopZ1, middleTop);
            middleTop.West = leftTopZ1.East;

            middleTop.East = Bind(middleTop, rightTop);
            rightTop.West  = middleTop.East;

            var id = Guid.NewGuid();

            rightTop.Down = Bind(id);

            for (var i = 0; i < height * 2; i += 4)
            {
                var bottomLeft = Tile("BottomLeft", i);

                if (i == 0)
                {
                    bottomLeft.Up = Bind(id);
                } else
                {
                    bottomLeft.South = Bind(id);
                }

                var bottomRight = Tile("BottomRight", i + 1);
                bottomLeft.East  = Bind(bottomLeft, bottomRight);
                bottomRight.West = bottomLeft.East;
                var topRight = Tile("TopRight", i + 2);

                bottomRight.North = Bind(bottomRight, topRight);
                topRight.South    = bottomRight.North;

                var topLeft = Tile("TopLeft", i + 3);
                topRight.West = Bind(topRight, topLeft);
                topLeft.East  = topRight.West;

                tiles.AddRange(new[] {bottomLeft, bottomRight, topRight, topLeft});
                id = Guid.NewGuid();
                topLeft.North = Bind(id);

            }

            
            var topLeftEnd = new Tile("RightTurnEnd") {
                East  = GlueFactory.RightWall,
                South = Bind(id)
            };

            tiles.Add(topLeftEnd);
        }

    
        private Glue Bind(Tile a, Tile b) => new Glue($"{a.Name} -> {b.Name}");
        private Glue Bind(Guid id) => new Glue(id.ToString());

        private void Bind(Tile a, Glue glueA, Tile b, Glue glueB)
        {

        }

        private Tile Tile(string tileName , int index = -1) => new Tile($"{GadgetName} {tileName} {(index >= 0 ? index.ToString() : string.Empty)}");
    }
}
// 17b2352b-9ba1-4067-abfa-66c304daa4c4