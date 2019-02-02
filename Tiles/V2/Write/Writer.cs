namespace Tiles.V2.Write
{

    using System;
    using System.Collections.Generic;

    using Models;

    public class Writer
    {
        private readonly string binaryValue;

        private readonly string signal;
        private readonly List<Tile> tiles;
        private readonly Glue startGlue;

        private readonly bool carry;
        public Writer(string binaryValue, string signal)
        {
            this.binaryValue = binaryValue;
            this.signal      = signal;
            tiles            = new List<Tile>();
            startGlue        = Glues.Writer(binaryValue, signal);
            carry = signal == Signals.Carry;
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
                var bit   = value == '0' ? (AbstractBit)new ZeroBit(binaryValue, i, Modes.Copy, carry) : new OneBit(binaryValue, i, Modes.Copy, carry);


                // the first tile needs to bind with the hook, according to this binary string's data
                bit.NorthGlue = i == 0 ? startGlue : new Glue(previous.ToString());
                

                // the last tiles should propagate a copy stopper, every other bit should propagate another bit
                previous = Guid.NewGuid();

                var lastBit = i + 1 == binaryValue.Length;
                
                bit.SouthGlue = lastBit ? Glues.CopyStopper(signal) :  new Glue(previous.ToString());
                

                tiles.AddRange(bit.Tiles());
            }
        }
    }


}
