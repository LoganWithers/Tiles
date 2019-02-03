namespace Tiles.V2
{

    using Models;

    using Seed;

    public static class Glues
    {

        public static Glue Writer(string binaryValue, string signal) => new Glue($"Writer: V={binaryValue} S={signal}");


        public static Glue CopyStopper(string signal) => new Glue($"CopyStopper: S={signal}");


        public static Glue Reader(string signal) => new Glue($"Reader: S={signal}");


        public static Glue Hook(string binaryValue, string signal) => new Glue($"Hook: {binaryValue}, S={signal}");


        public static Glue RightWall => new Glue("RightWall 0");


        public static Glue LeftWall => new Glue("LeftWall 0");


        public static Glue PreReadRight(string signal) => new Glue($"PreReadRight: S={signal}");


        public static Glue PreReadLeft(string signal) => new Glue($"PreReadLeft: S={signal}");

        public static Glue IncrementStopper(string binaryValue, string signal) => new Glue($"IncrementStopper: V={binaryValue}, S={signal}");

        public static Glue Copier(string binaryValue, string signal) => new Glue($"Copier: V={binaryValue}, S={signal}");
    }

}
