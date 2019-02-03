namespace Tiles.Models
{

    using System.Collections.Generic;
    using System.Text;

    public class Tile
    {
        public Tile(string name)
        {
            Name = name;
            Glues = new List<Glue> {North, South, East, West, Up, Down};
        }

        public string Name { get; }

        public List<Glue> Glues { get;  }
        public string Label { get; set; } = string.Empty;
        public Glue North { get; set; } = new Glue();
        public Glue South { get; set; } = new Glue();
        public Glue East { get; set; } = new Glue();
        public Glue West { get; set; } = new Glue();
        public Glue Up { get; set; } = new Glue();
        public Glue Down { get; set; } = new Glue();
        public string Color { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder()
                    .Append($"TILENAME {Name}")
                    .AppendLine()
                    .Append($"NORTHBIND {North.Bind}")
                    .AppendLine()
                    .Append($"EASTBIND {East.Bind}")
                    .AppendLine()
                    .Append($"SOUTHBIND {South.Bind}")
                    .AppendLine()
                    .Append($"WESTBIND {West.Bind}")
                    .AppendLine()
                    .Append($"UPBIND {Up.Bind}")
                    .AppendLine()
                    .Append($"DOWNBIND {Down.Bind}")
                    .AppendLine()
                    .Append($"NORTHLABEL {North.Label}")
                    .AppendLine()
                    .Append($"EASTLABEL {East.Label}")
                    .AppendLine()
                    .Append($"SOUTHLABEL {South.Label}")
                    .AppendLine()
                    .Append($"WESTLABEL {West.Label}")
                    .AppendLine()
                    .Append($"UPLABEL {Up.Label}")
                    .AppendLine()
                    .Append($"DOWNLABEL {Down.Label}")
                    .AppendLine();



            if (!string.IsNullOrEmpty(Color))
            {
                sb.Append($"TILECOLOR {Color}")
                  .AppendLine();
            }

            return sb.Append("CREATE")
                     .AppendLine()
                     .ToString();
        }
    }
}
