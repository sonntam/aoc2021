using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day10 : IAoCProgram
    {
        public Day10() : base(SameInputForBAsForA: true) { }

        private Dictionary<char, int> PenaltyDict = new Dictionary<char, int>()
        {
            { ')', 3 },
            { ']', 57 },
            { '}', 1197 },
            { '>', 25137 },
        };

        private Dictionary<char, int> CompletionScore = new Dictionary<char, int>()
        {
            { ')', 1 },
            { ']', 2 },
            { '}', 3 },
            { '>', 4 },
        };

        public override string RunA(StreamReader input)
        {
            int syntax_error_score = 0;

            while(!input.EndOfStream)
            {
                var line = input.ReadLine();

                var idx = FindFirstIllegalChar(line);

                if( idx >= 0 )
                {
                    syntax_error_score += PenaltyDict[line[idx]];
                }
            }

            return syntax_error_score.ToString();
        }

        public override string RunB(StreamReader input)
        {
            List<ulong> scores = new List<ulong>();

            while (!input.EndOfStream)
            {
                var line = input.ReadLine();

                var idx = FindFirstIllegalChar(line);

                if (idx < 0)
                {
                    // Complete line
                    var completion = GetCompletionString(line);

                    // Score
                    scores.Add(completion.Aggregate((ulong)0, (v, x) => v * 5 + (ulong)CompletionScore[x]));
                }
            }

            // Sort and get middle element
            scores.Sort();
            var score = scores.Skip((scores.Count - 1) / 2).Take(1).Single();

            return score.ToString();
        }

        public string GetCompletionString(string chunks)
        {
            var openChars = "([{<";
            var closeChars = ")]}>";
            Dictionary<char, char> CloseCharFromOpenChar = new Dictionary<char, char>()
            {
                { '(', ')' },
                { '[', ']' },
                { '{', '}' },
                { '<', '>' },
            };

            var OpenCharFromCloseChar = CloseCharFromOpenChar.ToDictionary(x => x.Value, x => x.Key);

            var chunkStack = new Stack<char>();

            for (int idx = 0; idx < chunks.Length; idx++)
            {
                var c = chunks[idx];


                if (openChars.Contains(c))
                {
                    chunkStack.Push(c);
                }
                else if (closeChars.Contains(c))
                {
                    if (chunkStack.TryPeek(out char lastOpener))
                    {
                        if (c == CloseCharFromOpenChar[lastOpener])
                        {
                            chunkStack.Pop();
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    // found illegal char
                    return null;
                }
            }

            return String.Concat(chunkStack.Select( x => CloseCharFromOpenChar[x]));
        }

        // returns -1 if none found
        public int FindFirstIllegalChar(string chunks)
        {
            var openChars = "([{<";
            var closeChars = ")]}>";
            Dictionary<char, char> CloseCharFromOpenChar = new Dictionary<char, char>()
            {
                { '(', ')' },
                { '[', ']' },
                { '{', '}' },
                { '<', '>' },
            };

            var OpenCharFromCloseChar = CloseCharFromOpenChar.ToDictionary(x => x.Value, x => x.Key);

            var chunkStack = new Stack<char>();

            for( int idx = 0; idx < chunks.Length; idx++ )
            {
                var c = chunks[idx];
                

                if (openChars.Contains(c) )
                {
                    chunkStack.Push(c);
                }
                else if( closeChars.Contains(c) )
                {
                    if (chunkStack.TryPeek(out char lastOpener))
                    {
                        if (c == CloseCharFromOpenChar[lastOpener])
                        {
                            chunkStack.Pop();
                        }
                        else
                        {
                            return idx;
                        }
                    }
                }
                else
                {
                    // found illegal char
                    return idx;
                }
            }

            return -1;
        }
    }


}
