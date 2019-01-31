namespace Tiles.Models
{
    public struct Coordinates
    {
        public Coordinates(int x = 0, int y = 0, int z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X { get; }
        public int Y { get; }
        public int Z { get; }


        public override string ToString() => $"{X} {Y} {Z}";
    }
}
