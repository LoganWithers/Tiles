namespace Tiles.V2.Write
{

    using System.Collections.Generic;

    using Models;


    public class ZeroBit : AbstractBit {

        public ZeroBit(string binaryStringValue, int index, string direction, string signal) : base(binaryStringValue, index, direction, signal) { }


        private void SetGlues()
        {
            tiles = new List<Tile> { UpperOuter, UpperInner, BumpTopRight, BumpBottomRight, LowerInner, LowerOuter };

            UpperOuter.North      = NorthGlue;
            LowerOuter.South      = SouthGlue;
            
            UpperOuter.South      = Bind(UpperOuter, UpperInner);
            UpperInner.North      = UpperOuter.South;
                                  
            UpperInner.East       = Bind(UpperInner, BumpTopRight);
            BumpTopRight.West     = UpperInner.East;

            BumpTopRight.South    = Bind(BumpTopRight, BumpBottomRight);
            BumpBottomRight.North = BumpTopRight.South;

            BumpBottomRight.West  = Bind(BumpBottomRight, LowerInner);
            LowerInner.East       = BumpBottomRight.West;
                                  
            LowerInner.South      = Bind(LowerInner, LowerOuter);
            LowerOuter.North      = LowerInner.South;
        }

        public override IEnumerable<Tile> Tiles()
        {
            SetGlues();

            return tiles;
        }
    }

}
