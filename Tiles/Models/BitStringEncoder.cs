namespace Tiles.Models
{

    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Gadgets;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AbstractGadget" />
    public static class BitStringEncoder
    {


        public static Bit[] Encode(string binaryString, string id = null)
        {
            Validate(binaryString);
            var bits = binaryString.Select(Convert.ToInt32)
                                   .Select((bit, index) => new Bit(bit, index))
                                   .ToArray();

            var first = bits.First();
            var last = bits.Last();

            for (var i = 1; i < bits.Length; i++)
            {
                if (i + 1 < bits.Length)
                {
                    EncodeMiddleBit(bits[i], bits[i - 1], bits[i + 1]);
                }
            }

            return bits;
        }


        private static void EncodeMiddleBit(Bit self, Bit north, Bit south)
        {
            var sameZSouth = self.Value == south.Value;
            var sameZNorth = self.Value == north.Value;
            Guid id;

            // the bit to the north is on the same z index
            if (sameZNorth)
            {
                id = Guid.NewGuid();
                self.TopLeft.North      = new Glue(id.ToString());
                north.BottomLeft.South = new Glue(id.ToString());
            }

            // the bit to the south is on the same z index
            if (sameZSouth)
            {
                id = Guid.NewGuid();
                self.BottomLeft.South = new Glue(id.ToString());
                south.TopLeft.North    = new Glue(id.ToString());
            }


            if (self.Value == 1 && south.Value == 0)
            {
                id = Guid.NewGuid();
                self.BindSouthAndDown(id);
                south.BindNorthUp(id);
            }

            if (self.Value == 0 && south.Value == 1)
            {
                id = Guid.NewGuid();
                self.BindSouthUp(id);
                south.BindNorthAndDown(id);
            }

            if (self.Value == 0 && north.Value == 1)
            {
                id = Guid.NewGuid();
                self.BindNorthUp(id);
                north.BindSouthAndDown(id);
            }

            if (self.Value == 1 && north.Value == 0)
            {
                id = Guid.NewGuid();
                self.BindNorthAndDown(id);
                north.BindSouthUp(id);
            }

        }

        private static readonly Regex Binary = new Regex("^[0-1]{1,}$");

        private static bool InvalidBinary(string value) => !Binary.IsMatch(value);


        private static void Validate(string value)
        {
            if (InvalidBinary(value))
            {
                throw new Exception($"Invalid bit string, expected all 0's or 1's, but got {value}");
            }

        }
        
    }

}
