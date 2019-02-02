namespace Tiles.V2.Write
{

    using System.Collections.Generic;

    using Models;

    public class CopyStopper
    {

        public CopyStopper(string signal)
        {
            this.signal = signal;
        }

        private Tile TopRight;
        private Tile TopLeft;
        private Tile BottomLeft;
        private Tile BottomRight;


        private readonly string signal;

        private List<Tile> tiles; 

        public IEnumerable<Tile> Tiles()
        {
            SetUp();

            return tiles;
        }


        private void SetUp()
        {
            TopRight         = Tile(TileNames.CopyStopperTopRight);
            TopLeft          = Tile(TileNames.CopyStopperTopLeft);
            BottomLeft       = Tile(TileNames.CopyStopperBottomLeft);
            BottomRight      = Tile(TileNames.CopyStopperBottomRight);
                             
            TopRight.North   = Glues.CopyStopper(signal);
                             
            TopRight.West    = Bind(TopRight, TopLeft);
            TopLeft.East     = TopRight.West;

            TopLeft.South    = Bind(TopLeft, BottomLeft);
            BottomLeft.North = TopLeft.South;

            BottomLeft.East  = Bind(BottomLeft, BottomRight);
            BottomRight.West = BottomLeft.East;

            BottomRight.South = Glues.Reader(signal);
            tiles             = new List<Tile> {TopRight, TopLeft, BottomLeft, BottomRight};
        }

        private static Glue Bind(Tile tileA, Tile tileB) => new Glue($"{tileA.Name} -> {tileB.Name}");

        private Tile Tile(string name) => new Tile($"{name}, S={signal} ");
    }
}
