﻿namespace Tiles.Models
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


        public long BestSeed;
        public string EncodedInBaseKBestSeed;

        public readonly List<string> BinaryStringsForSeed;
        public CounterSettings(int baseK, long stoppingValue = 10)
        {
            BaseK                                      = baseK;
            StoppingValue                              = stoppingValue;
            BitsRequiredToEncodeUpToBaseValueInBinary  = Convert.ToString(BaseK - 1, 2).Length;
            ActualHeightPerDigit                       = 2 * BitsRequiredToEncodeUpToBaseValueInBinary + 1;
            LastRowInBaseK                             = stoppingValue.ConvertToBase(BaseK);
            HorizontalDigitsPerRow                     = LastRowInBaseK.Length;
            BinaryDigitPatterns                        = CreateVerticalBitPatterns();
            LowestPowerOfTwoGreaterThanBaseK           = Convert.ToInt32(Math.Ceiling(Log2(BaseK)));

            BestSeed = Convert.ToInt32(Math.Pow(BaseK, LastRowInBaseK.Length) - 1 - stoppingValue);
            EncodedInBaseKBestSeed = BestSeed.ConvertToBase(BaseK);
            BinaryStringsForSeed = new List<string>();

            Console.WriteLine($"Closest value before next overflow: {Math.Pow(BaseK, LastRowInBaseK.Length) - 1}\n");
            Console.WriteLine($"Best seed in base {baseK}: {EncodedInBaseKBestSeed}\n");

            foreach (var letter in EncodedInBaseKBestSeed)
            {
                var digit = letter - '0';

                BinaryStringsForSeed.Add(Convert.ToString(digit, 2).PadLeft(BitsRequiredToEncodeUpToBaseValueInBinary, '0'));
            }
            Console.WriteLine(string.Join(", ", BinaryStringsForSeed));
        }


        
        private IEnumerable<string> CreateVerticalBitPatterns()
        {
            var uniqueBitPatterns = new HashSet<string>();

            Console.WriteLine("-------------------------");
            Console.WriteLine("| Digit     |  Binary    |");
            Console.WriteLine("-------------------------");

            for (var i = 0; i < BaseK; i++)
            {
                var paddedBinaryValue = Convert.ToString(i, 2).PadLeft(BitsRequiredToEncodeUpToBaseValueInBinary, '0');
                uniqueBitPatterns.Add(paddedBinaryValue);
                Console.WriteLine("| {0,-10} | {1,-10} |", i, paddedBinaryValue);
            }

            Console.WriteLine("-------------------------\n\n\n");
            Console.WriteLine("The binary values will be encoded with the most significant bit being the top most bit");
            return uniqueBitPatterns;
        }


        private static double Log2(double n) => Math.Log(n, 2);
    }
}
