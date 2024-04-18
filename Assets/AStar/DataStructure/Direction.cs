using System.Collections.Generic;

namespace AStar
{
    public static class Direction
    {
        public static readonly List<GridLocation> FourDir = new List<GridLocation>()
        {
            new GridLocation(){x = 1, y = 0},  // right
            new GridLocation(){x = 0, y = 1},  // top
            new GridLocation(){x = -1, y = 0}, // left
            new GridLocation(){x = 0, y = -1}, // down 
        };
        
        public static readonly List<GridLocation> EightDir = new List<GridLocation>()
        {
            new GridLocation(){x = 1, y = 0},  // right
            new GridLocation(){x = 1, y = 1},  // topRight
            new GridLocation(){x = 0, y = 1},  // top
            new GridLocation(){x = -1, y = 1},  // topLeft
            new GridLocation(){x = -1, y = 0}, // left
            new GridLocation(){x = -1, y = -1}, // leftDown
            new GridLocation(){x = 0, y = -1}, // down 
            new GridLocation(){x = 1, y = -1}, // downRight 
        };
    }
}