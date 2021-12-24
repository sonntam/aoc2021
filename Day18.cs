using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day18 : IAoCProgram
    {
        public Day18() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {
            SnailNumber opA = null, opB = null;

            while(!input.EndOfStream)
            {
                var lineVal = input.ReadLine();

                if( opA == null )
                {
                    opA = SnailNumber.Parse(lineVal);
                }
                else
                {
                    opB = SnailNumber.Parse(lineVal);
                    Console.WriteLine($"  {opA}");
                    Console.WriteLine($"+ {opB}");
                    opA = opA + opB;
                    opA.Reduce();
                    Console.WriteLine($"= {opA}");
                }

                
            }


            return opA.Magnitude().ToString();
        }

        public override string RunB(StreamReader input)
        {
            List<SnailNumber> numbers = new List<SnailNumber>();
            while (!input.EndOfStream)
            {
                numbers.Add( SnailNumber.Parse( input.ReadLine() ) );
            }

            int maxMagnitude = 0;
            SnailNumber opA = null, opB = null;
            // Check all combinations
            for( int i = 0; i < numbers.Count; i++ )
            {
                for( int j = 0; j < numbers.Count; j++ )
                {
                    if (i == j) continue;
                    var magnitude = (numbers[i] + numbers[j]).Magnitude();

                    if( magnitude > maxMagnitude )
                    {
                        opA = numbers[i]; opB = numbers[j];
                        maxMagnitude = magnitude;
                    }
                }
            }

            Console.WriteLine("Max addition magnitude:");
            Console.WriteLine($"  {opA}");
            Console.WriteLine($"+ {opB}");
            Console.WriteLine($"= {opA + opB}");

            return maxMagnitude.ToString();
        }
    }

    public abstract class SnailNumberElement
    {
        public SnailNumber Parent;

        public abstract int Magnitude();
        public abstract SnailNumberElement DeepClone();

        public void ReplaceChildWith(SnailNumberElement newEl)
        {
            if (Parent == null) throw new Exception("Element is not a child element.");

            if( Parent.X == this )
                Parent.X = newEl;

            if (Parent.Y == this)
                Parent.Y = newEl;

            newEl.Parent = Parent;
        }
    }

    public class SnailValue : SnailNumberElement
    {
        public SnailValue(int value)
        {
            Value = value;
        }

        public int Value { get; set; }

        public void Add( SnailValue sv )
        {
            Value += sv.Value;
        }

        public override SnailNumberElement DeepClone()
        {
            return new SnailValue(Value);
        }

        public override int Magnitude()
        {
            return Value;
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }

    public class SnailNumber : SnailNumberElement
    {
        public SnailNumberElement X = null;
        public SnailNumberElement Y = null;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('[');
            sb.Append(X.ToString());
            sb.Append(',');
            sb.Append(Y.ToString());
            sb.Append(']');

            return sb.ToString();
        }

        public SnailNumber(SnailNumberElement x, SnailNumberElement y)
        {
            X = x;
            Y = y;

            if( x != null ) x.Parent = this;
            if (y != null) y.Parent = this;
        }

        public override SnailNumberElement DeepClone()
        {
            return new SnailNumber(this.X.DeepClone(), this.Y.DeepClone());
        }

        public static SnailNumber operator +(SnailNumber a, SnailNumber b)
        {
            var newA = a.DeepClone(); // new SnailNumber(a.X, a.Y);
            var newB = b.DeepClone(); // new SnailNumber(b.X, b.Y);
            
            var newVal = new SnailNumber(newA, newB) { Parent = null };

            newVal.Reduce();

            return newVal;
        }

        public void Reduce()
        {
            bool reduceActionHappened = false;  // Exit  flag
            IEnumerable<SnailNumber> explodables = null;
            IEnumerable<SnailValue> splittables = null;

            //Console.WriteLine(this.ToString());
            do
            {
                reduceActionHappened = false;

                // Check for explosions
                explodables = (this as SnailNumberElement).IterateTreeDepthFirst(x => x switch
                {
                    SnailNumber sn => new SnailNumberElement[] { sn.X, sn.Y },
                    _ => null
                }).Where(x => x switch // Select only numbers that have two value elements
                {
                    SnailNumber sn => sn.X.GetType() == typeof(SnailValue) && sn.Y.GetType() == typeof(SnailValue),
                    _ => false
                })
                .Where(x =>
                {    // Select numbers that have at least 4 parents 
                    int numparents = 0;

                    if (x == this) return false;

                    SnailNumberElement currentParent = x.Parent;
                    while (currentParent != null)
                    {
                        numparents++;
                        if (numparents >= 4) return true;

                        if (currentParent == this) break;
                        currentParent = currentParent.Parent;
                    }
                    return false;
                }).Select(x => x as SnailNumber);

                foreach (var explodable in explodables)
                {
                    reduceActionHappened = true;
                    Explode(explodable);
                    //Console.WriteLine(this.ToString());
                }

                // Check for splits
                splittables = (this as SnailNumberElement).IterateTreeDepthFirst(x => x switch
                {
                    SnailNumber sn => new SnailNumberElement[] { sn.X, sn.Y },
                    _ => null
                }).Where(x => x switch // Select only numbers that have two value elements
                {
                    SnailValue sv => sv.Value >= 10,
                    _ => false
                }).Select(x => x as SnailValue);

                // Split
                var splittable = splittables.FirstOrDefault();
                if( splittable != null )
                {
                    reduceActionHappened = true;
                    Split(splittable);
                    //Console.WriteLine(this.ToString());
                }
            } while (reduceActionHappened);
        }

        private static void Split(SnailValue splittable)
        {
            SnailValue newX, newY;

            newX = new SnailValue( (int)Math.Floor((double)splittable.Value / 2.0) );
            newY = new SnailValue( (int)Math.Ceiling((double)splittable.Value / 2.0) );

            splittable.ReplaceChildWith(new SnailNumber(newX, newY));
        }

        private void Explode(SnailNumber explodable)
        {
            SnailValue leftVal = null, rightVal = null;

            // Find left value
            var currentParent = explodable.Parent;
            var currentPart = explodable;
            while (currentParent != null)
            {
                if (currentParent.Y == currentPart)
                {
                    // Recurse into X of this parent to get its Y
                    leftVal = GetRightmostYValue(currentParent.X);
                    break;
                }

                // Go up the tree
                currentPart = currentParent;
                currentParent = currentParent.Parent;
            }

            // Find right value
            currentParent = explodable.Parent;
            currentPart = explodable;
            while (currentParent != null)
            {
                if (currentParent.X == currentPart)
                {
                    rightVal = GetLeftmostXValue(currentParent.Y);
                    break;
                }

                // Go up the tree
                currentPart = currentParent;
                currentParent = currentParent.Parent;
            }

            // Do the explosion
            // Replace with "0"
            explodable.ReplaceChildWith(new SnailValue(0));

            // New numbers
            leftVal?.Add(explodable.X as SnailValue);
            rightVal?.Add(explodable.Y as SnailValue);
        }

        public SnailValue GetRightmostYValue(SnailNumberElement sne)
        {
            switch( sne )
            {
                case SnailValue sv: return sv;
                case SnailNumber sn:
                {
                    while (sn.Y.GetType() == typeof(SnailNumber))
                    {
                        sn = sn.Y as SnailNumber;
                    }
                    return sn.Y as SnailValue;
                }
                default: return null;
            }
        }

        public SnailValue GetLeftmostXValue(SnailNumberElement sne)
        {
            switch (sne)
            {
                case SnailValue sv: return sv;
                case SnailNumber sn:
                    {
                        while (sn.X.GetType() == typeof(SnailNumber))
                        {
                            sn = sn.X as SnailNumber;
                        }
                        return sn.X as SnailValue;
                    }
                default: return null;
            }
        }



        public static SnailNumber Parse(string input)
        {
            StringReader rdr = new StringReader(input);

            return Parse(rdr, null) as SnailNumber;
        }

        private enum ReadState
        {
            EXPECT_NEWNUMBER,
            EXPECT_COMMA,
            EXPECT_LEFT,
            EXPECT_RIGHT,
            EXPECT_CLOSENUMBER
        }

        public static SnailNumberElement Parse(StringReader str, SnailNumber parent = null)
        {
            ReadState state = ReadState.EXPECT_NEWNUMBER;
            SnailNumber current = null;

            char ch = (char)str.Read();
            while ( ch != unchecked((char)-1) )
            {
                switch( ch )
                {
                    case char c when "0123456789".Contains(c):
                        if( state == ReadState.EXPECT_NEWNUMBER )
                        {
                            return new SnailValue(int.Parse($"{c}")) { Parent = parent };
                        } else throw new Exception("Parse error.");

                    case '[':
                        if( state == ReadState.EXPECT_NEWNUMBER )
                        {
                            current = new SnailNumber(null,null) { Parent = parent };
                            state = ReadState.EXPECT_LEFT;
                            current.X = Parse(str, current);
                            state = ReadState.EXPECT_COMMA;
                        } else throw new Exception("Parse error.");

                        break;
                    case ']':
                        if (state == ReadState.EXPECT_CLOSENUMBER)
                        {
                            return current;
                        }
                        else throw new Exception("Parse error.");
                        break;
                    case ',':
                        if (state == ReadState.EXPECT_COMMA)
                        {
                            state = ReadState.EXPECT_RIGHT;
                            current.Y = Parse(str, current);
                            state = ReadState.EXPECT_CLOSENUMBER;
                        }
                        else throw new Exception("Parse error.");
                        break;
                    default:
                        throw new Exception($"Invalid character {ch}");
                }

                ch = (char)str.Read();
            }
            return null;
        }

        public override int Magnitude()
        {
            return 3 * this.X.Magnitude() + 2 * this.Y.Magnitude();
        }

    }
}
