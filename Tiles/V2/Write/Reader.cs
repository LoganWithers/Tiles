namespace Tiles.V2.Write
{

    using System.Collections.Generic;

    using Models;

    public class Reader
    {
        public Glue NorthGlue { get; set; }

        private string Signal;

        private string BitsRead;

        private int TotalBits;

        private List<Tile> tiles;

        private Tile FirstZ0;
        private Tile SecondZ0;
        private Tile ThirdZ0;
        private Tile FourthZ0;
        private Tile FifthZ0;



        private Tile FirstZ1;
        private Tile SecondZ1;
        private Tile ThirdZ1;
        private Tile FourthZ1;


        public Reader(string bitsRead, string signal, int totalBits)
        {
            BitsRead = bitsRead;
            Signal   = signal;
            TotalBits = totalBits;
        }


        public IEnumerable<Tile> Tiles()
        {
            Setup();

            return tiles;
        }


        private void Setup()
        {
            FirstZ0   = Tile($"R: Z0-1st: {BitsRead}, S={Signal}");
            SecondZ0  = Tile($"R: Z0-2nd: {BitsRead + "1"}, S={Signal}");
            ThirdZ0   = Tile($"R: Z0-3rd: {BitsRead + "1"}, S={Signal}");
            FourthZ0  = Tile($"R: Z0-4th: {BitsRead + "1"}, S={Signal}");
            FifthZ0   = Tile($"R: Z0-5th: {BitsRead + "1"}, S={Signal}");
                                          

            FirstZ1   = Tile($"R: Z1-1st: {BitsRead}, S={Signal}");
            SecondZ1  = Tile($"R: Z1-2nd: {BitsRead + "0"}, S={Signal}");
            ThirdZ1   = Tile($"R: Z1-3rd: {BitsRead + "0"}, S={Signal}");
            FourthZ1  = Tile($"R: Z1-4th: {BitsRead + "0"}, S={Signal}");

            tiles = new List<Tile>{ FirstZ0, SecondZ0, ThirdZ0, FourthZ0, FifthZ0, FirstZ1, SecondZ1, ThirdZ1, FourthZ1 };

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

            FourthZ0.Down  = Bind(FourthZ0, FifthZ0);
            FirstZ0.Up     = FourthZ0.Down;

            FirstZ0.Up     = Bind(FirstZ0, FirstZ1);
            FirstZ1.Down   = FirstZ0.Up;

            FirstZ1.North  = Bind(FirstZ1, SecondZ1);
            SecondZ1.South = FirstZ1.North;

            SecondZ1.North = Bind(SecondZ1, ThirdZ1);
            ThirdZ1.South  = SecondZ1.North;

            ThirdZ1.North  = Bind(ThirdZ1, FourthZ1);
            FourthZ1.South = ThirdZ1.North;

            // final bit
            if (BitsRead.Length == TotalBits)
            {
                FourthZ1.North = new Glue($"ReadContinue: {BitsRead}0, S={Signal}");
                FifthZ0.North  = new Glue($"ReadContinue: {BitsRead}1, S={Signal}");
            } else
            {
                FourthZ1.North = new Glue($"ReadContinue: {BitsRead}0, S={Signal}");
                FifthZ0.North  = new Glue($"ReadContinue: {BitsRead}1, S={Signal}");
            }



                
            
        }

        Glue Bind(Tile a, Tile b) => new Glue($"{a.Name} -> {b.Name}, {BitsRead}, S={Signal} ");
        Tile Tile(string name) => new Tile(name);
    }
}
