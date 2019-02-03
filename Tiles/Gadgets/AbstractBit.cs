namespace Tiles.Gadgets {

    using System.Collections.Generic;

    using Models;

    public abstract class AbstractBit
    {
        public Glue NorthGlue { protected get; set; }
        public Glue SouthGlue { protected get; set; }

        public abstract IEnumerable<Tile> Tiles();

        protected readonly Tile UpperOuter;
        protected readonly Tile UpperInner;

        protected readonly Tile BumpTopRight;
        protected readonly Tile BumpBottomRight;

        protected readonly Tile LowerInner;
        protected readonly Tile LowerOuter;
        
        protected List<Tile> tiles;

        private readonly int index;
        private readonly string binaryStringValue;
        private readonly string signal;

        private readonly string mode;

        private readonly string value;
        protected AbstractBit(string binaryStringValue, int index, string mode, string signal)
        {
            this.index             = index;
            this.binaryStringValue = binaryStringValue;
            this.mode              = mode;
            this.signal            = signal;

            UpperOuter             = Tile(TileNames.UpperOuter);
            UpperInner             = Tile(TileNames.UpperInner);
                                     
            BumpTopRight           = Tile(TileNames.BumpTopRight);
            BumpBottomRight        = Tile(TileNames.BumpBottomRight);
                                     
            LowerInner             = Tile(TileNames.LowerInner);
            LowerOuter             = Tile(TileNames.LowerOuter);

            value = binaryStringValue.Substring(0, index);

        }


        public void SetNorth(string color) => UpperOuter.Color = color;

        protected Glue Bind(Tile tileA, Tile tileB) => new Glue($"{tileA.Name} -> {tileB.Name}, S={signal}");
        
        private string Content => $"V={value}, I={index}, S={signal}";

        protected Tile Tile(string name) => new Tile($"{name}, {mode}, B={binaryStringValue}, {Content}");
    }

}