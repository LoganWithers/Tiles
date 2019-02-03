namespace Tiles.Gadgets.Write
{

    using System;
    using System.Collections.Generic;

    using Models;

    public class LeftHook
    {

        private readonly string binaryStringValue;

        private readonly string signal;

        private List<Tile> tiles;

        private readonly int height;

        private Tile hookBottomLeftUpperZ0;

        private Tile hookBottomLeftUpperZ1;

        private Tile hookBottomMiddleUpperZ0;

        private Tile hookBottomMiddleUpperZ1;

        private Tile hookBottomRightLowerZ0;

        private Tile hookBottomRightUpperZ0;

        private Tile hookBottomRightUpperZ1;

        private Tile hookTopLeftZ0;

        private Tile hookTopLeftZ1;

        private Tile hookTopMiddleLeftZ1;

        private Tile hookTopMiddleRightZ1;

        private Tile hookTopRightZ1;


        public LeftHook(string binaryStringValue, int height)
        {
            this.height            = height * 4;
            this.binaryStringValue = binaryStringValue;
            
            signal                 = Signals.Nth;
            NorthGlue              = GlueFactory.LeftHook();
            SouthGlue              = GlueFactory.PreReadLeft(signal);
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
            hookTopLeftZ0.South = glue;
        }


        public void AttachEast(Glue glue)
        {
            hookBottomRightUpperZ0.East = glue;
        }

        private string Content      => $"V={binaryStringValue}, I=Hook, S={signal}";
        private string Index(int n) => $"V={binaryStringValue}, I={n}, S={signal}";

        private void SetUp()
        {
            InitDefaultCommonTiles();

        }


        private void InitDefaultCommonTiles()
        {

            hookBottomRightLowerZ0  = Tile(TileNames.HookBottomRightLowerZ0);
            hookBottomRightUpperZ0  = Tile(TileNames.HookBottomRightUpperZ0);
            hookBottomMiddleUpperZ0 = Tile(TileNames.HookBottomMiddleUpperZ0);
            hookBottomLeftUpperZ0   = Tile(TileNames.HookBottomLeftUpperZ0);
            hookBottomLeftUpperZ1   = Tile(TileNames.HookBottomLeftUpperZ1);
            hookBottomMiddleUpperZ1 = Tile(TileNames.HookBottomMiddleUpperZ1);
            hookBottomRightUpperZ1  = Tile(TileNames.HookBottomRightUpperZ1);
            hookTopRightZ1          = Tile(TileNames.HookTopRightZ1);
            hookTopMiddleRightZ1    = Tile(TileNames.HookTopMiddleRightZ1);
            hookTopMiddleLeftZ1     = Tile(TileNames.HookTopMiddleLeftZ1);
            hookTopLeftZ1           = Tile(TileNames.HookTopLeftZ1);
            hookTopLeftZ0           = Tile(TileNames.HookTopLeftZ0);

            tiles = new List<Tile>{
                hookBottomRightLowerZ0,
                hookBottomRightUpperZ0,
                hookBottomMiddleUpperZ0,
                hookBottomLeftUpperZ0,
                hookBottomLeftUpperZ1,
                hookBottomMiddleUpperZ1,
                hookBottomRightUpperZ1,
                hookTopRightZ1,
                hookTopMiddleRightZ1,
                hookTopMiddleLeftZ1,
                hookTopLeftZ1,
                hookTopLeftZ0
            };

            hookBottomRightLowerZ0.South = SouthGlue; 
            hookBottomRightLowerZ0.North = Bind(hookBottomRightLowerZ0, hookBottomRightUpperZ0);
            hookBottomRightUpperZ0.South = hookBottomRightLowerZ0.North;

            hookBottomRightUpperZ0.West  = Bind(hookBottomRightUpperZ0, hookBottomMiddleUpperZ0);
            hookBottomMiddleUpperZ0.East = hookBottomRightUpperZ0.West;
            hookBottomMiddleUpperZ0.West = Bind(hookBottomMiddleUpperZ0, hookBottomLeftUpperZ0);
            hookBottomLeftUpperZ0.East   = hookBottomMiddleUpperZ0.West;

            hookBottomLeftUpperZ0.Up     = Bind(hookBottomLeftUpperZ0, hookBottomLeftUpperZ1);
            hookBottomLeftUpperZ1.Down   = hookBottomLeftUpperZ0.Up;
            

            hookBottomLeftUpperZ1.East   = Bind(hookBottomLeftUpperZ1, hookBottomMiddleUpperZ1);
            hookBottomMiddleUpperZ1.West = hookBottomLeftUpperZ1.East;

            hookBottomMiddleUpperZ1.East = Bind(hookBottomMiddleUpperZ1, hookBottomRightUpperZ1);
            hookBottomRightUpperZ1.West  = hookBottomMiddleUpperZ1.East;

            hookTopRightZ1.West          = Bind(hookTopRightZ1, hookTopMiddleRightZ1);
            hookTopMiddleRightZ1.East    = hookTopRightZ1.West;
                                         
            hookTopMiddleRightZ1.West    = Bind(hookTopMiddleRightZ1, hookTopMiddleLeftZ1);
            hookTopMiddleLeftZ1.East     = hookTopMiddleRightZ1.West;
                                         
            hookTopMiddleLeftZ1.West     = Bind(hookTopMiddleLeftZ1, hookTopLeftZ1);
            hookTopLeftZ1.East           = hookTopMiddleLeftZ1.West;
                                         
            hookTopLeftZ1.Down           = Bind(hookTopLeftZ1, hookTopLeftZ0);
            hookTopLeftZ0.Up             = hookTopLeftZ1.Down;
            hookTopLeftZ0.South          = NorthGlue;

            var previous = Guid.NewGuid();
            hookBottomRightUpperZ1.North = new Glue(previous.ToString());

            for (var i = 0; i < height; i++)
            {
                var tile = Tile(Index(i));
                tile.South = new Glue(previous.ToString());
                previous = Guid.NewGuid();

                tile.North = new Glue(previous.ToString());
                tiles.Add(tile);
            }

            hookTopRightZ1.South  = new Glue(previous.ToString());

        }



        protected Glue Bind(Tile tileA, Tile tileB) => new Glue($"{tileA.Name} -> {tileB.Name} {signal}");


        protected Tile Tile(string name) => new Tile($"{name}, {Content}");

    }

}
