namespace Tiles.Gadgets.Seed
{

    using System.Collections.Generic;

    using Models;

    public class CopyStopper
    {

        public CopyStopper(string signal)
        {
            this.signal = signal;
        }

        private Tile topRight;
        private Tile topLeft;
        private Tile bottomLeft;
        private Tile bottomRight;


        private readonly string signal;

        private List<Tile> tiles; 

        public IEnumerable<Tile> Tiles()
        {
            SetUp();

            return tiles;
        }


        private void SetUp()
        {
            topRight         = Tile(TileNames.CopyStopperTopRight);
            topLeft          = Tile(TileNames.CopyStopperTopLeft);
            bottomLeft       = Tile(TileNames.CopyStopperBottomLeft);
            bottomRight      = Tile(TileNames.CopyStopperBottomRight);
                             
            topRight.North   = GlueFactory.CopyStopper(signal);
                             
            topRight.West    = Bind(topRight, topLeft);
            topLeft.East     = topRight.West;

            topLeft.South    = Bind(topLeft, bottomLeft);
            bottomLeft.North = topLeft.South;

            bottomLeft.East  = Bind(bottomLeft, bottomRight);
            bottomRight.West = bottomLeft.East;

            bottomRight.South = GlueFactory.PreReadRight(signal);
            tiles             = new List<Tile> {topRight, topLeft, bottomLeft, bottomRight};
        }

        private static Glue Bind(Tile tileA, Tile tileB) => new Glue($"{tileA.Name} -> {tileB.Name}");

        private Tile Tile(string name) => new Tile($"{name}, S={signal} ");
    }
}
