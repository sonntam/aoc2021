using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day11 : IAoCProgram
    {
        public Day11() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {
            List<List<int>> initval = new List<List<int>>();

            //input = GetInputB();

            // Get initval
            while(!input.EndOfStream)
            {
                initval.Add(input.ReadLine().Select(x => int.Parse($"{x}")).ToList());
            }

            var x0 = initval.To2DArray();

            int n = x0.GetLength(0), m = x0.GetLength(1);
            int flashcounter = 0;

            //Console.Clear();
            //Console.WriteLine("Initial condition");
            //x0.ToPlainString();
            //Console.ReadKey();

            // Simulate
            for( int step = 0; step < 100; step++ )
            {
                var prevstate = (int[,])x0.Clone();
                flashcounter += SimulateStep(ref x0);

                //Console.Clear();
                //Console.WriteLine($"After {step + 1} steps:");
                //x0.ToPlainString();
                //Console.WriteLine("\nPrevious step:");
                //prevstate.ToPlainString();
                //Console.ReadKey();
            }

            return flashcounter.ToString();
        }

        private int SimulateStep(ref int[,] state)
        {
            int n = state.GetLength(0), m = state.GetLength(1);
            bool[,] flashedDone = new bool[n, m];
            flashedDone.Fill2DArray(false);

            // Increase all by 1
            bool oneHasFlashed = false;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    state[i, j]++;
                    if (state[i, j] >= 10)
                    {
                        // Flash!
                        oneHasFlashed = true;
                    }
                }
            }

STARTOVER:
            while (oneHasFlashed)
            {
                oneHasFlashed = false;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        if (state[i, j] >= 10 && flashedDone[i,j] == false)
                        {
                            // Flash!
                            for (int i1 = Math.Max(0, i - 1); i1 < Math.Min(n, i + 2); i1++)
                            {
                                for (int j1 = Math.Max(0, j - 1); j1 < Math.Min(m, j + 2) ; j1++)
                                {
                                    if (i1 != i || j1 != j)
                                    {
                                        state[i1, j1]++;
                                        if( state[i1, j1] >= 10)
                                        {
                                            oneHasFlashed = true;
                                        }
                                    }
                                }
                            }
                            flashedDone[i, j] = true;
                        }
                    }
                }
                goto STARTOVER;
            }

            // Reset to zero
            var flashCounter = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (state[i, j] >= 10)
                    {
                        state[i, j] = 0;
                    }

                    if (flashedDone[i, j]) flashCounter++;
                }
            }

            return flashCounter;
        }

        public override string RunB(StreamReader input)
        {
            List<List<int>> initval = new List<List<int>>();

            //input = GetInputB();

            // Get initval
            while (!input.EndOfStream)
            {
                initval.Add(input.ReadLine().Select(x => int.Parse($"{x}")).ToList());
            }

            var x0 = initval.To2DArray();

            int n = x0.GetLength(0), m = x0.GetLength(1);
            int flashcounter = 0;

            //Console.Clear();
            //Console.WriteLine("Initial condition");
            //x0.ToPlainString();
            //Console.ReadKey();

            // Simulate
            int step = 0;

            while (flashcounter != 100)
            {
                var prevstate = (int[,])x0.Clone();
                flashcounter = SimulateStep(ref x0);
                step++;

                //Console.Clear();
                //Console.WriteLine($"After {step + 1} steps:");
                //x0.ToPlainString();
                //Console.WriteLine("\nPrevious step:");
                //prevstate.ToPlainString();
                //Console.ReadKey();
            }

            return step.ToString();
        }
    }
}
