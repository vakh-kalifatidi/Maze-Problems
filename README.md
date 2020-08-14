# Maze-Problems
Solving problems in logical matrix (binary rectangular matrix).

# Simplify the maze with walled up cells that can be ignored when looking for a path from start to exit.

logic that I posted can be used to solve the problem of finding a path from start to exit while minimizing calculation time.

### Today I will only post the code in C# that walled up the cells

Forgive me this time for the code without comments.


## Driver
Please copy into console application into Main.
```
using (Maze maze = new Maze())
{
    maze.LoadMaze("...text file path or jagged or 2D array...");
    maze.DisplayMaze();
    Console.WriteLine("\n");

    MazeWallBuilder wb = new MazeWallBuilder(maze);
    wb.WallUpMazeCells();

    Maze newmaze = new Maze();
    newmaze.LoadMaze(wb._mazeArray, maze.StartX, maze.StartY, maze.ExitX, maze.ExitY);
    newmaze.DisplayMaze();
}

Console.Read();

```
