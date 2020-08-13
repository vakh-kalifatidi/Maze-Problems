using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MazeExplorer
{
    class Maze : IDisposable
    {
        public int[,] Array { set; get; }
        public int Rows { set; get; }
        public int Columns { set; get; }
        public int Wall { set; get; }
        public int Room { set; get; }
        public int StartX { set; get; }
        public int StartY { set; get; }
        public int ExitX { set; get; }
        public int ExitY { set; get; }
        public char WallChar { set; get; }
        public char RoomChar { set; get; }
        public char StartChar { set; get; }
        public char ExitChar { set; get; }


        public Maze(int room = 0, int wall = 1,
            char roomChar = ' ', char wallChar = 'X',
            char startChar = 'S', char exitChar = 'F')
        {
            Array = null;
            Rows = 0;
            Columns = 0;
            StartX = 0;
            StartY = 0;
            ExitX = 0;
            ExitY = 0;
            Room = room;
            Wall = wall;
            WallChar = wallChar;
            RoomChar = roomChar;
            StartChar = startChar;
            ExitChar = exitChar;
        }

        public bool LoadMaze<T>(T tmaze,
                int startX = 0, int startY = 0, int exitX = 0, int exitY = 0)
        {
            try
            {
                if (typeof(T) == typeof(string))
                    return LoadFromFile((string)(object)tmaze);

                int[,] array;
                if (typeof(T) == typeof(int[][]))
                    array = ConvertJaggedTo2D((int[][])(object)tmaze);
                else if (typeof(T) == typeof(int[,]))
                    array = (int[,])(object)tmaze;
                else
                    throw new Exception("Unknown array type.");
                CheckRoomsAndWalls(array);
                Rows = array.GetLength(0);
                Columns = array.GetLength(1);
                Array = array;
                SetStartOrExit(true, startX, startY);
                SetStartOrExit(false, exitX, exitY);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Exeption caught in Maze:\n{0}", e.Message));
            }
        }


        void SetStartOrExit(bool isSettingStart, int x, int y)
        {
            try
            {
                if (!IsInRange(x, y))
                    throw new Exception("out of range");
                if (IsWall(x, y))
                    throw new Exception("a Wall");
                if (isSettingStart)
                {
                    StartX = x;
                    StartY = y;
                }
                else
                {
                    if (StartX == 0 && ExitX == 0 && StartY == 0 && ExitY == 0)
                    {
                        x = Rows - 1;
                        y = Columns - 1;
                    }
                    else if (StartX == ExitX && StartY == ExitY)
                        throw new Exception("a Start");
                    ExitX = x;
                    ExitY = y;
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    string.Format("Invalid {0} index [{1},{2}] - it is {3}.",
                    isSettingStart ? "Start" : "Exit", x, y, e.Message));
            }
        }

        void CheckRoomsAndWalls(int[,] maze)
        {
            int walls = CountSpecific(maze, Wall);
            int rooms = CountSpecific(maze, Room);
            if (walls + rooms != maze.Length)
                throw new Exception(string.Format(
                    "Unknown cell found. Valid only: '{0}' and '{1}'.",
                    Wall, Room));
        }

        private bool LoadFromFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                    throw new Exception("File not found.");
                bool isStartSet = false;
                bool isExitSet = false;
                var ss = File.ReadLines(path);
                Rows = ss.Count();
                Columns = ss.First().Length;
                int[,] maze = new int[Rows, Columns];
                int i = 0;
                foreach (string s in ss)
                {
                    int j = 0;
                    foreach (char c in s)
                    {
                        int entry;
                        if (c == WallChar)
                            entry = Wall;
                        else if (c == StartChar)
                        {
                            if (isStartSet)
                                throw new Exception("Multiple Start.");
                            StartX = i;
                            StartY = j;
                            isStartSet = true;
                            entry = Room;
                        }
                        else if (c == ExitChar)
                        {
                            if (isExitSet)
                                throw new Exception("Multiple Exit.");
                            ExitX = i;
                            ExitY = j;
                            isExitSet = true;
                            entry = Room;
                        }
                        else if (c == RoomChar)
                            entry = Room;
                        else
                            throw new Exception(
                                string.Format("Unknown mark: '{0}'.", c));
                        maze[i, j] = entry;
                        j++;
                    }
                    i++;
                }
                if (!isExitSet || !isStartSet)
                    throw new Exception(string.Format("{0} not set.", isExitSet ? "Start" : "Exit"));

                this.Array = maze;

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("File path {0}.\n{1}",
                    path, e.Message));
            }
        }

        #region utils
        public bool IsInRange(int x, int y)
        {
            return x >= 0 && x < Rows && y >= 0 && y < Columns;
        }

        public bool IsWall(int x, int y)
        {
            return IsInRange(x, y) ? Array[x, y] == Wall : false;
        }

        public bool IsRoom(int x, int y)
        {
            return IsInRange(x, y) ? Array[x, y] == Room : false;
        }

        public bool IsStart(int x, int y)
        {
            return IsInRange(x, y) ? x == StartX && y == StartY : false;
        }

        public bool IsExit(int x, int y)
        {
            return IsInRange(x, y) ? x == ExitX && y == ExitY : false;
        }

        public int CountSpecific(int[,] array, int value)
        {
            var query = from int item in array
                        where item == value
                        select item;
            return query.Count();
        }

        public int[,] ConvertJaggedTo2D(int[][] arr)
        {
            int[,] array2D = new int[arr.Length, arr[0].Length];
            for (int i = 0; i < arr.Length; i++)
                for (int j = 0; j < arr[i].Length; j++)
                    array2D[i, j] = arr[i][j];
            return array2D;
        }

        public bool[,] ConvertJaggedTo2D(bool[][] arr)
        {
            bool[,] array2D = new bool[arr.Length, arr[0].Length];
            for (int i = 0; i < arr.Length; i++)
                for (int j = 0; j < arr[i].Length; j++)
                    array2D[i, j] = arr[i][j];
            return array2D;
        }

        public int[,] ConvertBoolToInt(bool[,] arr, int trueNbr = 1, int falseNbr = 0)
        {
            int[,] array = new int[arr.GetLength(0), arr.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    array[i, j] = arr[i, j] ? trueNbr : falseNbr;
            return array;
        }

        public void DisplayCellState(int x, int y)
        {
            string state;
            if (!IsInRange(x, y))
                state = "out of range";
            else if (IsWall(x, y))
                state = "a Wall";
            else if (IsStart(x, y))
                state = "a Start";
            else if (IsExit(x, y))
                state = "an Exit";
            else
                state = "a Room";
            Console.WriteLine("\nMaze cell [{0},{1}] is {2}.", x, y, state);
        }

        public void DisplayArray(int[,] array, string note = "",
            string pad = " ", Nullable<char> padChar = '0')
        {
            if (array != null)
            {
                int maxWidth = 0;
                if (!string.IsNullOrEmpty(pad))
                {
                    for (int i = 0; i < array.GetLength(0); i++)
                        for (int j = 0; j < array.GetLength(1); j++)
                        {
                            int width = array[i, j].ToString().Length;
                            maxWidth = width > maxWidth ? width : maxWidth;
                        }
                    maxWidth = maxWidth + pad.Length;
                }

                if (!string.IsNullOrWhiteSpace(note))
                    Console.WriteLine("{0}:", note);
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    if (maxWidth > 0)
                        for (int j = 0; j < array.GetLength(1); j++)
                        {
                            if (padChar == null)
                                Console.Write(array[i, j].ToString().PadLeft(maxWidth));
                            else
                                Console.Write(array[i, j].ToString().
                                  PadLeft(maxWidth - 1, (char)padChar).PadLeft(maxWidth));
                        }
                    else
                        for (int j = 0; j < array.GetLength(1); j++)
                            Console.Write(array[i, j]);

                    Console.WriteLine();
                }
            }
        }

        public void DisplayMaze(string note = "Maze", Nullable<char> roomChar = null, Nullable<char> wallChar = null)
        {
            if (Array != null)
            {
                if (!string.IsNullOrWhiteSpace(note))
                    Console.WriteLine("{0}:", note);
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        char c;
                        if (IsWall(i, j))
                            c = wallChar ?? WallChar;
                        else
                        {
                            if (IsStart(i, j))
                                c = StartChar;
                            else if (IsExit(i, j))
                                c = ExitChar;
                            else
                                c = roomChar ?? RoomChar;
                        }
                        Console.Write(c);
                    }
                    Console.WriteLine();
                }
            }
            else
                Console.Write("--- Maze not set ---");
        }

        public void DisplayMazeWithPath(List<Tuple<int, int>> path, string note = "Path", char pathChar = '*',
            Nullable<char> roomChar = null, Nullable<char> wallChar = null)
        {
            if (Array != null && path != null && path.Count != 0)
            {
                if (!string.IsNullOrWhiteSpace(note))
                    Console.WriteLine("{0} to exit {1} steps:", note, path.Count);

                int[,] arrPath = new int[Rows, Columns];
                foreach (var t in path)
                    arrPath[t.Item1, t.Item2] = 1;

                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        char c;
                        if (IsWall(i, j))
                            c = wallChar ?? WallChar;
                        else
                        {
                            if (IsStart(i, j))
                                c = StartChar;
                            else if (IsExit(i, j))
                                c = ExitChar;
                            else if (arrPath[i, j] == 1)
                                c = pathChar;
                            else
                                c = roomChar ?? RoomChar;
                        }
                        Console.Write(c);
                    }
                    Console.WriteLine();
                }
            }
        }

        #endregion


        #region for GC
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.Array != null)
                this.Array = null;
        }
        #endregion
    }
}
