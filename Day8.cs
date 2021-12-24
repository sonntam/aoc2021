using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{

    public class DigitDemangler
    {
        protected List<Digit> digits;

        protected Dictionary<char, char> permutation;

        protected Dictionary<Digit, int> dictionary;

        public DigitDemangler( IEnumerable<Digit> digits )
        {
            this.digits = digits.ToList();

            /* Demangle: 1,4,7,8 are easy --> know "a"
             * 0,6,9 have 6 segments
             * 2,3,5 have 5 segments
             * 0 - 6 and 9 have "d" in common with 8, but 0 doesn't have it --> know "d" and 0. then 
             * 2 - has 5 segments and the one that is in 7 but not in 1 is one segment
             * 3 - 
             * 5 - 
             * 6 - has all from 8 but one not from 1
             * 9 - has all from 8 and also the one 1 does not have
             * 
             */
            permutation = new Dictionary<char, char>(Enumerable.Range(0, 7).Select(x => new KeyValuePair<char, char>((char)('a' + x), (char)('a' + x))));

            var digit1 = digits.Where(x => x.Segments.Length == 2).Single();
            var digit7 = digits.Where(x => x.Segments.Length == 3).Single();
            var digit4 = digits.Where(x => x.Segments.Length == 4).Single();
            var digit8 = digits.Where(x => x.Segments.Length == 7).Single();

            var a = digit7.Segments.Except(digit1.Segments).Single();
            permutation[a] = 'a';

            // find 0 out of 7, 6  and 9
            var digit6 = digits.Where(x => x.Segments.Length == 6 && x.Segments.Intersect(digit7.Segments).Count() == 2).Single();

            // now find 9
            var digit9 = digits.Where(x => x.Segments.Length == 6 && x.Segments.Intersect(digit4.Segments).Count() == digit4.Segments.Length && x.Segments != digit6.Segments).Single();

            // now find 0
            var digit0 = digits.Where(x => x.Segments.Length == 6 && x.Segments != digit6.Segments && x.Segments != digit9.Segments).Single();

            // know we know c from 6 and e from 9 and d from 0
            var c = digit8.Segments.Except(digit6.Segments).Single();
            permutation[c] = 'c';

            var e = digit8.Segments.Except(digit9.Segments).Single();
            permutation[e] = 'e';

            var d = digit8.Segments.Except(digit0.Segments).Single();
            permutation[d] = 'd';

            // three segments to go

            // we know g is in 2 together with a c d e
            var digit2 = digits.Where(x => x.Segments.Length == 5 && x.Segments.Intersect($"{a}{c}{d}{e}").Count() == 4).Single();
            var g = digit2.Segments.Except($"{a}{c}{d}{e}").Single();
            permutation[g] = 'g';

            // we know f is in common with 6
            var f = digit6.Segments.Intersect(digit1.Segments).Single();
            permutation[f] = 'f';

            // b to go
            var b = digit8.Segments.Except($"{a}{c}{d}{e}{f}{g}").Single();
            permutation[b] = 'b';

            var digit3 = digits.Where(x => x.Segments.Intersect($"{a}{c}{d}{f}{g}").Count() == 5 && x.Segments.Length == 5).Single();
            var digit5 = digits.Where(x => x.Segments.Intersect($"{a}{b}{d}{f}{g}").Count() == 5 && x.Segments.Length == 5).Single();

            dictionary = new Dictionary<Digit, int>() {
                { digit0, 0 },
                { digit1, 1 },
                { digit2, 2 },
                { digit3, 3 },
                { digit4, 4 },
                { digit5, 5 },
                { digit6, 6 },
                { digit7, 7 },
                { digit8, 8 },
                { digit9, 9 },
            };

        }

        public int Demangle(string digit)
        {
            Digit d = new Digit(digit);

            return dictionary[d];
        }
    }

    public class Digit {

        protected string segments;

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Digit)) throw new TypeAccessException($"Cannot compare type {nameof(Digit)} to type {obj.GetType().Name}.");

            var dig = obj as Digit;

            return dig.Segments.Intersect(this.Segments).Count() == dig.Segments.Length;
        }

        public Digit(string segments)
        {
            this.segments = String.Concat(segments.OrderBy(x => (int)x));
        }

        public override int GetHashCode()
        {
            return this.Segments.GetHashCode();
        }

        public string Segments { get { return new string(segments); } }
    }

    class Day8 : IAoCProgram
    {
        

        public Day8() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {

            List<Digit> digitCodes = new List<Digit>();

            int counter = 0;

            while(!input.EndOfStream)
            {
                var temp = input.ReadLine().Split('|');
                var digits = temp.First().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                var numbers = temp.Last().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                var demangler = new DigitDemangler(digits.Select(x => new Digit(x)));

                foreach( var number in numbers )
                {
                    switch (demangler.Demangle(number))
                    {
                        case 1:
                        case 4:
                        case 7:
                        case 8:
                            counter++;
                            break;
                        default: break;
                    }
                }
            }

            return counter.ToString();
        }

        public override string RunB(StreamReader input)
        {

            List<Digit> digitCodes = new List<Digit>();

            int counter = 0;

            while (!input.EndOfStream)
            {
                var temp = input.ReadLine().Split('|');
                var digits = temp.First().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                var numbers = temp.Last().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                var demangler = new DigitDemangler(digits.Select(x => new Digit(x)));

                var currentNumber = 0;
                foreach (var number in numbers)
                {
                    currentNumber = currentNumber * 10 + demangler.Demangle(number);
                }
                counter += currentNumber;
            }

            return counter.ToString();
        }
    }
}
