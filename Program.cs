using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace arraypathcs
{
    class RandomFunc {
        public static int GetRandomInt(int maxValue, Predicate<int> numFilter) {
            byte[] rngSeed = new byte[4];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(rngSeed);
            }

            var random = new Random(BitConverter.ToInt32(rngSeed, 0));
            int temp = random.Next(maxValue);
            while(!numFilter(temp))
                temp = random.Next(maxValue);

            return temp;
        }
        
        public static int[,] GenerateRandomArray(int n, int m, int maxValue) {
            int[,] randomArray = new int[n, m];

            for (int i = 0; i < n; i++) {
                for (int j = 0; j < m; j++) {
                    randomArray[i, j] = GetRandomInt(maxValue, (int value) => { return value >= 1 && value <= maxValue; });
                }
            }

            return randomArray;
        }
    }

    class IndexPair {
        private int row { get; set; }
        private int col { get; set; }

        public IndexPair(int row, int col) {
            this.row = row;
            this.col = col;
        }

        public int GetRow() {
            return this.row;
        }

        public int GetCol() {
            return this.col;
        }
    }

    class ArrayPath {
        private int[,] numberArray { get; set; }
        private int arrayRows { get; set; }
        private int arrayCols { get; set; }
        private int sumGoal { get; set; }
        private int startX { get; set; }
        private int startY { get; set; }
        private List<IndexPair> indexPath { get; set; }
        private List<string> pathCollection { get; set; }

        public ArrayPath(int m, int n, int maxValue, int sumGoal) {
            this.numberArray = RandomFunc.GenerateRandomArray(m, n, maxValue);
            this.arrayRows = m;
            this.arrayCols = n;
            this.sumGoal = sumGoal;
            this.indexPath = new List<IndexPair>();
            this.pathCollection = new List<string>();
        }

        public void SetStart(int x = -1, int y = -1) {
            if (x == -1)
                this.startX = RandomFunc.GetRandomInt(this.arrayRows,  (int value) => { return value >= 1 && value <= this.arrayRows; });
            else
                this.startX = x;
            if (y == -1)
                this.startY = RandomFunc.GetRandomInt(this.arrayCols,  (int value) => { return value >= 1 && value <= this.arrayCols; });
            else
                this.startY = y;
        }

        public string DisplayArrayStart() {
            // int arrayRank = numberArray.Rank;
            int maxRowIdx = this.numberArray.GetUpperBound(0) - this.numberArray.GetLowerBound(0);
            int maxColIdx = this.numberArray.GetUpperBound(1) - this.numberArray.GetLowerBound(1);

            string displayString = "";
            for (int i = 0; i <= maxColIdx; i++) {
                displayString += "\t[" + i + "]";
            }
            displayString += "\n";

            for (int i = 0; i <= maxRowIdx; i++) {
                displayString += "[" + i + "]\t";
                for (int j = 0; j <= maxColIdx; j++) {
                    displayString += this.numberArray[i, j];
                    if (j != maxColIdx)
                        displayString += "\t";
                }
                displayString += "\n";
            }
            displayString += "\n";

            displayString += "Start Row " + this.startX + " : Col " + this.startY + "\t" + this.numberArray[this.startX, this.startY];

            return displayString;
        }

        public async Task<int> CountPaths() {
            if (this.numberArray[this.startX, this.startY] <= this.sumGoal) {
                if (this.numberArray[this.startX, this.startY] == this.sumGoal) {
                    this.pathCollection.Add(this.startX + ":" + this.startY);
                }
                else {
                    IndexPair pair = new IndexPair(this.startX, this.startY);
                    this.indexPath.Add(pair);
                    await FindPaths(this.startX - 1, this.startY);
                    await FindPaths(this.startX + 1, this.startY);
                    await FindPaths(this.startX, this.startY - 1);
                    await FindPaths(this.startX, this.startY + 1);
                }
            }

            if (this.pathCollection.Count > 0) {
                foreach (string pathString in pathCollection) {
                    Console.WriteLine($"Path: {pathString}");
                }
                Console.WriteLine("");
            }

            return this.pathCollection.Count;
        }

        private async Task FindPaths(int row, int col) {
            if (0 <= row && row < this.arrayRows) {
                if (0 <= col && col < this.arrayCols) {
                    int pathSum = 0;
                    foreach (IndexPair pair in this.indexPath) {
                        pathSum += this.numberArray[pair.GetRow(), pair.GetCol()];
                    }
                    if (pathSum + this.numberArray[row, col] <= sumGoal) {
                        IndexPair newPair = new IndexPair(row, col);
                        this.indexPath.Add(newPair);

                        if (pathSum + this.numberArray[row, col] == sumGoal) {
                            string currentPath = "";
                            foreach (IndexPair pair in this.indexPath) {
                                currentPath += pair.GetRow() + ":" + pair.GetCol() + ",";
                            }
                            currentPath = currentPath.Substring(0, currentPath.Length - 1);
                            this.pathCollection.Add(currentPath);
                        }
                        else {
                            await FindPaths(row - 1, col);
                            await FindPaths(row + 1, col);
                            await FindPaths(row, col - 1);
                            await FindPaths(row, col + 1);
                        }

                        this.indexPath.RemoveAt(indexPath.Count - 1);
                    }
                }
            }
        }
    }
    
    class Program {
        public static void Main (string[] args) {
            ArrayPath paths = new ArrayPath(6, 9, 22, 20);
            paths.SetStart(-1, -1);
            // paths.SetPathRoot(3, 4);

            Console.WriteLine($"{paths.DisplayArrayStart()}\n");

            Console.WriteLine($"Found {paths.CountPaths().Result} path(s)");
        }
    }
}
