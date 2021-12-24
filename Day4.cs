using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace aoc2021
{
    class BingoBoard
    {
        protected List<List<int>> data;
        protected List<List<bool>> marked;
        protected int n, m;

        public enum MarkedStatus
        {
            NotFound,
            FoundAndMarked,
            FoundAndWon,
            AlreadyCompleted
        }

        public BingoBoard( List<List<int>> rows )
        {
            n = rows.Count;
            m = rows[0].Count;

            data = rows;
            marked = new List<List<bool>>(Enumerable.Range(0,n).Select( _ => new List<bool>(Enumerable.Repeat(false, m))));
        }

        public bool HasBoardWon()
        {
            return marked.Any(x => x.All(y => y)) || Enumerable.Range(0, m).Select( x => marked.Select( y => y[x]).All(y => y)).Any(x => x);
        }

        public MarkedStatus TryMarkNumber(int number, out int iRow, out int iCol)
        {
            iRow = -1; iCol = -1;

            if (HasBoardWon()) return MarkedStatus.AlreadyCompleted;

            for( int i = 0; i < n; i++ )
            {
                for( int j = 0; j < m; j++ )
                {
                    if( data[i][j] == number )
                    {
                        marked[i][j] = true;

                        // Check if won
                        if( marked[i].All( x => x ) || marked.Select(x => x[j]).All( x => x) )
                        {
                            iRow = i; iCol = j;
                            return MarkedStatus.FoundAndWon;
                        }
                        return MarkedStatus.FoundAndMarked;
                    }
                }
            }
            return MarkedStatus.NotFound;
        }

        public int SumUnmarked()
        {
            int sum = 0;

            for( int i = 0; i < n; i++ )
            {
                for (int j = 0; j < m; j++)
                {
                    if (!marked[i][j]) sum += data[i][j];
                }
            }

            return sum;
        }
    }


    class Day4 : IAoCProgram
    {
        public Day4() : base(SameInputForBAsForA: true) { }

        public void ReadBoards(out List<BingoBoard> boards, out List<int> drawnNumbers)
        {
            var s = this.GetInputA();

            drawnNumbers = s.ReadLine().Split(',').Select(x => int.Parse(x)).ToList();

            bool startNewBoard = false;

            boards = new List<BingoBoard>();

            List<List<int>> currentRows = null;

            while (!s.EndOfStream)
            {
                var line = s.ReadLine();

                if (String.IsNullOrWhiteSpace(line))
                {
                    // Save
                    if (currentRows != null)
                    {
                        boards.Add(new BingoBoard(currentRows));
                    }

                    startNewBoard = true;
                    continue;
                }

                if (startNewBoard)
                {
                    currentRows = new List<List<int>>();
                    startNewBoard = false;
                }

                // Add row
                currentRows.Add(line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList());

            }

            // Save last board
            boards.Add(new BingoBoard(currentRows));
        }

        public override string RunA(StreamReader s)
        {
            ReadBoards(out List<BingoBoard> boards, out List<int> drawnNumbers);

            // Play
            foreach (var number in drawnNumbers) {
                foreach (var board in boards)
                {
                    if( board.TryMarkNumber(number, out int iRow, out int iCol) == BingoBoard.MarkedStatus.FoundAndWon )
                    {
                        return (board.SumUnmarked() * number).ToString();
                    }

                }
            }

            return "";

        }

        public override string RunB(StreamReader s)
        {
            ReadBoards(out List<BingoBoard> boards, out List<int> drawnNumbers);

            BingoBoard lastWinningBoard = null;
            int lastWinningNumber = 0;

            // Play
            foreach (var number in drawnNumbers)
            {
                foreach (var board in boards)
                {
                    if (board.TryMarkNumber(number, out int iRow, out int iCol) == BingoBoard.MarkedStatus.FoundAndWon)
                    {
                        lastWinningBoard = board;
                        lastWinningNumber = number;
                    }
                }
            }

            if (lastWinningBoard != null)
                return (lastWinningNumber * lastWinningBoard.SumUnmarked()).ToString();

            return "";

        }
    }
}
