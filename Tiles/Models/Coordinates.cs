namespace Tiles.Models
{
    public struct Coordinates
    {

        public int X { get; }
        public int Y { get; }
        public int Z { get; }


        public override string ToString() => $"{X} {Y} {Z}";
    }
}
