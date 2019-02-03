namespace Tiles.V2.Read
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Models;

    public class RightReader
    {
        private readonly string Signal;

        private readonly string BitsRead;

        private readonly int TotalBits;

        private List<Tile> tiles;

        private Tile FirstZ0;

        private Tile SecondZ0;

        private Tile ThirdZ0;

        private Tile FourthZ0;



        private Tile FirstZ1;

        private Tile SecondZ1;

        private Tile ThirdZ1;

        private Tile FourthZ1;

        private Tile FifthZ1;


        public RightReader(string bitsRead, string signal, int totalBits)
        {
            BitsRead  = bitsRead;
            Signal    = signal;
            TotalBits = totalBits;
        }


        public IEnumerable<Tile> Tiles()
        {
            Setup();

            return tiles;
        }


        private void Setup()
        {
            FirstZ0  = Tile($"R: Z0-1st, B='{BitsRead}', S={Signal}");
            SecondZ0 = Tile($"R: Z0-2nd, B='{BitsRead}1', S={Signal}");
            ThirdZ0  = Tile($"R: Z0-3rd, B='{BitsRead}1', S={Signal}");
            FourthZ0 = Tile($"R: Z0-4th, B='{BitsRead}1', S={Signal}");


            FirstZ1  = Tile($"R: Z1-1st, B='{BitsRead}', S={Signal}");
            SecondZ1 = Tile($"R: Z1-2nd, B='{BitsRead}0', S={Signal}");
            ThirdZ1  = Tile($"R: Z1-3rd, B='{BitsRead}0', S={Signal}");
            FourthZ1 = Tile($"R: Z1-4th, B='{BitsRead}0', S={Signal}");
            FifthZ1  = Tile($"R: Z1-5th, B='{BitsRead}0', S={Signal}");

            tiles = new List<Tile> {FirstZ0, SecondZ0, ThirdZ0, FourthZ0, FirstZ1, SecondZ1, ThirdZ1, FourthZ1, FifthZ1};

            if (BitsRead.Length == 0)
            {
                FirstZ0.East = Glues.Reader(Signal);
            } else
            {
                FirstZ0.South = new Glue($"ReadContinue: {BitsRead}, S={Signal}");
            }

            FirstZ0.North  = Bind(FirstZ0, SecondZ0);
            SecondZ0.South = FirstZ0.North;

            SecondZ0.North = Bind(SecondZ0, ThirdZ0);
            ThirdZ0.South  = SecondZ0.North;

            ThirdZ0.North  = Bind(ThirdZ0, FourthZ0);
            FourthZ0.South = ThirdZ0.North;

            FirstZ0.Up = FourthZ0.Down;

            FirstZ0.Up   = Bind(FirstZ0, FirstZ1);
            FirstZ1.Down = FirstZ0.Up;

            FirstZ1.North  = Bind(FirstZ1, SecondZ1);
            SecondZ1.South = FirstZ1.North;

            SecondZ1.North = Bind(SecondZ1, ThirdZ1);
            ThirdZ1.South  = SecondZ1.North;

            ThirdZ1.North = Bind(ThirdZ1, FourthZ1);

            FourthZ1.South = ThirdZ1.North;
            FourthZ1.Down  = Bind(FourthZ1, FifthZ1);
            FifthZ1.Up     = FourthZ1.Down;

            // final bit
            if (BitsRead.Length + 1 == TotalBits)
            {
                FourthZ0.North = new Glue($"ReadContinue: {BitsRead}1, S={Signal}");
                FifthZ1.North  = new Glue($"ReadContinue: {BitsRead}0, S={Signal}");
                var currentValueForFifthZ1  = $"{BitsRead}0";
                var currentValueForFourthZ0 = $"{BitsRead}1";

                var possibleToIncrement = BitsRead.Contains("0");
                var currentCarrySignal  = Signal == Signals.Carry;

                if (currentCarrySignal)
                {

                    if (possibleToIncrement)
                    {
                        var valueToCreateFifthZ1 = IncrementInBinary(currentValueForFifthZ1);
                        FifthZ1.North = Glues.Hook(valueToCreateFifthZ1, Signals.NoCarry);
                        FifthZ1.Color = "red";

                        var valueToCreateFourthZ0 = IncrementInBinary(currentValueForFourthZ0);
                        FourthZ0.North = Glues.Hook(valueToCreateFourthZ0, Signals.NoCarry);

                    } else
                    {
                        var allZeros = string.Concat(Enumerable.Repeat("0", TotalBits));
                        FourthZ0.North = Glues.Hook(allZeros, Signals.Carry);
                        FifthZ1.North  = Glues.Hook(allZeros, Signals.Carry);
                        
                    }


                } else
                {
                    FourthZ0.North = Glues.Hook(currentValueForFourthZ0, Signals.NoCarry);
                    FifthZ1.North  = Glues.Hook(currentValueForFifthZ1,  Signals.NoCarry);
                }


            } else
            {
                FourthZ0.North = new Glue($"ReadContinue: {BitsRead}1, S={Signal}");
                FifthZ1.North  = new Glue($"ReadContinue: {BitsRead}0, S={Signal}");
            }
        }


        private Glue Bind(Tile a, Tile b) => new Glue($"{a.Name} -> {b.Name}, {BitsRead}, S={Signal} ");


        private Tile Tile(string name) => new Tile(name);


        private string IncrementInBinary(string currentValue)
        {

            var value = Convert.ToInt32(currentValue, 2);

            var incremented = value + 1;

            var incrementedInBinary = Convert.ToString(incremented, 2);

            if (incrementedInBinary.Length == currentValue.Length)
            {
                return incrementedInBinary;
            }

            var droppedZeroes = currentValue.Length - incrementedInBinary.Length;

            var result = incrementedInBinary.PadLeft(droppedZeroes + incrementedInBinary.Length, '0');
            return result;

        }
    }
}
