using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2021
{
    class Day3 : IAoCProgram
    {
        public Day3() : base(SameInputForBAsForA: true) { }

        public override string RunA(StreamReader input)
        {

            int codeCount = 0;

            List<int> bitCounter = new List<int>();

            while(!input.EndOfStream)
            {
                var bitcode = input.ReadLine();
                codeCount++;

                int i = -1;
                foreach( var bit in bitcode )
                {
                    i++;

                    if (bitCounter.Count < i+1) bitCounter.Add(0);

                    if (bit == '1') bitCounter[i]++;
                    
                }
            }

            var gamma_rate = Convert.ToInt32( String.Concat(bitCounter.Select(x => codeCount - x <= x ? '1' : '0')), 2);
            var epsilon_rate = Convert.ToInt32( String.Concat(bitCounter.Select(x => codeCount - x > x ? '1' : '0')), 2);

            return (gamma_rate * epsilon_rate).ToString();

        }

        public override string RunB(StreamReader input)
        {
            List<string> codes = new List<string>();

            // Read
            while (!input.EndOfStream)
            {
                var bitcode = input.ReadLine();
                codes.Add(bitcode);
            }

            List<int> idxOxygen = new List<int>(Enumerable.Range(0, codes.Count));
            List<int> idxCO2Scrubber = new List<int>(Enumerable.Range(0, codes.Count));

            int offs = 0;

            while (true)
            {
                FilterBitList(ref idxOxygen, codes, offs, true);
                if (idxOxygen.Count == 1) break;
                offs++;
            }

            offs = 0;
            while (true)
            {
                FilterBitList(ref idxCO2Scrubber, codes, offs, false);
                if (idxCO2Scrubber.Count == 1) break;
                offs++;
            }

            var oxy_gen_rate = Convert.ToInt32(codes[idxOxygen[0]], 2);
            var co2_scrub_rate = Convert.ToInt32(codes[idxCO2Scrubber[0]], 2);

            return (co2_scrub_rate * oxy_gen_rate).ToString();
        }

        private void FilterBitList(ref List<int> idxList, List<string> codes, int offs, bool ChooseMostCommonBit)
        {
            // Analyze
            var bitCounter = 0;
            foreach (var idx in idxList)
            {
                if (codes[idx].ElementAt(offs) == '1')
                    bitCounter++;
            }

            // Filter phase
            char selected;
            if (ChooseMostCommonBit)
            {
                if (bitCounter >= idxList.Count - bitCounter) selected = '1'; else selected = '0';
            } 
            else
            {
                if (bitCounter >= idxList.Count - bitCounter) selected = '0'; else selected = '1';
            }

            idxList = idxList.Where(x => codes[x].ElementAt(offs) == selected).ToList();
        }
    }
}
