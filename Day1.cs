using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day1 : IAoCProgram
    {
        public Day1() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {
            int? prevVal = null;
            int counter = 0;

            while(!input.EndOfStream)
            {
                var lineVal = int.Parse( input.ReadLine() );

                if( prevVal.HasValue )
                {
                    if (prevVal < lineVal) counter++;
                }

                prevVal = lineVal;
            }

            return counter.ToString();
        }

        public override string RunB(StreamReader input)
        {
            Queue<int> buffer = new Queue<int>(4);
            int counter = 0;

            while(!input.EndOfStream)
            {
                while( buffer.Count < 4 )
                {
                    buffer.Enqueue(input.ReadLineToInt());
                }

                // Compare
                if (buffer.Take(3).Sum() < buffer.Skip(1).Take(3).Sum()) 
                    counter++;

                buffer.Dequeue();
            }

            return counter.ToString();
        }
    }
}
