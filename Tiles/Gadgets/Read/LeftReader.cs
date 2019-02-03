namespace Tiles.Gadgets.Read
{

    using System;
    using System.Collections.Generic;

    using Models;

    public class LeftReader
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


        public LeftReader(string bitsRead, string signal, int totalBits)
        {
            this.bitsRead  = bitsRead;
            this.signal    = signal;
            this.totalBits = totalBits;
        }


        public IEnumerable<Tile> Tiles()
        {
            Setup();

            return tiles;
        }


        private void Setup()
        {
            firstZ0  = Tile($"LR: Z0-1st, B='{bitsRead}', S={signal}");
            secondZ0 = Tile($"LR: Z0-2nd, B='1{bitsRead}', S={signal}");
            thirdZ0  = Tile($"LR: Z0-3rd, B='1{bitsRead}', S={signal}");
            fourthZ0 = Tile($"LR: Z0-4th, B='1{bitsRead}', S={signal}");


            firstZ1  = Tile($"LR: Z1-1st, B='{bitsRead}', S={signal}");
            secondZ1 = Tile($"LR: Z1-2nd, B='0{bitsRead}', S={signal}");
            thirdZ1  = Tile($"LR: Z1-3rd, B='0{bitsRead}', S={signal}");
            fourthZ1 = Tile($"LR: Z1-4th, B='0{bitsRead}', S={signal}");
            fifthZ1  = Tile($"LR: Z1-5th, B='0{bitsRead}', S={signal}");

            tiles = new List<Tile> {firstZ0, secondZ0, thirdZ0, fourthZ0, firstZ1, secondZ1, thirdZ1, fourthZ1, fifthZ1};

            if (bitsRead.Length == 0)
            {
                firstZ1.West = GlueFactory.Reader(signal);
            } else
            {
                firstZ0.South = new Glue($"LR: ReadContinue: {bitsRead}, S={signal}");
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
                var currentValueForFifthZ1  = $"0{bitsRead}";
                var currentValueForFourthZ0 = $"1{bitsRead}";
                
                fourthZ0.North = GlueFactory.IncrementStopper(currentValueForFourthZ0, signal);
                fifthZ1.North  = GlueFactory.IncrementStopper(currentValueForFifthZ1,  signal);
 
            } else
            {
                fourthZ0.North = new Glue($"LR: ReadContinue: 1{bitsRead}, S={signal}");
                fifthZ1.North  = new Glue($"LR: ReadContinue: 0{bitsRead}, S={signal}");
            }
        }


        private Glue Bind(Tile a, Tile b) => new Glue($"{a.Name} -> {b.Name}, {bitsRead}, S={signal} ");


        private Tile Tile(string name) => new Tile(name);
    }
}
