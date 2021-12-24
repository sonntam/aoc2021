using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;

namespace aoc2021
{
    class Program
    {
        static private List<IAoCProgram> programs = new List<IAoCProgram>() {
            new Day1(), new Day2(), new Day3(), new Day4(), new Day5(), new Day6(), new Day7(), new Day8(), new Day9(), new Day10(), new Day11(), new Day12(),
            new Day13(), new Day14(), new Day15(), new Day16(), new Day17(), new Day18(), new Day19()
        };


        static void Main(string[] args)
        {
            var numDays = programs.Max(x => x.Day);
            int selectedDay;

            Console.WriteLine("AoC 2021");
            Console.WriteLine("========");
            Console.WriteLine("");

            IAoCProgram selectedProgram;

            while (true)
            {
                Console.Write($"Select a day to compute (1-{numDays}): ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out selectedDay))
                {
                    selectedProgram = programs.Where(x => x.Day == selectedDay).FirstOrDefault();

                    if (selectedProgram == null)
                    {
                        Console.WriteLine($"Program {selectedDay} does not exist. Try again...");
                        continue;
                    }

                    break;
                }
            }

            // Run
            string r;

            if (selectedProgram.IsTestARunnable)
            {
                r = selectedProgram.RunA(selectedProgram.GetInputTestA());

                if (r != null)
                {
                    Console.WriteLine(new string('*', 79));
                    Console.WriteLine($"Calculating Day {selectedDay} test input data of part A:");
                    Console.WriteLine(r);
                    Console.WriteLine(new string('*', 79));
                    WindowsClipboard.SetText(r);

                    Console.WriteLine("Press any key when ready to run next part...");
                    Console.ReadKey();
                }
            }

            if (selectedProgram.IsARunnable)
            {
                r = selectedProgram.RunA(selectedProgram.GetInputA());

                if (r != null)
                {
                    Console.WriteLine(new string('*', 79));
                    Console.WriteLine($"Calculating Day {selectedDay} part A:");
                    Console.WriteLine(r);
                    Console.WriteLine(new string('*', 79));
                    WindowsClipboard.SetText(r);
                    Console.WriteLine("Press any key when ready to run next part...");
                    Console.ReadKey();
                }
            }

            if (selectedProgram.IsTestBRunnable)
            {
                r = selectedProgram.RunB(selectedProgram.GetInputTestB());

                if (r != null)
                {
                    Console.WriteLine(new string('*', 79));
                    Console.WriteLine($"Calculating Day {selectedDay} test input data of part B:");
                    Console.WriteLine(r);
                    Console.WriteLine(new string('*', 79));
                    WindowsClipboard.SetText(r);

                    Console.WriteLine("Press any key when ready to run next part...");
                    Console.ReadKey();
                }
            }


            if (selectedProgram.IsBRunnable)
            {
                r = selectedProgram.RunB(selectedProgram.GetInputB());

                if (r != null)
                {
                    Console.WriteLine(new string('*', 79));
                    Console.WriteLine($"Calculating Day {selectedDay} part B:");
                    Console.WriteLine(r);
                    Console.WriteLine(new string('*', 79));
                    WindowsClipboard.SetText(r);
                }
            }
        }
    }
}
