namespace Tiles.Models
{

    using System.Text;

    public class TdpOptions
    {

        public TdpOptions(string tdsFileName, string seedName)
        {
            TdsFileName = tdsFileName;
            SeedName    = seedName;
            Coordinates = new Coordinates();
        }


        public string TdsFileName { get; }

        public int Temperature { get; } = 1;

        public string SeedName { get; }

        public Coordinates Coordinates { get; }

        public override string ToString() => new StringBuilder()
                                            .Append($"{TdsFileName}.tds")
                                            .AppendLine()
                                            .Append("Mode=aTAM")
                                            .AppendLine()
                                            .Append($"Temperature={Temperature}")
                                            .AppendLine()
                                            .Append($"{SeedName} {Coordinates.ToString()}")
                                            .ToString();
    }

}