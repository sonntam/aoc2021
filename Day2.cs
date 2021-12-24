using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day2 : IAoCProgram
    {
        public Day2() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {

            int depth = 0;
            int position = 0;

            while(!input.EndOfStream)
            {
                var lineCmd = (input.ReadLine()).Split(' ');

                switch( lineCmd[0].ToLower() )
                {
                    case "forward": position += int.Parse(lineCmd[1]); break;
                    case "up": depth -= int.Parse(lineCmd[1]); break;
                    case "down": depth += int.Parse(lineCmd[1]); break;
                }
            }

            return (depth*position).ToString();
        }

        public override string RunB(StreamReader input)
        {

            int depth = 0;
            int position = 0;
            int aim = 0;

            while (!input.EndOfStream)
            {
                var lineCmd = (input.ReadLine()).Split(' ');

                switch (lineCmd[0].ToLower())
                {
                    case "forward":
                        {
                            var x = int.Parse(lineCmd[1]);
                            position += x;
                            depth += aim * x;
                            break;
                        }
                    case "up": aim -= int.Parse(lineCmd[1]); break;
                    case "down": aim += int.Parse(lineCmd[1]); break;
                }
            }

            return (depth * position).ToString();
        }
    }
}
