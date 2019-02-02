namespace Tiles.V2.Write {

    using System.Collections.Generic;

    using Models;

    public abstract class AbstractBit
    {
        public Glue NorthGlue { get; set; }
        public Glue SouthGlue { get; set; }

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

        private string Value;
        protected AbstractBit(string binaryStringValue, int index, string mode, bool carry)
        {
            this.index             = index;
            this.binaryStringValue = binaryStringValue;
            this.mode              = mode;
            signal                 = carry ? Signals.Carry : Signals.NoCarry;

            UpperOuter             = Tile(TileNames.UpperOuter);
            UpperInner             = Tile(TileNames.UpperInner);
                                     
            BumpTopRight           = Tile(TileNames.BumpTopRight);
            BumpBottomRight        = Tile(TileNames.BumpBottomRight);
                                     
            LowerInner             = Tile(TileNames.LowerInner);
            LowerOuter             = Tile(TileNames.LowerOuter);

            Value = binaryStringValue.Substring(0, index);

        }


        protected Glue Bind(Tile tileA, Tile tileB) => new Glue($"{tileA.Name} -> {tileB.Name}, S={signal}");
        
        private string Content => $"V={Value}, I={index}, S={signal}";

        protected Tile Tile(string name) => new Tile($"{name}, {mode}, B={binaryStringValue}, {Content}");
    }

}