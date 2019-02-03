namespace Tiles.Gadgets.Write
{

    using System;
    using System.Collections.Generic;

    using Models;

    public class Copier
    {

        private readonly string binaryValue;

        private readonly string signal;
        private readonly List<Tile> tiles;
        private readonly Glue startGlue;

        public Copier(string binaryValue, string signal)
        {
            this.binaryValue = binaryValue;
            this.signal      = signal;
            tiles            = new List<Tile>();
            startGlue        = GlueFactory.LeftHook();

        }


        public IEnumerable<Tile> Tiles()
        {
            SetUp();
            return tiles;
        } 

        private void SetUp()
        {
            var previous = Guid.NewGuid();
            var results = new List<Tile>();

            for (var i = 0; i < binaryValue.Length; i++)
            {
                var bit = binaryValue[i] == '0' ? (AbstractBit)new ZeroBit(binaryValue, i, Modes.Copy, signal) : new OneBit(binaryValue, i, Modes.Copy, signal);

                if (i == 0)
                {
                    bit.SouthGlue = GlueFactory.Copier(binaryValue, signal);
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


                results.AddRange(bit.Tiles());
            }

            tiles.AddRange(results);
        }

    }
}
