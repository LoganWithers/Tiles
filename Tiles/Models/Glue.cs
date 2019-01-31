namespace Tiles.Models
{
    public class Glue
    {
        public int Bind { get; set; }

        public string Label { get; set; }

        public Glue(string label)
        {
            Label = label;
            Bind  = 1;
        }

        public Glue() { }
    }
}
