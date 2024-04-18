using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AStar
{
    public class AStarPathFinding : MonoBehaviour
    {
        public GameObject cellRoot;
        public GameObject cellPrefab;
        public Color colorStartPos = Color.red;
        public Color colorEndPos = Color.blue;
        public Color colorFloor = Color.white;
        public Color colorWall = Color.black;

        public int rangeX = 10;
        public int rangeY = 10;
        [Range(0,1f)]public float wallPercent = 0.2f;

        private GridLocation startPos;
        private GridLocation endPos;
        public bool IsDebugMode = false;
        [ShowIf("IsDebugMode")] public int maxStep = 1000;
        
        public MoveDirection moveDirection;
        
        public enum MoveDirection
        {
            Four, // 上下左右四方寻路
            Eight, // 八方寻路
            FourPlus, // 上下左右四方寻路 如果左可以移动且上可以移动，那么左上也加入移动范围
        }
        
        private Graph graph;

        [Button("构造地图")]
        private void ConstructGraph()
        {
            cellPrefab.SetActive(false);
            int totalCount = rangeX * rangeY;
            int wallCount = Mathf.FloorToInt(totalCount * wallPercent);
            HashSet<GridLocation> walls = new HashSet<GridLocation>();
            for (int i = 0; i < wallCount; i++)
            {
                var wallX = Random.Range(0, rangeX);
                var wallY = Random.Range(0, rangeY);
                GridLocation wall = new GridLocation(wallX, wallY);
                walls.Add(wall);
            }
            graph = new Graph(rangeX, rangeY, walls);

            while (true)
            {
                startPos = new GridLocation(Random.Range(0, rangeX), Random.Range(0, rangeY));
                if (!walls.Contains(startPos))
                {
                    break;
                }
            }
            while (true)
            {
                endPos = new GridLocation(Random.Range(0, rangeX), Random.Range(0, rangeY));
                if (!walls.Contains(endPos))
                {
                    break;
                }
            }
            
            InitGraphView();
            DrawGraphView(new List<GridLocation>(){startPos}, null);
        }

        [Button("开始寻路")]
        public void StartPathFinding()
        {
            var step = !IsDebugMode ? Int32.MaxValue : maxStep;
            if (!Graph.AStarPathFinding(graph, startPos, endPos, moveDirection, step,
                out var cameFrom, out var infoMap))
            {
                DrawGraphView(new List<GridLocation>(), infoMap);
                Debug.Log("查找路径失败");
                return;
            }

            List<GridLocation> path = new List<GridLocation>();
            var lastOne = endPos;
            path.Add(lastOne);
            while (cameFrom.ContainsKey(lastOne))
            {
                var parent = cameFrom[lastOne];
                path.Add(parent);
                if (parent == startPos)
                {
                    break;
                }
                lastOne = parent;
            }
            path.Reverse();
  
            DrawGraphView(path, infoMap);
        }
        
        #region GraphView

        private List<List<GameObject>> cells;
        private List<List<Renderer>> cellRenderers;
        private List<List<Material>> cellMaterials;
        
        private void InitGraphView()
        {
            if (cells != null)
            {
                foreach (var gameObjects in cells)
                    foreach (var o in gameObjects)
                        Destroy(o);
                cells.Clear();
            }
            if (cellRenderers != null)
            {
                cellRenderers.Clear();
            }
            if (cellMaterials != null)
            {
                cellMaterials.Clear();
            }

            if (cellRoot == null)
                cellRoot = new GameObject("CellRoot");
            cellRoot.transform.localPosition = Vector3.zero;
            cellRoot.transform.localRotation = Quaternion.identity;
            cellRoot.transform.localScale = Vector3.one;
            cells = new List<List<GameObject>>();
            cellRenderers = new List<List<Renderer>>();
            cellMaterials = new List<List<Material>>();
            for (int i = 0; i < rangeX; i++)
            {
                cells.Add(new List<GameObject>());
                cellRenderers.Add(new List<Renderer>());
                cellMaterials.Add(new List<Material>());
                for (int j = 0; j < rangeY; j++)
                {
                    var cell = Instantiate(cellPrefab, cellRoot.transform, true);
                    cell.name = $"[X={i} Y={j}]";
                    cell.SetActive(true);
                    cell.transform.localPosition = new Vector3(i, 0, j);
                    cell.transform.localRotation = Quaternion.identity;
                    // cell.transform.localScale = Vector3.one;
                    var renderer = cell.GetComponent<Renderer>();
                    cells[i].Add(cell);
                    cellRenderers[i].Add(renderer);
                    cellMaterials[i].Add(new Material(renderer.material));
                }
            }
        }

        private void SetCellColor(GridLocation id, Color color)
        {
            var mat = cellMaterials[id.x][id.y];
            mat.color = color;
            cellRenderers[id.x][id.y].material = mat;
        }
        
        private void DrawGraphView(List<GridLocation> path, Dictionary<GridLocation, GridInfo> infoMap)
        {
            for (int x = 0; x < rangeX; x++)
            {
                for (int y = 0; y < rangeY; y++)
                {
                    GridLocation xy = new GridLocation(x, y);

                    var infoMono = cells[x][y].GetComponent<GridInfoMono>();
                    if (infoMap != null && infoMap.TryGetValue(xy, out var info))
                    {
                        infoMono.Set(info);
                    }
                    else
                    {
                        infoMono.Hide();
                    }
                    
                    if (!graph.Passable(xy))
                    {
                        SetCellColor(xy, colorWall);
                        continue;
                    }
                    if (xy == startPos)
                    {
                        SetCellColor(xy, colorStartPos);
                        continue;
                    }
                    if (xy == endPos)
                    {
                        SetCellColor(xy, colorEndPos);
                        continue;
                    }

                    var posIndex = path.IndexOf(xy);
                    if (posIndex > 0)
                    {
                        var percent = (float)posIndex / path.Count;
                        var color = Color.Lerp(colorStartPos, colorEndPos, percent);
                        SetCellColor(xy, color);
                    }
                    else
                    {
                        SetCellColor(xy, colorFloor);

                    }
                }
            }
        }

        #endregion
    }
}