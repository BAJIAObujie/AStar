using System;
using System.Collections.Generic;

namespace PathFinding
{
    // public struct GridLocationWithCost : IComparable
    // {
    //     public int x;
    //     public int y;
    //     public int cost;
    //
    //     public GridLocationWithCost(GridLocation id, int valueCost)
    //     {
    //         x = id.x;
    //         y = id.y;
    //         cost = valueCost;
    //     }
    //     
    //     public GridLocationWithCost(int valueX, int valueY, int valueCost)
    //     {
    //         x = valueX;
    //         y = valueY;
    //         cost = valueCost;
    //     }
    //     
    //     public int CompareTo(object obj)
    //     {
    //         if (obj is GridLocationWithCost other)
    //             return cost.CompareTo(other.cost);
    //         return 1;
    //     }
    // }
    
    public struct GridLocationWithCost : IComparable<GridLocationWithCost>
    {
        private int x;
        private int y;
        private int cost;
        private int time;

        public GridLocationWithCost(GridLocation id, int valueCost, int valueTime)
        {
            x = id.x;
            y = id.y;
            cost = valueCost;
            time = valueTime;
        }
        
        public GridLocationWithCost(int valueX, int valueY, int valueCost, int valueTime)
        {
            x = valueX;
            y = valueY;
            cost = valueCost;
            time = valueCost;
        }

        public GridLocation GetLocation()
        {
            return new GridLocation(x, y);
        }

        // 消耗越小的优先级越高
        // 同等消耗的情况下，后加入的节点优先级越高
        public int CompareTo(GridLocationWithCost other)
        {
            if (cost == other.cost)
            {
                return -time.CompareTo(other.time);
            }
            return cost.CompareTo(other.cost);
        }
    }
}