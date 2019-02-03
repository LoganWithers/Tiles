namespace Tiles.V2.Read
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Models;

    public class LeftReader
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


        public LeftReader(string bitsRead, string signal, int totalBits)
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
            FirstZ0  = Tile($"LR: Z0-1st, B='{BitsRead}', S={Signal}");
            SecondZ0 = Tile($"LR: Z0-2nd, B='{BitsRead}1', S={Signal}");
            ThirdZ0  = Tile($"LR: Z0-3rd, B='{BitsRead}1', S={Signal}");
            FourthZ0 = Tile($"LR: Z0-4th, B='{BitsRead}1', S={Signal}");


            FirstZ1  = Tile($"LR: Z1-1st, B='{BitsRead}', S={Signal}");
            SecondZ1 = Tile($"LR: Z1-2nd, B='{BitsRead}0', S={Signal}");
            ThirdZ1  = Tile($"LR: Z1-3rd, B='{BitsRead}0', S={Signal}");
            FourthZ1 = Tile($"LR: Z1-4th, B='{BitsRead}0', S={Signal}");
            FifthZ1  = Tile($"LR: Z1-5th, B='{BitsRead}0', S={Signal}");

            tiles = new List<Tile> {FirstZ0, SecondZ0, ThirdZ0, FourthZ0, FirstZ1, SecondZ1, ThirdZ1, FourthZ1, FifthZ1};

            if (BitsRead.Length == 0)
            {
                FirstZ1.West = Glues.Reader(Signal);
            } else
            {
                FirstZ0.South = new Glue($"LR: ReadContinue: {BitsRead}, S={Signal}");
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
                var currentValueForFifthZ1  = $"{BitsRead}0";
                var currentValueForFourthZ0 = $"{BitsRead}1";
                
                FourthZ0.North = Glues.IncrementStopper(currentValueForFourthZ0, Signal);
                FifthZ1.North  = Glues.IncrementStopper(currentValueForFifthZ1,  Signal);
                FifthZ1.Color = "red";

            } else
            {
                FourthZ0.North = new Glue($"LR: ReadContinue: {BitsRead}1, S={Signal}");
                FifthZ1.North  = new Glue($"LR: ReadContinue: {BitsRead}0, S={Signal}");
            }
        }


        Glue Bind(Tile a, Tile b) => new Glue($"{a.Name} -> {b.Name}, {BitsRead}, S={Signal} ");


        Tile Tile(string name) => new Tile(name);


        string IncrementInBinary(string currentValue)
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
