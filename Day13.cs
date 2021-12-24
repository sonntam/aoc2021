using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day13 : IAoCProgram
    {
        public Day13() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {

            List<Point2D> coords = new List<Point2D>();
            List<(char, int)> foldInstructions = new List<(char, int)>();

            bool readCoords = true;
            while(!input.EndOfStream)
            {
                var line = input.ReadLine();

                if( string.IsNullOrWhiteSpace(line) )
                {
                    readCoords = false;
                    continue;
                }

                if( readCoords )
                {
                    var coord = line.Split(',');
                    coords.Add(new Point2D(int.Parse(coord.First()), int.Parse(coord.Last())));
                }
                else
                {
                    var fold = line.Split('=');
                    foldInstructions.Add((fold.First().Last(), int.Parse(fold.Last())));
                }
            }

            //PlotCoords(coords);

            // Do 1 fold
            var fold1 = foldInstructions.First();
            var foldedCoords = coords.Select(e => { 
                if( fold1.Item1 == 'x' )
                {
                    return e.X <= fold1.Item2 ? e : e.MirrorX(fold1.Item2);
                }
                else
                {
                    return e.Y <= fold1.Item2 ? e : e.MirrorY(fold1.Item2);
                }
            }).Distinct(new Point2DEqualityComparer()).ToList();

            Console.WriteLine("After 1 fold:");
            //PlotCoords(foldedCoords);

            return foldedCoords.Count.ToString();
        }

        private void PlotCoords(List<Point2D> coords)
        {
            int maxXVal = coords.Max(x => x.X);
            int maxYVal = coords.Max(x => x.Y);

            for (int j = 0; j <= maxYVal; j++)
            {
                for (int i = 0; i <= maxXVal; i++)
                {
                    Console.Write(coords.Contains(new Point2D(i, j)) ? '#' : '.');
                }
                Console.WriteLine("");
            }
        }

        public override string RunB(StreamReader input)
        {
            List<Point2D> coords = new List<Point2D>();
            List<(char, int)> foldInstructions = new List<(char, int)>();

            bool readCoords = true;
            while (!input.EndOfStream)
            {
                var line = input.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    readCoords = false;
                    continue;
                }

                if (readCoords)
                {
                    var coord = line.Split(',');
                    coords.Add(new Point2D(int.Parse(coord.First()), int.Parse(coord.Last())));
                }
                else
                {
                    var fold = line.Split('=');
                    foldInstructions.Add((fold.First().Last(), int.Parse(fold.Last())));
                }
            }

            //PlotCoords(coords);
            List<Point2D> foldedCoords = new List<Point2D>(coords);
            int numfolds = 0;
            foreach( var fold in foldInstructions)
            {
                numfolds++;
                foldedCoords = foldedCoords.Select(e => {
                    if (fold.Item1 == 'x')
                    {
                        return e.X <= fold.Item2 ? e : e.MirrorX(fold.Item2);
                    }
                    else
                    {
                        return e.Y <= fold.Item2 ? e : e.MirrorY(fold.Item2);
                    }
                }).Distinct(new Point2DEqualityComparer()).ToList();
            }

            Console.WriteLine($"After {numfolds} fold:");
            PlotCoords(foldedCoords);

            return foldedCoords.Count.ToString();
        }
    }
}
