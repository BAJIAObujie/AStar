using System;
using System.Collections.Generic;

namespace AStar
{
    public struct GridLocation
    {
        public int x;
        public int y;

        public GridLocation(int valueX, int valueY)
        {
            x = valueX;
            y = valueY;
        }

        public static GridLocation Zero = new GridLocation(0, 0);
        
        public static bool operator== (GridLocation options1, GridLocation options2)
        {
            return options1.Equals(options2);
        }
        
        public static bool operator!=(GridLocation point1, GridLocation point2)
        {
            return !point1.Equals(point2);
        }
        
        public static GridLocation operator+(GridLocation point1, GridLocation point2)
        {
            return new GridLocation(point1.x + point2.x, point1.y + point2.y);
        }
        
        public static GridLocation operator-(GridLocation point1, GridLocation point2)
        {
            return new GridLocation(point1.x - point2.x, point1.y - point2.y);
        }
        
        public override bool Equals(object obj)
        {
            if (obj is GridLocation other)
            {
                return this.x == other.x && this.y == other.y;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + x.GetHashCode();
                hash = hash * 23 + y.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"[x:{x} y:{y}]";
        }
    }
    
   
}