namespace Tiles.V2
{

    using Models;

    public static class Glues
    {
        public static Glue Writer(string binaryValue, string signal) => new Glue($"Writer: V={binaryValue} S={signal}");

        public static Glue CopyStopper(string signal) => new Glue($"CopyStopper: S={signal}");

        public static Glue Reader(string signal)  => new Glue($"Reader: S={signal}");

        public static Glue Hook(string binaryValue, string signal) => new Glue($"Hook: {binaryValue}, S={signal}");
    }
}
