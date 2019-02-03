namespace Tiles.Gadgets.Write
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Models;

    public class Writer
    {
        private readonly string binaryValue;

        private readonly string signal;
        private readonly List<Tile> tiles;
        private readonly Glue startGlue;

        private readonly int stoppingValue;


        public Writer(string binaryValue, string signal, int stoppingValue)
        {
            this.stoppingValue = stoppingValue;
            this.binaryValue = binaryValue;
            this.signal      = signal;
            tiles            = new List<Tile>();
            startGlue        = GlueFactory.Writer(binaryValue, signal);
        }


        public IEnumerable<Tile> Tiles()
        { 
            SetUp();
            return tiles;
        }


        private void SetUp()
        {
            var previous = Guid.NewGuid();
            
            for (var i = 0; i < binaryValue.Length; i++)
            {
                
                var value = binaryValue[i];
                var bit   = value == '0' ? (AbstractBit)new ZeroBit(binaryValue, i, Modes.Copy, signal) : new OneBit(binaryValue, i, Modes.Copy, signal);

               
                // the first tile needs to bind with the hook, according to this binary string's data
                bit.NorthGlue = i == 0 ? startGlue : new Glue(previous.ToString());
                

                // the last tiles should propagate a copy stopper, every other bit should propagate another bit
                previous = Guid.NewGuid();

                var lastBit = i + 1 == binaryValue.Length;

                if (lastBit)
                {
                    var valueBase10 = Convert.ToInt32(binaryValue, 2) + 1;
                    bit.SouthGlue = valueBase10 <= stoppingValue ? GlueFactory.CopyStopper(signal) : new Glue();
                } else
                {
                    bit.SouthGlue = new Glue(previous.ToString());
                }

                List<Tile> results = bit.Tiles().ToList();
                if (value == '1')
                {
                    foreach (var tile in results)
                    {
                        tile.Color = "blue";
                    }
                }

                tiles.AddRange(results);
            }
        }
    }


}
