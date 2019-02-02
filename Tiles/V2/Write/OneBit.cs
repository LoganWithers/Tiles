namespace Tiles.V2.Write
{

    using System.Collections.Generic;

    using Models;

    public class OneBit : AbstractBit
    {
        private Tile bumpTopLeft;
        private Tile bumpBottomLeft;


        public OneBit(string binaryStringValue, int index, string direction, bool carry = true) : base(binaryStringValue, index, direction, carry){}


        private void SetGlues()
        {
            bumpTopLeft    = Tile(TileNames.BumpTopLeft);
            bumpBottomLeft = Tile(TileNames.BumpBottomLeft);

            tiles = new List<Tile> { UpperOuter, UpperInner, bumpTopLeft, BumpTopRight, BumpBottomRight, bumpBottomLeft, LowerInner, LowerOuter };

            UpperOuter.North = NorthGlue;
            LowerOuter.South = SouthGlue;
                                  
            UpperOuter.South = Bind(UpperOuter, UpperInner);
            UpperInner.North = UpperOuter.South;
                                  
            UpperInner.Up    = Bind(UpperInner, bumpTopLeft);
            bumpTopLeft.Down = UpperInner.Up;

            bumpTopLeft.East  = Bind(bumpTopLeft, BumpTopRight);
            BumpTopRight.West = bumpTopLeft.East;
            
            BumpTopRight.South    = Bind(BumpTopRight, BumpBottomRight);
            BumpBottomRight.North = BumpTopRight.South;

            BumpBottomRight.West = Bind(BumpBottomRight, bumpBottomLeft);
            bumpBottomLeft.East  = BumpBottomRight.West;

            bumpBottomLeft.Down = Bind(bumpBottomLeft, LowerInner);
            LowerInner.Up       = bumpBottomLeft.Down;

            LowerInner.South = Bind(LowerInner, LowerOuter);
            LowerOuter.North = LowerInner.South;

        }


        public override IEnumerable<Tile> Tiles()
        {
            SetGlues();

            return tiles;
        }
    }

}