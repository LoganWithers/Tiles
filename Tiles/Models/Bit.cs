namespace Tiles.Models {

    using System;
    using System.Collections.Generic;

    using Gadgets;

    public class Bit : AbstractGadget
    {
        public Tile TopLeft     { get; }
        public Tile BottomLeft { get; }
        public readonly List<Tile> Tiles;


        public int Value        { get; }

        private Tile TopRight   { get; }
        private Tile BottomRight { get; }
        private readonly string uuid;

        
        public Bit(int value, int key)
        {
            uuid  = Guid.NewGuid().ToString();
            Tiles = new List<Tile>();
            Value = value;
            
            BottomLeft = new Tile(Id(A, key))
            {
                East = Glue(A, B, key),
            };

            BottomRight = new Tile(Id(B, key))
            {
                West  = Glue(A, B, key),
                North = Glue(B, C, key)
            };

            TopRight = new Tile(Id(C, key))
            {
                South = Glue(B, C, key),
                West  = Glue(C, D, key)
            };

            TopLeft = new Tile(Id(D, key))
            {
                East = Glue(C, D, key)
            };

            Tiles = new List<Tile>{ BottomLeft, BottomRight, TopLeft, TopRight };
        }

        ///
        /// <summary>
        ///     If a bit is at z = 0, this will add a up label to the bottom right tile for a z = 1 tile to attach with it
        ///     using the provided glue id.
        /// <br/>
        ///     The 1 bit to the south must provide a down glue with the id provided.
        /// </summary>
        /// 
        public void BindSouthUp(Guid guid)
        {
            if (Value != 0)
            {
                throw new Exception("Value is not 0. Can't bind to Z = 2 plane.");
            }

            var id = guid.ToString();

            BottomLeft.Up = new Glue(id);
        }


        ///
        /// <summary>
        ///     If a bit is at z = 0, this will add a up label to the bottom right tile for a z = 1 tile to attach with it
        ///     using the provided glue id.
        /// <br/>
        ///     The 1 bit to the south must provide a down glue with the id provided.
        /// </summary>
        /// 
        public void BindNorthUp(Guid guid)
        {
            if (Value != 0)
            {
                throw new Exception("Value is not 0. Can't bind to Z = 2 plane.");
            }

            var id = guid.ToString();

            TopLeft.Up = new Glue(id);
        }


        ///
        /// <summary>
        ///     If a bit is at z = 1, this will add a tile to the north so that it can attach to a bit at z = 0,
        ///     using the provided glue id.
        /// <br/>
        ///     The 0 bit to the north must provide a up glue with the id provided.
        /// </summary>
        /// 
        public void BindNorthAndDown(Guid guid)
        {
            if (Value != 1)
            {
                throw new Exception("Value is not 1. Can't bind to Z = -1 plane.");
            }

            var id = guid.ToString();

            var addedTileNorth = new Tile($"North Down {id}") {
                South = new Glue(id),
                Down  = new Glue(id)
            };


            TopLeft.North = new Glue(id);

            Tiles.Add(addedTileNorth);
        }



        ///
        /// <summary>
        ///     If a bit is at z = 1, this will add a tile to the south so that it can attach to a bit at z = 0,
        ///     using the provided glue id.
        /// <br/>
        ///     The 0 bit to the north must provide a up glue with the id provided.
        /// </summary>
        /// 
        public void BindSouthAndDown(Guid guid)
        {
            if (Value != 1)
            {
                throw new Exception("Value is not 1. Can't bind south to Z = -1 plane.");
            }

            var id = guid.ToString();
            var addedTileSouth = new Tile($"South Down {id}") {
                North = new Glue(id),
                Down  = new Glue(id)
            };

            BottomLeft.South = new Glue(id);

            Tiles.Add(addedTileSouth);
        }


        private string Id(string id,   int    key) => $"{id} Zero {key}";
        private Glue Glue(string from, string to, int key) => new Glue($"({from}, {to}) Zero {key} {uuid}");

    }

}