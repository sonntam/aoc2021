using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    public static class StreamReaderExtensions
    {
        public static int ReadLineToInt(this StreamReader rdr)
        {
            return int.Parse(rdr.ReadLine());
        }
    }
}
