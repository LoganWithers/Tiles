namespace Tiles.Models
{

    using System;
    using System.Collections.Generic;

    public class CounterSettings
    {

        // should always be odd 
        public int ActualHeightPerDigit { get; }

        public int BitsRequiredToEncodeUpToBaseValueInBinary { get; }
        public int HorizontalDigitsPerRow                        { get; }
        public IEnumerable<string> BinaryDigitPatterns { get; }
        public int BaseK                                { get; }
        public long StoppingValue { get; }
        public string LastRowInBaseK { get; }
        public int LowestPowerOfTwoGreaterThanBaseK { get; }

        public CounterSettings(int baseK, long stoppingValue = 10, int startingValue = 0)
        {
            BaseK                                      = baseK;
            StoppingValue                              = stoppingValue;
            BitsRequiredToEncodeUpToBaseValueInBinary  = Convert.ToString(BaseK - 1, 2).Length;
            ActualHeightPerDigit                       = 2 * BitsRequiredToEncodeUpToBaseValueInBinary + 1;
            LastRowInBaseK                             = stoppingValue.ConvertToBase(BaseK);
            HorizontalDigitsPerRow                     = LastRowInBaseK.Length;
            BinaryDigitPatterns                        = CreateVerticalBitPatterns();
            LowestPowerOfTwoGreaterThanBaseK           = Convert.ToInt32(Math.Ceiling(Log2(BaseK)));

        }


        
        private IEnumerable<string> CreateVerticalBitPatterns()
        {
            var uniqueBitPatterns = new HashSet<string>();

            Console.WriteLine("-------------------------");
            Console.WriteLine(" Value     |  Binary    |");
            Console.WriteLine("-------------------------");

            for (var i = 0; i < BaseK; i++)
            {
                var paddedBinaryValue = Convert.ToString(i, 2).PadLeft(BitsRequiredToEncodeUpToBaseValueInBinary, '0');
                uniqueBitPatterns.Add(paddedBinaryValue);
                Console.WriteLine("{0,-10} | {1,-10} |", i, paddedBinaryValue);
            }

            Console.WriteLine("-------------------------\n\n\n");

            return uniqueBitPatterns;
        }


        private static double Log2(double n) => Math.Log(n, 2);
    }
}
