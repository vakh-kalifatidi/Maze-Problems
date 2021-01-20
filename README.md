# Maze-Problems
Solving problems in logical matrix (binary rectangular matrix).

# Simplify the maze by walled up cells that can be ignored when looking for a path from start to exit.

The logic I posted can be used to solve the problem of finding a path from start to exit while minimizing computation time.
This is achieved by significantly reducing the number of cells accepted for consideration. Let's call this simplification.
This simplification allows you to build a graph with a very limited number of nodes and edges. And this makes it easy to apply heavy algorithms such as DFS.
Run the code and you will be pleasantly surprised.
If anyone is familiar with such an algorithm, I would be grateful for the reference.
Please note I am not posting a code that is looking for an exit from a maze.

### The code contains logic for walled cells, which can be ignored when looking for an exit from the maze.

Forgive me this time for the code without comments.


## Driver 
Please mark the starting cell as “s” and the exitcell as “f”, or use “LoadMaze” or do whatever is convenient for you by updating the Maze class.
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
