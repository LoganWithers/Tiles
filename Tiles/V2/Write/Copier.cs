namespace Tiles.V2.Write
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Models;

    public class Copier
    {

        private readonly string binaryValue;

        private readonly string signal;
        private readonly List<Tile> tiles;
        private readonly Glue startGlue;

        public Copier(string binaryValue, string signal)
        {
            this.binaryValue = Reverse(binaryValue);
            this.signal      = signal;
            tiles            = new List<Tile>();
            startGlue        = Glues.Hook(binaryValue, signal);

        }


        public IEnumerable<Tile> Tiles() => SetUp();

        // I=0: UpperOuter "61ba30d0-bd53-435e-a25f-5a0d93a6e936"
        // I=1: LowerOuter:
        // 
        // UpperOuter, Copy, B=0000, V=, I=0, S=First
        private IEnumerable<Tile> SetUp()
        {
            var previous = Guid.NewGuid();
            var results = new List<Tile>();

            for (var i = 0; i < binaryValue.Length; i++)
            {
                var bit = binaryValue[i] == '0' ? (AbstractBit)new ZeroBit(binaryValue, i, Modes.Copy, signal) : new OneBit(binaryValue, i, Modes.Copy, signal);

                if (i == 0)
                {
                    bit.SouthGlue = Glues.Copier(Reverse(binaryValue), signal);
                } else
                {
                    bit.SouthGlue = new Glue(previous.ToString());
                }

                if (i + 1 == binaryValue.Length)
                {
                    bit.NorthGlue = startGlue;
                } else
                {
                    previous = Guid.NewGuid();
                    bit.NorthGlue = new Glue(previous.ToString());
                }

             
                if (i + 1 == binaryValue.Length)
                {
                    bit.SetNorth("red");
                }  

                results.AddRange(bit.Tiles());
            }

            return results;
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
