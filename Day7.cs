using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    
    class Day7 : IAoCProgram
    {
        // Optimization problem with 1 variable:
        //
        // min abs(x-p1) + abs(x-p2) + ... + abs(x-pN)
        //  x
        //
        // with x integer
        public Day7() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {
            // Brute force this shit

            List<int> positions = new List<int>();

            while(!input.EndOfStream)
            {
                positions.AddRange((input.ReadLine()).Split(',').Select(x => int.Parse(x)));
            }

            // Brute!!!
            int minfuel = int.MaxValue;
            int minp = positions.Min(), maxp = positions.Max();
            int optpos = 0;
            for( int i = minp; i <= maxp; i++ )
            {

                int fuel = positions.Select(x => Math.Abs(x - i)).Sum();
                
                if (fuel < minfuel)
                {
                    minfuel = fuel;
                    optpos = i;
                }
            }

            Console.WriteLine($"The optimal position is {optpos} @ {minfuel} fuel burnt.");

            return minfuel.ToString();
        }

        public override string RunB(StreamReader input)
        {
            // Brute force this shit

            List<int> positions = new List<int>();

            while (!input.EndOfStream)
            {
                positions.AddRange((input.ReadLine()).Split(',').Select(x => int.Parse(x)));
            }

            // Brute!!!
            long minfuel = long.MaxValue;
            int minp = positions.Min(), maxp = positions.Max();
            int optpos = 0;
            for (int i = minp; i <= maxp; i++)
            {
                long fuel = positions.Select(x => Enumerable.Range(0, Math.Abs(x - i)+1).Sum(x => (long)x)).Sum();
                if (fuel < minfuel)
                {
                    minfuel = fuel;
                    optpos = i;
                }
            }

            Console.WriteLine($"The optimal position is {optpos} @ {minfuel} fuel burnt.");

            return minfuel.ToString();
        }
    }
}
