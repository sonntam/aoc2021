using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day14 : IAoCProgram
    {
        public Day14() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {

            LinkedList<char> polymerChain = new LinkedList<char>();
            Dictionary<string, char> rules = new Dictionary<string, char>();

            bool readPolymerChain = true;
            
            while (!input.EndOfStream)
            {
                var lineVal = input.ReadLine();
                if (string.IsNullOrWhiteSpace(lineVal)) continue;
                if( readPolymerChain )
                {
                    foreach( var polymer in lineVal )
                        polymerChain.AddLast(polymer);

                    readPolymerChain = false;
                } 
                else
                {
                    var entry = lineVal.Split(" -> ");
                    rules.Add(entry.First(), entry.Last().Single());
                }
            }

			// Do the polymerization steps
			for (int i = 0; i < 10; i++)
			{
				var currentPolymer = polymerChain.First;
				while (currentPolymer.Next != null)
				{
					if (rules.TryGetValue($"{currentPolymer.Value}{currentPolymer.Next.Value}", out char polymerizationElement) ) {
                        polymerChain.AddAfter(currentPolymer, polymerizationElement);

                        // Skip one (the currently added)
                        currentPolymer = currentPolymer.Next;
                    }

                    currentPolymer = currentPolymer.Next;
				}
			}

            var polymerCounts = polymerChain.GroupBy(c => c).ToDictionary(x => x.Key, x => x.Count());

            // Count most common and substract least common
            var countDiff = polymerCounts.Max(x => x.Value) - polymerCounts.Min(x => x.Value);


            return countDiff.ToString();
        }

        public override string RunB(StreamReader input)
        {
            LinkedList<char> polymerChain = new LinkedList<char>();
            Dictionary<string, char> rules = new Dictionary<string, char>();

            bool readPolymerChain = true;

            while (!input.EndOfStream)
            {
                var lineVal = input.ReadLine();
                if (string.IsNullOrWhiteSpace(lineVal)) continue;
                if (readPolymerChain)
                {
                    foreach (var polymer in lineVal)
                        polymerChain.AddLast(polymer);

                    readPolymerChain = false;
                }
                else
                {
                    var entry = lineVal.Split(" -> ");
                    rules.Add(entry.First(), entry.Last().Single());
                }
            }

            // Housekeeping
            // Current Counts of polymer pairs
            // Current Counts of single polymers

            Dictionary<string, long> polymerPairCounts = new Dictionary<string, long>();
            Dictionary<char, long> polymerCounts;

            // initialize
            var currentPolymer = polymerChain.First;
            while( currentPolymer.Next != null )
            {
                var currentPair = $"{currentPolymer.Value}{currentPolymer.Next.Value}";
                if (polymerPairCounts.ContainsKey(currentPair)) polymerPairCounts[currentPair]++;
                else polymerPairCounts[currentPair] = 1;

                currentPolymer = currentPolymer.Next;
            }

            polymerCounts = polymerChain.GroupBy(x => x).ToDictionary(x => x.Key, x => (long)x.Count());

            // Do the polymerization steps
            for (int i = 0; i < 40; i++)
            {
                var diffPairCounts = new Dictionary<string, long>();
                foreach ( var polymerPair in polymerPairCounts )
                {
                    if (!rules.ContainsKey(polymerPair.Key) || polymerPair.Value == 0) continue;

                    // Update pairs
                    if (diffPairCounts.ContainsKey(polymerPair.Key)) diffPairCounts[polymerPair.Key] -= polymerPair.Value;
                    else diffPairCounts[polymerPair.Key] = -polymerPair.Value;

                    // Add new pairs 
                    var newPolymer = rules[polymerPair.Key];
                    var newPairs = new string[] {
                        $"{polymerPair.Key.First()}{newPolymer}",
                        $"{newPolymer}{polymerPair.Key.Last()}"
                    };

                    foreach (var newPair in newPairs)
                    {
                        if (diffPairCounts.ContainsKey(newPair)) diffPairCounts[newPair] += polymerPair.Value;
                        else diffPairCounts[newPair] = polymerPair.Value;
                    }

                    // Update polymercounts
                    if (polymerCounts.ContainsKey(newPolymer)) polymerCounts[newPolymer] += polymerPair.Value;
                    else polymerCounts[newPolymer] = 1;
                }

                // Update polymerPairCounts
                foreach( var diffPairCount in diffPairCounts )
                {
                    if (polymerPairCounts.ContainsKey(diffPairCount.Key)) polymerPairCounts[diffPairCount.Key] += diffPairCount.Value;
                    else polymerPairCounts[diffPairCount.Key] = diffPairCount.Value;
                }

                // Plausicheck
                if (polymerPairCounts.Any(x => x.Value < 0) )
                    throw new Exception($"Implausible data for {polymerPairCounts.First(x => x.Value < 0).Key} polymer!");
            }

            // Count most common and substract least common
            var countDiff = polymerCounts.Max(x => x.Value) - polymerCounts.Min(x => x.Value);


            return countDiff.ToString();
        }
    }
}
