using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    
    class Day6 : IAoCProgram
    {

        public Day6() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {

            List<int> fishAges = new List<int>();

            while(!input.EndOfStream)
            {
                fishAges.AddRange((input.ReadLine()).Split(',').Select(x => int.Parse(x)));
            }

            // Simulate
            for( int day = 0; day < 80; day++ )
            {
                int newFishes = 0;
                fishAges = fishAges.Select(x => {
                    if( x == 0 )
                    {
                        newFishes++;
                        return 6;
                    }
                    return x - 1;
                }).ToList();

                fishAges.AddRange(Enumerable.Repeat(8, newFishes));
            }

            return fishAges.Count.ToString();
        }

        public override string RunB(StreamReader input)
        {

            List<int> fishAges = new List<int>();

            while (!input.EndOfStream)
            {
                fishAges.AddRange((input.ReadLine()).Split(',').Select(x => int.Parse(x)));
            }

            var fishAgesCount = fishAges.GroupBy(x => x).Select(x => new KeyValuePair<int, long>(x.Key, x.Count())).ToList();

            // Simulate
            for (int day = 0; day < 256; day++)
            {
                long newFishes = 0;
                fishAgesCount = fishAgesCount.Select(x => {
                    if( x.Key == 0 )
                    {
                        newFishes += x.Value;
                        return new KeyValuePair<int, long>(6, x.Value);
                    }
                    return new KeyValuePair<int, long>(x.Key - 1, x.Value);
                }).ToList();

                // Consolidate
                fishAgesCount = fishAgesCount.GroupBy(x => x.Key, x => x.Value, (k, v) => new KeyValuePair<int, long>(k, v.Sum())).ToList();
                
                if( newFishes > 0 )
                    fishAgesCount.Add(new KeyValuePair<int, long>(8, newFishes));

                Console.WriteLine($"Population count after {day + 1} days: {fishAgesCount.Sum(x => x.Value)}");
            }

            return fishAgesCount.Sum(x => x.Value).ToString();
        }
    }
}
