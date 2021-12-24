using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day12 : IAoCProgram
    {
        public Day12() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {
            Dictionary<string, List<string>> connections = new Dictionary<string, List<string>>();

            while (!input.EndOfStream)
            {
                var lineVal = input.ReadLine().Split('-');
                var from = lineVal.First();
                var to = lineVal.Last();

                if (connections.ContainsKey(from))
                    connections[from].Add(to);
                else
                    connections.Add(from, new List<string>() { to });
            }

            Dictionary<string, List<string>> invertedConnections = new Dictionary<string, List<string>>();
            foreach( var targets in connections )
            {
                foreach( var target in targets.Value )
                {
                    if( invertedConnections.ContainsKey( target ) )
                    {
                        invertedConnections[target].Add(targets.Key);
                    }
                    else
                    {
                        invertedConnections[target] = new List<string>() { targets.Key };
                    }
                }
            }

            // Add and select distinct only
            foreach( var connection in invertedConnections )
            {
                if( connections.ContainsKey(connection.Key) ) {
                    connections[connection.Key].AddRange(connection.Value);
                    connections[connection.Key] = connections[connection.Key].Distinct().ToList();
                }
                else
                {
                    connections[connection.Key] = new List<string>(connection.Value.Distinct().ToList());
                }
            }

            // Pathfind from start to end
            // State is number of visits in each cave

            Console.WriteLine("Found paths:");
            int counter = 0;
            foreach( var path in Navigate("start", "end", connections) )
            {
                Console.WriteLine(path);
                counter++;
            }
            return counter.ToString();
        }

        public override string RunB(StreamReader input)
        {
            Dictionary<string, List<string>> connections = new Dictionary<string, List<string>>();

            while (!input.EndOfStream)
            {
                var lineVal = input.ReadLine().Split('-');
                var from = lineVal.First();
                var to = lineVal.Last();

                if (connections.ContainsKey(from))
                    connections[from].Add(to);
                else
                    connections.Add(from, new List<string>() { to });
            }

            Dictionary<string, List<string>> invertedConnections = new Dictionary<string, List<string>>();
            foreach (var targets in connections)
            {
                foreach (var target in targets.Value)
                {
                    if (invertedConnections.ContainsKey(target))
                    {
                        invertedConnections[target].Add(targets.Key);
                    }
                    else
                    {
                        invertedConnections[target] = new List<string>() { targets.Key };
                    }
                }
            }

            // Add and select distinct only
            foreach (var connection in invertedConnections)
            {
                if (connections.ContainsKey(connection.Key))
                {
                    connections[connection.Key].AddRange(connection.Value);
                    connections[connection.Key] = connections[connection.Key].Distinct().ToList();
                }
                else
                {
                    connections[connection.Key] = new List<string>(connection.Value.Distinct().ToList());
                }
            }

            // Pathfind from start to end
            // State is number of visits in each cave

            Console.WriteLine("Found paths:");
            int counter = 0;
            foreach (var path in Navigate2("start", "end", connections))
            {
                Console.WriteLine(path);
                counter++;
            }
            return counter.ToString();
        }

        private IEnumerable<string> Navigate(string startLoc, string endLoc, Dictionary<string, List<string>> connections, Dictionary<string, int> visitState = null, string path = null)
        {
            if (visitState == null)
                visitState = connections.Keys.ToDictionary(x => x, x => 0);

            if (path == null)
                path = startLoc;

            // clone state
            visitState = new Dictionary<string, int>(visitState);

            visitState[startLoc]++;

            // Find possible destinations
            var destinations = connections[startLoc].Where(x => IsBigCave(x) || visitState[x] == 0).ToList();

            if (destinations.Count == 0) // nothing found
                yield break;


            // Recurse
            foreach (var destination in destinations)
            {
                if (destination == endLoc)
                {
                    yield return path + "-" + endLoc;
                    continue;
                }

                foreach (var nextpath in Navigate(destination, endLoc, connections, visitState, path + "-" + destination))
                {
                    yield return nextpath;
                }
            }
        }

        private IEnumerable<string> Navigate2(string startLoc, string endLoc, Dictionary<string, List<string>> connections, Dictionary<string, int> visitState = null, string path = null)
        {
            if( visitState == null )
                visitState = connections.Keys.ToDictionary(x => x, x => 0);

            if (path == null)
                path = startLoc;

            // clone state
            visitState = new Dictionary<string, int>(visitState);

            visitState[startLoc]++;

            // Find possible destinations
            var destinations = connections[startLoc].Where(x => IsBigCave(x) || visitState[x] == 0 || ( 
                visitState.Where( y => !IsBigCave(y.Key) ).All(y => y.Value <= 1) && ( x != "start" || visitState["start"] == 0 ) && ( x != "end" || visitState["end"] == 0)
            ) ).ToList();

            if (destinations.Count == 0) // nothing found
                yield break;

            
            // Recurse
            foreach( var destination in destinations )
            {
                if( destination == endLoc )
                {
                    yield return path + "-" + endLoc;
                    continue;
                }

                foreach( var nextpath in Navigate2(destination, endLoc, connections, visitState, path + "-" + destination) )
                {
                    yield return nextpath;
                }
            }
        }

        private bool IsBigCave(string cave)
        {
            if (cave.ToLower() == cave) return false;
            return true;
        }
    }

   
}
