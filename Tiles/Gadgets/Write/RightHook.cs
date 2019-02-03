namespace Tiles.Gadgets.Write
{

    using System;
    using System.Collections.Generic;

    using Models;

    public class RightHook
    {

        private readonly string binaryStringValue;

        private readonly string signal;

        private List<Tile> tiles;

        private readonly int Height;

        private Tile HookBottomLeftUpperZ0;

        private Tile HookBottomLeftUpperZ1;

        private Tile HookBottomMiddleUpperZ0;

        private Tile HookBottomMiddleUpperZ1;

        private Tile HookBottomRightLowerZ0;

        private Tile HookBottomRightUpperZ0;

        private Tile HookBottomRightUpperZ1;

        private Tile HookTopLeftZ0;

        private Tile HookTopLeftZ1;

        private Tile HookTopMiddleLeftZ1;

        private Tile HookTopMiddleRightZ1;

        private Tile HookTopRightZ1;


        public RightHook(string binaryStringValue, int height, bool carry)
        {
            Height                 = height;
            this.binaryStringValue = binaryStringValue;
            signal                 = carry ? Signals.Carry : Signals.NoCarry;
            NorthGlue              = GlueFactory.Writer(binaryStringValue, signal);
            SouthGlue              = GlueFactory.RightHook(binaryStringValue, signal);
        }


        private Glue SouthGlue { get; }
        public Glue NorthGlue { private get; set; }


        public IEnumerable<Tile> Tiles()
        {
            SetUp();

            return tiles;
        }


        public void AttachTop(Glue glue)
        {
            HookTopLeftZ0.South = glue;
        }


        public void AttachEast(Glue glue)
        {
            HookBottomRightUpperZ0.East = glue;
        }

        private string Content      => $"V={binaryStringValue}, I=Hook, S={signal}";
        private string Index(int n) => $"V={binaryStringValue}, I={n}, S={signal}";

        private void SetUp()
        {
            InitDefaultCommonTiles();

        }


        private void InitDefaultCommonTiles()
        {

            HookBottomRightLowerZ0  = Tile(TileNames.HookBottomRightLowerZ0);
            HookBottomRightUpperZ0  = Tile(TileNames.HookBottomRightUpperZ0);
            HookBottomMiddleUpperZ0 = Tile(TileNames.HookBottomMiddleUpperZ0);
            HookBottomLeftUpperZ0   = Tile(TileNames.HookBottomLeftUpperZ0);
            HookBottomLeftUpperZ1   = Tile(TileNames.HookBottomLeftUpperZ1);
            HookBottomMiddleUpperZ1 = Tile(TileNames.HookBottomMiddleUpperZ1);
            HookBottomRightUpperZ1  = Tile(TileNames.HookBottomRightUpperZ1);
            HookTopRightZ1          = Tile(TileNames.HookTopRightZ1);
            HookTopMiddleRightZ1    = Tile(TileNames.HookTopMiddleRightZ1);
            HookTopMiddleLeftZ1     = Tile(TileNames.HookTopMiddleLeftZ1);
            HookTopLeftZ1           = Tile(TileNames.HookTopLeftZ1);
            HookTopLeftZ0           = Tile(TileNames.HookTopLeftZ0);

            tiles = new List<Tile>{
                HookBottomRightLowerZ0,
                HookBottomRightUpperZ0,
                HookBottomMiddleUpperZ0,
                HookBottomLeftUpperZ0,
                HookBottomLeftUpperZ1,
                HookBottomMiddleUpperZ1,
                HookBottomRightUpperZ1,
                HookTopRightZ1,
                HookTopMiddleRightZ1,
                HookTopMiddleLeftZ1,
                HookTopLeftZ1,
                HookTopLeftZ0
            };

            HookBottomRightLowerZ0.South = SouthGlue; 
            HookBottomRightLowerZ0.North = Bind(HookBottomRightLowerZ0, HookBottomRightUpperZ0);
            HookBottomRightUpperZ0.South = HookBottomRightLowerZ0.North;

            HookBottomRightUpperZ0.West  = Bind(HookBottomRightUpperZ0, HookBottomMiddleUpperZ0);
            HookBottomMiddleUpperZ0.East = HookBottomRightUpperZ0.West;

            HookBottomMiddleUpperZ0.West = Bind(HookBottomMiddleUpperZ0, HookBottomLeftUpperZ0);
            HookBottomLeftUpperZ0.East   = HookBottomMiddleUpperZ0.West;

            HookBottomLeftUpperZ0.Up     = Bind(HookBottomLeftUpperZ0, HookBottomLeftUpperZ1);
            HookBottomLeftUpperZ1.Down   = HookBottomLeftUpperZ0.Up;

            HookBottomLeftUpperZ1.East   = Bind(HookBottomLeftUpperZ1, HookBottomMiddleUpperZ1);
            HookBottomMiddleUpperZ1.West = HookBottomLeftUpperZ1.East;

            HookBottomMiddleUpperZ1.East = Bind(HookBottomMiddleUpperZ1, HookBottomRightUpperZ1);
            HookBottomRightUpperZ1.West  = HookBottomMiddleUpperZ1.East;

            HookTopRightZ1.West          = Bind(HookTopRightZ1, HookTopMiddleRightZ1);
            HookTopMiddleRightZ1.East    = HookTopRightZ1.West;
                                         
            HookTopMiddleRightZ1.West    = Bind(HookTopMiddleRightZ1, HookTopMiddleLeftZ1);
            HookTopMiddleLeftZ1.East     = HookTopMiddleRightZ1.West;
                                         
            HookTopMiddleLeftZ1.West     = Bind(HookTopMiddleLeftZ1, HookTopLeftZ1);
            HookTopLeftZ1.East           = HookTopMiddleLeftZ1.West;
                                         
            HookTopLeftZ1.Down           = Bind(HookTopLeftZ1, HookTopLeftZ0);
            HookTopLeftZ0.Up             = HookTopLeftZ1.Down;
            HookTopLeftZ0.South          = NorthGlue;

            var previous = Guid.NewGuid();
            HookBottomRightUpperZ1.North = new Glue(previous.ToString());

            for (var i = 0; i < Height; i++)
            {
                var tile = Tile(Index(i));
                tile.South = new Glue(previous.ToString());
                previous = Guid.NewGuid();

                tile.North = new Glue(previous.ToString());
                tiles.Add(tile);
            }

            HookTopRightZ1.South  = new Glue(previous.ToString());

        }



        protected Glue Bind(Tile tileA, Tile tileB) => new Glue($"{tileA.Name} -> {tileB.Name} {signal}");


        protected Tile Tile(string name) => new Tile($"{name}, {Content}");

    }

}
