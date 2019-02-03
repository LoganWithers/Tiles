namespace Tiles.Gadgets.Read
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Models;

    public class RightReader
    {
        private readonly string signal;

        private readonly string bitsRead;

        private readonly int totalBits;

        private List<Tile> tiles;

        private Tile firstZ0;

        private Tile secondZ0;

        private Tile thirdZ0;

        private Tile fourthZ0;
        

        private Tile firstZ1;

        private Tile secondZ1;

        private Tile thirdZ1;

        private Tile fourthZ1;

        private Tile fifthZ1;

        private readonly int baseK;

        public RightReader(string bitsRead, string signal, int totalBits, int baseK)
        {
            this.bitsRead  = bitsRead;
            this.signal    = signal;
            this.baseK     = baseK;
            this.totalBits = totalBits;
        }


        public IEnumerable<Tile> Tiles()
        {
            Setup();

            return tiles;
        }


        private void Setup()
        {
            firstZ0  = Tile($"R: Z0-1st, B='{bitsRead}', S={signal}");
            secondZ0 = Tile($"R: Z0-2nd, B='{bitsRead}1', S={signal}");
            thirdZ0  = Tile($"R: Z0-3rd, B='{bitsRead}1', S={signal}");
            fourthZ0 = Tile($"R: Z0-4th, B='{bitsRead}1', S={signal}");


            firstZ1  = Tile($"R: Z1-1st, B='{bitsRead}', S={signal}");
            secondZ1 = Tile($"R: Z1-2nd, B='{bitsRead}0', S={signal}");
            thirdZ1  = Tile($"R: Z1-3rd, B='{bitsRead}0', S={signal}");
            fourthZ1 = Tile($"R: Z1-4th, B='{bitsRead}0', S={signal}");
            fifthZ1  = Tile($"R: Z1-5th, B='{bitsRead}0', S={signal}");

            tiles = new List<Tile> {firstZ0, secondZ0, thirdZ0, fourthZ0, firstZ1, secondZ1, thirdZ1, fourthZ1, fifthZ1};

            if (bitsRead.Length == 0)
            {
                firstZ0.East = GlueFactory.Reader(signal);
            } else
            {
                firstZ0.South = new Glue($"ReadContinue: {bitsRead}, S={signal}");
            }

            firstZ0.North  = Bind(firstZ0, secondZ0);
            secondZ0.South = firstZ0.North;

            secondZ0.North = Bind(secondZ0, thirdZ0);
            thirdZ0.South  = secondZ0.North;

            thirdZ0.North  = Bind(thirdZ0, fourthZ0);
            fourthZ0.South = thirdZ0.North;

            firstZ0.Up = fourthZ0.Down;

            firstZ0.Up   = Bind(firstZ0, firstZ1);
            firstZ1.Down = firstZ0.Up;

            firstZ1.North  = Bind(firstZ1, secondZ1);
            secondZ1.South = firstZ1.North;

            secondZ1.North = Bind(secondZ1, thirdZ1);
            thirdZ1.South  = secondZ1.North;

            thirdZ1.North = Bind(thirdZ1, fourthZ1);

            fourthZ1.South = thirdZ1.North;
            fourthZ1.Down  = Bind(fourthZ1, fifthZ1);
            fifthZ1.Up     = fourthZ1.Down;


            // final bit
            if (bitsRead.Length + 1 == totalBits)
            {
                fourthZ0.North = new Glue($"ReadContinue: {bitsRead}1, S={signal}");
                fifthZ1.North  = new Glue($"ReadContinue: {bitsRead}0, S={signal}");


                var currentCarrySignal = signal == Signals.Carry;

                if (currentCarrySignal)
                {
                    var value = $"{bitsRead}0";
                    var inDecimal = Convert.ToInt32(value, 2);
                    var canIncrementIfLastBitWas0 = inDecimal < baseK - 1;
                    if (canIncrementIfLastBitWas0)
                    {
                        var valueToCreateFifthZ1 = IncrementInBinary(value);
                        fifthZ1.North = GlueFactory.RightHook(valueToCreateFifthZ1, Signals.NoCarry);
                    } else
                    {
                        var allZeros = string.Concat(Enumerable.Repeat("0", totalBits));

                        fifthZ1.North = GlueFactory.RightHook(allZeros, Signals.Carry);
                    }
                    
                    
                    value = $"{bitsRead}1";
                    var canIncrementIfLastBitWas1 = Convert.ToInt32($"{bitsRead}1", 2) < baseK - 1;

                    
                    if (canIncrementIfLastBitWas1)
                    {
                        var valueToCreateFourthZ0 = IncrementInBinary(value);
                        fourthZ0.North = GlueFactory.RightHook(valueToCreateFourthZ0, Signals.NoCarry);
                    } else
                    {

                        var allZeros = string.Concat(Enumerable.Repeat("0", totalBits));
                        fourthZ0.North = GlueFactory.RightHook(allZeros, Signals.Carry);
                    }
                }
                else
                {
                    fourthZ0.North = GlueFactory.RightHook($"{bitsRead}1", Signals.NoCarry);
                    fifthZ1.North = GlueFactory.RightHook($"{bitsRead}0", Signals.NoCarry);
                }


            }
            else {
                fourthZ0.North = new Glue($"ReadContinue: {bitsRead}1, S={signal}");
                fifthZ1.North  = new Glue($"ReadContinue: {bitsRead}0, S={signal}");
            }

        }


        private Glue Bind(Tile a, Tile b) => new Glue($"{a.Name} -> {b.Name}, {bitsRead}, S={signal} ");


        private static Tile Tile(string name) => new Tile(name);


        private static string IncrementInBinary(string currentValue)
        {
            var value = Convert.ToInt32(currentValue, 2);

            var incremented = value + 1;

            var incrementedInBinary = Convert.ToString(incremented, 2);

            if (incrementedInBinary.Length == currentValue.Length)
            {
                return incrementedInBinary;
            }

            var droppedZeroes = currentValue.Length - incrementedInBinary.Length;

            return incrementedInBinary.PadLeft(droppedZeroes + incrementedInBinary.Length, '0');
        }
    }
}
