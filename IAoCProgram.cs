using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    public abstract class IAoCProgram
    {
        protected string PathInputTestA;
        protected string PathInputTestB;
        protected string PathInputA;
        protected string PathInputB;
        protected bool SameInputForBAsForA = false;

        public bool IsTestARunnable => PathInputTestA != null;

        public bool IsTestBRunnable => PathInputTestB != null;

        public bool IsARunnable => PathInputA != null;
        public bool IsBRunnable => PathInputB != null || IsARunnable && SameInputForBAsForA;

        public int Day => int.Parse(String.Concat(this.GetType().Name.Reverse().TakeWhile(x => x >= '0' && x <= '9').Reverse()));

        public IAoCProgram(bool SameInputForBAsForA)
        {
            this.SameInputForBAsForA = SameInputForBAsForA;

            // try to find data file
            string path;

            path = Path.Combine(Environment.CurrentDirectory, $"day{Day}TestA.txt");
            PathInputTestA = File.Exists(path) ? path : null;

            path = Path.Combine(Environment.CurrentDirectory, $"day{Day}TestB.txt");
            PathInputTestB = File.Exists(path) ? path : null;

            path = Path.Combine(Environment.CurrentDirectory, $"day{Day}A.txt");
            PathInputA = File.Exists(path) ? path : null;

            path = Path.Combine(Environment.CurrentDirectory, $"day{Day}B.txt");
            PathInputB = File.Exists(path) ? path : null;
        }

        public virtual string RunA(StreamReader s) => "";
        public virtual string RunB(StreamReader s) => "";

        public StreamReader GetInputA()
        {
            if (PathInputA != null)
                return new StreamReader(PathInputA, true);

            return null;
        }

        public StreamReader GetInputTestA()
        {
            if (PathInputTestA != null)
                return new StreamReader(PathInputTestA, true);

            return null;
        }

        public StreamReader GetInputTestB()
        {
            if (PathInputTestB != null)
                return new StreamReader(PathInputTestB, true);

            return null;
        }

        public StreamReader GetInputB()
        {
            if (PathInputB != null)
                return new StreamReader(PathInputB, true);

            if (SameInputForBAsForA)
                return GetInputA();

            return null;
        }

    }
}
