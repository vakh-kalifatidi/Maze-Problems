using System;

namespace MazeExplorer
{
    class MazeWallBuilder : IDisposable
    {
        Maze _maze { set; get; }
        public int[,] _mazeArray { set; get; }

        public MazeWallBuilder(Maze maze)
        {
            _mazeArray = maze.Array.Clone() as int[,];
            _maze = maze;
        }

        public int[,] WallUpMazeCells()
        {
            try
            {
                WallUp_Square_Spiral();
                while (WallUp_Square());
                return _mazeArray;
            }
            catch (Exception e)
            {
                throw new Exception(string.Concat("Error building map: ", e.Message));
            }
        }


        bool WallUp_Square()
        {
            bool isUpdateDetected = false;
            for (int x = 0; x < _maze.Rows; x++)
                for (int y = 0; y < _maze.Columns; y++)
                    if (TryToWallUp(x, y))
                        if (!isUpdateDetected)
                            isUpdateDetected = true;
            return isUpdateDetected;
        }

        public void WallUp_Square_Spiral()
        {
            int topX = 0;
            int bottomX = _mazeArray.GetLength(0) - 1;
            int leftY = 0;
            int rightY = _mazeArray.GetLength(1) - 1;
            int iterations = _mazeArray.GetLength(0) / 2;
            for (int countIter = 0; countIter < iterations; countIter++)
            {
                for (int y = leftY; y <= rightY; y++)
                    TryToWallUp(topX, y);
                topX++;
                for (int x = topX; x <= bottomX; x++)
                    TryToWallUp(x, rightY);
                rightY--;
                for (int y = rightY; y >= leftY; y--)
                    TryToWallUp(bottomX, y);
                bottomX--;
                for (int x = bottomX; x >= topX; x--)
                    TryToWallUp(x, leftY);
                leftY++;
            }
            if (_mazeArray.GetLength(0) % 2 > 0) //if odd number
                for (int y = leftY; y <= rightY; y++)
                    TryToWallUp(topX, y);
        }

        bool TryToWallUp(int x, int y)
        {
            if (_mazeArray[x, y] == _maze.Room &&
                !(_maze.IsStart(x, y) || _maze.IsExit(x, y))
                && IsHaveToWallUp(x, y))
            {
                _mazeArray[x, y] = _maze.Wall;
                return true;
            }
            return false;
        }

        bool IsHaveToWallUp(int x, int y)
        {
            //traverse adjoining cells
            int[,] surrounding = CollectCellStateAround(x, y);
            bool n = surrounding[0, 1] == _maze.Room;
            bool e = surrounding[1, 2] == _maze.Room;
            bool s = surrounding[2, 1] == _maze.Room;
            bool w = surrounding[1, 0] == _maze.Room;
            //max 3 doors - previously visited cell allways Wall
            int count = (n ? 1 : 0) + (e ? 1 : 0) + (s ? 1 : 0) + (w ? 1 : 0);
            if (count == 0 || count == 1)
                return true;
            bool ne = surrounding[0, 2] == _maze.Room;
            bool se = surrounding[2, 2] == _maze.Room;
            bool sw = surrounding[2, 0] == _maze.Room;
            bool nw = surrounding[0, 0] == _maze.Room;
            //check arrangement and check if can go around
            if (count == 2 && !((n && s) || (w && e)))
                return (n && ne && e) || (e && se && s)
                        || (s && sw && w) || (w && nw && n);
            if (count == 3 && ((n && ne && e && se && s) || (e && se && s && sw && w)
                    || (s && sw && w && nw && n) || (w && nw && n && ne && e)))
                return true;
            return false;
        }

        //Moore neighborhood
        int[,] CollectCellStateAround(int x, int y)
        {
            int[,] array =
            {{ CetCellState(x - 1, y - 1), CetCellState(x - 1, y), CetCellState(x - 1, y + 1) },
            {  CetCellState(x, y - 1),     _maze.Wall,         CetCellState(x, y + 1) },
            {  CetCellState(x + 1, y - 1), CetCellState(x + 1, y), CetCellState(x + 1, y + 1) }};
            return array;
        }

        //               N  NE  E  SE S  SW   W   NW
        //int[] CdX = { -1, -1, 0, 1, 1,  1,  0, -1 };
        //int[] CdY = {  0,  1, 1, 1, 0, -1, -1, -1 };

        int CetCellState(int x, int y)
        {
            return _maze.IsInRange(x, y) && _mazeArray[x, y] == _maze.Room ?
                _maze.Room : _maze.Wall;
        }


        #region GC
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._maze = null;
                this._mazeArray = null;
            }
        }
        #endregion
    }
}
