using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading;

namespace aoc2021
{
    class Day19 : IAoCProgram
    {
        public Day19() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {

            Scanner currentScanner = null;
            List<Scanner> scanners = new List<Scanner>();

            Regex scannerRex = new Regex(@"--- (scanner \d+) ---", RegexOptions.Compiled);

            while (!input.EndOfStream)
            {
                var lineVal = input.ReadLine();
                if (string.IsNullOrWhiteSpace(lineVal)) continue;

                var m = scannerRex.Match(lineVal);
                if (m.Success)
                {
                    currentScanner = new Scanner(m.Captures[0].Value);
                    scanners.Add(currentScanner);
                }
                else
                {
                    // Coordinate
                    var coordVals = lineVal.Split(',').Select(x => int.Parse(x)).ToArray();

                    currentScanner.DetectedBeacons.Add(new Vector3(coordVals[0], coordVals[1], coordVals[2]));
                }
            }

            List<(Matrix4x4, Scanner, Scanner)> conjunctions = new List<(Matrix4x4, Scanner, Scanner)>();

            foreach (var scanner1 in scanners)
            {
                object lockObj = new object();

                Parallel.ForEach(scanners.SkipWhile(x => x != scanner1).Skip(1), new ParallelOptions() { MaxDegreeOfParallelism = 1 }, scanner2 =>
                {
                    var success = scanner1.TryMatch(scanner2, out Matrix4x4 newRotation, out int maxMatches);

                    lock (lockObj)
                    {
                        Console.WriteLine($"Trying {scanner1} vs {scanner2}: # matched points={maxMatches}");
                        if (success)
                        {
                            Console.WriteLine($"{scanner1} matches {scanner2} with");
                            Console.WriteLine($"{newRotation}");
                        }
                    }
                });

                //foreach( var scanner2 in scanners.SkipWhile( x => x != scanner1 ).Skip(1) )
                //{
                //    if (scanner1 == scanner2) continue;

                //    Console.Write($"Trying {scanner1} vs {scanner2}");
                //    var success =  scanner1.TryMatch(scanner2, out Matrix4x4 newRotation, out int maxMatches);

                //    Console.WriteLine($": # matched points={maxMatches}");
                //    if( success )
                //    {
                //        Console.WriteLine($"{scanner1} matches {scanner2} with");
                //        Console.WriteLine($"{newRotation}");
                //    }
                //}
            }

            return "";
        }

        public override string RunB(StreamReader input)
        {
            return null;
        }
    }

    public struct RelativeOrientation   // XYZ rotations to get to the target coord system rotation
    {
        public Matrix4x4 Rotation;
    }

    public class Scanner
    {
        public override string ToString()
        {
            return Name;
        }
        public Matrix4x4 Rotation { get; protected set; } = Matrix4x4.Identity; // Relative to some coordinate system...

        public List<Vector3> DetectedBeacons { get; protected set; } = new List<Vector3>();
        public string Name { get; protected set; }

        public Scanner(string name)
        {
            Name = name;
        }

        public bool TryMatch(Scanner other, out Matrix4x4 otherRelativeRotation, out int maxMatches)
        {
            // At least 12 beacons must be matched

            // Brute force the shit out of it - try all points with all other points and all 24 rotations
            maxMatches = 0;

            List<Matrix4x4> rotCandidates = new List<Matrix4x4>();

            for (int j = 0; j < 4; j++) // rot y
            {
                var rY = Matrix4x4.CreateRotationY((float)(j * Math.PI / 2.0));
                for (int k = 0; k < 4; k++) // rot x
                {
                    var rX = Matrix4x4.CreateRotationX((float)(k * Math.PI / 2.0));
                    rotCandidates.Add(rX * rY);
                }
            }

            for (int i = 0; i < 2; i++)  // rot z 
            {
                var rZ = Matrix4x4.CreateRotationZ((float)(Math.PI / 2.0 + i * Math.PI));
                for (int k = 0; k < 4; k++) // rot x
                {
                    var rX = Matrix4x4.CreateRotationX((float)(k * Math.PI / 2.0));
                    rotCandidates.Add(rX * rZ);
                }
            }

            //var qq = rotCandidates.Select(x => Vector3.Transform(new Vector3(1, 2, 3), x)).Select(x => new Vector3((float)Math.Round(x.X), (float)Math.Round(x.Y), (float)Math.Round(x.Z))).Distinct().ToList();
            foreach (var candidateRotation in rotCandidates)
            {
                //Console.WriteLine($"{Vector3.Transform(new Vector3(1,2,3), candidateRotation)}");
                // Match
                for (int i1 = 0; i1 < this.DetectedBeacons.Count; i1++)
                {
                    for (int i2 = 0; i2 < other.DetectedBeacons.Count; i2++)
                    {
                        var matchCandidatePoint = Vector3.Transform(other.DetectedBeacons[i2], candidateRotation);
                        var offset = matchCandidatePoint - this.DetectedBeacons[i1];

                        // Now check all other points with teh offset and count - if 12 found then it is a match!
                        int matchCounter = 1;
                        for (int j1 = 0; j1 < DetectedBeacons.Count; j1++)
                        {
                            if (j1 == i1) continue;
                            for (int j2 = 0; j2 < other.DetectedBeacons.Count; j2++)
                            {
                                if (j2 == i2) continue;

                                var transformedPoint = Vector3.Transform(other.DetectedBeacons[j2], candidateRotation);
                                var squaredDistance = (DetectedBeacons[j1] - (transformedPoint - offset)).LengthSquared();
                                if (squaredDistance < 1e-1)
                                {
                                    matchCounter++;
                                    goto nextouterpoint;
                                }
                            }
                            nextouterpoint:;
                        }

                        if (matchCounter > maxMatches)
                        {
                            maxMatches = matchCounter;
                        }

                        if (matchCounter >= 12)
                        {
                            otherRelativeRotation = candidateRotation;
                            return true;
                        }
                    }

                }
            }

            otherRelativeRotation = Matrix4x4.Identity;

            return false;

        }
    }
}
