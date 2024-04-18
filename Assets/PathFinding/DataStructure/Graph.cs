using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class Graph
    {
        public int width;
        public int height;
        public HashSet<GridLocation> walls = new HashSet<GridLocation>();

        public Graph(int width, int height, HashSet<GridLocation> walls)
        {
            this.width = width;
            this.height = height;
            this.walls = walls;
        }

        public bool InBounds(GridLocation id)
        {
            return 0 <= id.x && id.x < width &&
                   0 <= id.y && id.y < height;
        }
        
        public bool Passable(GridLocation id)
        {
            return !walls.Contains(id);
        }

        public List<GridLocation> GetNeighbors(GridLocation id, PathFindingRoot.MoveDirection moveDirection)
        {
            if (moveDirection == PathFinding.PathFindingRoot.MoveDirection.FourPlus)
            {
                return GetNeighborsByFourPlus(id);
            }
            
            List<GridLocation> neighbors = new List<GridLocation>();
            List<GridLocation> dirs = moveDirection == PathFinding.PathFindingRoot.MoveDirection.Four ? Direction.FourDir : Direction.EightDir;
            foreach (var dir in dirs)
            {
                int nextX = id.x + dir.x;
                int nextY = id.y + dir.y;
                GridLocation next = new GridLocation(nextX, nextY);
                if (InBounds(next) && Passable(next))
                {
                    neighbors.Add(next);
                }
            }
            return neighbors;
        }

        private List<GridLocation> GetNeighborsByFourPlus(GridLocation id)
        {
            List<GridLocation> activeDirection = new List<GridLocation>();
            List<GridLocation> neighbors = new List<GridLocation>();
            foreach (var dir in Direction.FourDir)
            {
                int nextX = id.x + dir.x;
                int nextY = id.y + dir.y;
                GridLocation next = new GridLocation(nextX, nextY);
                if (InBounds(next) && Passable(next))
                {
                    activeDirection.Add(dir);
                    neighbors.Add(next);
                }
            }
            if (activeDirection.Count <= 1)
            {
                return neighbors;
            }

            // 计算额外的四个方向
            HashSet<GridLocation> extraActiveDir = new HashSet<GridLocation>();
            for (var i = 0; i < activeDirection.Count; i++)
            {
                for (var j = 0; j < activeDirection.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    var combinedDir = activeDirection[i] + activeDirection[j];
                    if (combinedDir != GridLocation.Zero)
                    {
                        extraActiveDir.Add(combinedDir);
                    }
                }
            }
            foreach (var dir in extraActiveDir)
            {
                int nextX = id.x + dir.x;
                int nextY = id.y + dir.y;
                GridLocation next = new GridLocation(nextX, nextY);
                if (InBounds(next) && Passable(next))
                {
                    neighbors.Add(next);
                }
            }
            return neighbors;
        }

        // 两个相邻格子的消耗 准确函数
        private static int Cost(GridLocation from, GridLocation to)
        {
            var deltaX = Math.Abs(to.x - from.x);
            var deltaY = Math.Abs(to.y - from.y);
            return Math.Min(deltaX, deltaY) * 14 + Math.Abs(deltaX - deltaY) * 10;
            // return Math.Abs(to.x - from.x) + Math.Abs(to.y - from.y);
        }
        
        // 两个不相邻格子的消耗 预测函数
        // 如果是Dijkstra寻路 那么预测函数直接返回0即可。（当然可以另外写一个函数 让函数更加清晰
        // 如果是BFS寻路 那么让最后算出来的优先级更高即可
        // 所以说AStar是两者的结合
        private static int Heuristic(GridLocation from, GridLocation to)
        {
            var deltaX = Math.Abs(to.x - from.x);
            var deltaY = Math.Abs(to.y - from.y);
            return Math.Min(deltaX, deltaY) * 14 + Math.Abs(deltaX - deltaY) * 10;
        }
        
        public static bool AStarSearch(Graph graph, 
            GridLocation start, 
            GridLocation end, 
            PathFindingRoot.MoveDirection moveDirection,
            int maxStep,
            out Dictionary<GridLocation, GridLocation> cameFrom, 
            out Dictionary<GridLocation, GridInfo> infoMap)
        {
            // 初始化所有字段
            cameFrom = new Dictionary<GridLocation, GridLocation>();
            infoMap = new Dictionary<GridLocation, GridInfo>();
            PriorityQueue<GridLocationWithCost> frontier = new PriorityQueue<GridLocationWithCost>();
            Dictionary<GridLocation, int> costSoFar = new Dictionary<GridLocation, int>();
            int step = 0;
            
            // startPos作为第一个节点
            cameFrom[start] = start;
            var startToEnd = Heuristic(start, end);
            infoMap[start] = new GridInfo()
            {
                F = startToEnd,
                G = 0,
                H = startToEnd,
                Step = step,
            };
            frontier.Enqueue(new GridLocationWithCost(start, startToEnd, step));
            costSoFar[start] = 0;

            // 每次从Dirty节点中取出优先级最高的节点 并且从这个节点发出继续外扩去查找新节点
            while (frontier.Count() != 0)
            {
                if (step >= maxStep)
                {
                    return false;
                }
                
                var current = frontier.Dequeue();
                var currentLocation = current.GetLocation();

                // 目前是最快查找路径 如果使用查找最短路径则打开下边三行
                // if (currentLocation == end)
                // {
                //     return true;
                // }
                
                step++;
                var neighbors = graph.GetNeighbors(currentLocation, moveDirection);
                foreach (var next in neighbors)
                {
                    if (!graph.InBounds(next) || !graph.Passable(next))
                    {
                        continue;
                    }
                    var oldCostSoFar = int.MaxValue;
                    if (costSoFar.ContainsKey(next))
                    {
                        oldCostSoFar = costSoFar[next];
                    }

                    // 如果尚未到达此点或者新路线距离此点的路程更近 那么需要更新此点
                    var newCostSoFar = costSoFar[currentLocation] + Cost(currentLocation, next);
                    if (newCostSoFar < oldCostSoFar)
                    {
                        cameFrom[next] = currentLocation;
                        costSoFar[next] = newCostSoFar;
                        var costToEndPoint = Heuristic(next, end);
                        int priority = newCostSoFar + costToEndPoint;
                        frontier.Enqueue(new GridLocationWithCost(next, priority, step)); // time表示是在第几轮找到这个节点的。
                        infoMap[next] = new GridInfo()
                        {
                            F = priority,
                            G = newCostSoFar,
                            H = costToEndPoint,
                            Step = step,
                        };

                        // 目前是最快查找路径 如果使用查找最短路径则注释下边三行
                        if (next == end)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


    }
}