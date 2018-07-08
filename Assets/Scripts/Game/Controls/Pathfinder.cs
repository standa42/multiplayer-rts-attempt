using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Controls
{
    public static class Pathfinder
    {
        public static MoveDirection FindPath(Map map, Vector2Int myPosition, Vector2Int destinationPosition)
        {
            return Dijkstra(map, myPosition, destinationPosition);
        }

        private static Map currentMap;
        private static int[][] DistanceGrid;
        private static int[][] DestinationGrid;
        private static Queue<Vector2Int> q;

        private static MoveDirection Dijkstra(Map map, Vector2Int myPosition, Vector2Int destinationPosition)
        {
            q = new Queue<Vector2Int>();
            currentMap = map;

            DistanceGrid = new int[map.Size.X][];
            for (int i = 0; i < map.Size.X; i++)
            {
                DistanceGrid[i] = new int[map.Size.Y];
            }

            DestinationGrid = new int[map.Size.X][];
            for (int i = 0; i < map.Size.X; i++)
            {
                DestinationGrid[i] = new int[map.Size.Y];
            }

            // prepare distance grid
            for (int x = 0; x < map.Size.X; x++)
            {
                for (int y = 0; y < map.Size.Y; y++)
                {
                    if (myPosition.x == x && myPosition.y == y)
                    {
                        DistanceGrid[x][y] = 0;
                        q.Enqueue(new Vector2Int(x, y));
                    }
                    else
                    {
                        if (map.Entities[x][y] != null)
                        {
                            DistanceGrid[x][y] = int.MinValue;
                        }
                        else
                        {
                            DistanceGrid[x][y] = int.MaxValue;
                        }
                    }
                }
            }

            // fill distance grid
            while (q.Count != 0)
            {
                var node = q.Dequeue();

                //right
                ExpandDistanceGridToPosition(q, node.x, node.y, node.x + 1, node.y);
                //left
                ExpandDistanceGridToPosition(q, node.x, node.y, node.x - 1, node.y);
                //up
                ExpandDistanceGridToPosition(q, node.x, node.y, node.x, node.y + 1);
                //down
                ExpandDistanceGridToPosition(q, node.x, node.y, node.x, node.y - 1);

            }

            // distance grid obstacles to max int
            for (int x = 0; x < map.Size.X; x++)
            {
                for (int y = 0; y < map.Size.Y; y++)
                {
                    if (DistanceGrid[x][y] == int.MinValue)
                    {
                        DistanceGrid[x][y] = int.MaxValue;
                    }
                }
            }

            // prepare & fill destination grid
            for (int x = 0; x < map.Size.X; x++)
            {
                for (int y = 0; y < map.Size.Y; y++)
                {
                    if (x == myPosition.x && y == myPosition.y)
                    {
                        DestinationGrid[x][y] = ManhattanDistance(x, y, destinationPosition.x, destinationPosition.y);
                    }
                    else if (map.Entities[x][y] == null)
                    {
                        DestinationGrid[x][y] = ManhattanDistance(x, y, destinationPosition.x, destinationPosition.y);
                    }
                    else
                    {
                        DestinationGrid[x][y] = int.MaxValue;
                    }
                }
            }

            // find FinalNode
            Vector2Int finalNode = myPosition;
            int distanceValue = int.MaxValue;
            int destinationValue = int.MaxValue;

            for (int x = 0; x < map.Size.X; x++)
            {
                for (int y = 0; y < map.Size.Y; y++)
                {
                    if (DestinationGrid[x][y] == destinationValue && DistanceGrid[x][y] != int.MaxValue)
                    {
                        if (DistanceGrid[x][y] < distanceValue)
                        {
                            destinationValue = DestinationGrid[x][y];
                            distanceValue = DistanceGrid[x][y];
                            finalNode = new Vector2Int(x, y);
                        }
                    }
                    else if (DestinationGrid[x][y] < destinationValue && DistanceGrid[x][y] != int.MaxValue)
                    {
                        destinationValue = DestinationGrid[x][y];
                        distanceValue = DistanceGrid[x][y];
                        finalNode = new Vector2Int(x, y);
                    }
                }
            }

            if (distanceValue == int.MaxValue || destinationValue == int.MaxValue)
            {
                return MoveDirection.None;
            }

            if (myPosition.x == finalNode.x && myPosition.y == finalNode.y)
            {
                return MoveDirection.None;
            }

            // backtracking
            int currentValue = DistanceGrid[finalNode.x][finalNode.y];
            var currentNode = finalNode;
            while (currentValue != 1)
            {
                var neighborsList = new List<ValueTuple<int, Vector2Int>>();
                AddBetterNeighborToList(neighborsList, new Vector2Int(currentNode.x - 1, currentNode.y), currentValue);
                AddBetterNeighborToList(neighborsList, new Vector2Int(currentNode.x + 1, currentNode.y), currentValue);
                AddBetterNeighborToList(neighborsList, new Vector2Int(currentNode.x, currentNode.y - 1), currentValue);
                AddBetterNeighborToList(neighborsList, new Vector2Int(currentNode.x, currentNode.y + 1), currentValue);
                var sorted = neighborsList.OrderBy(x =>
                    ManhattanDistance(x.Item2.x, x.Item2.x, myPosition.x, myPosition.y)).First();

                currentValue = sorted.Item1;
                currentNode = sorted.Item2;
            }

            if (currentNode.x > myPosition.x)
            {
                return MoveDirection.Right;
            }

            if (currentNode.x < myPosition.x)
            {
                return MoveDirection.Left;
            }

            if (currentNode.y > myPosition.y)
            {
                return MoveDirection.Up;
            }

            if (currentNode.y < myPosition.y)
            {
                return MoveDirection.Down;
            }

            throw new NotImplementedException("Pathfinding failed");
        }

        private static void AddBetterNeighborToList(List<ValueTuple<int, Vector2Int>> list, Vector2Int position, int currentValue)
        {
            if (ValidPositionOnMap(position))
            {
                if (DistanceGrid[position.x][position.y] < currentValue)
                {
                    list.Add(new ValueTuple<int, Vector2Int>(DistanceGrid[position.x][position.y], position));
                }
            }
        }

        private static void ExpandDistanceGridToPosition(Queue<Vector2Int> q, int sx, int sy, int ex, int ey)
        {
            if (ValidPositionOnMap(ex, ey))
            {
                if (DistanceGrid[ex][ey] > (DistanceGrid[sx][sy] + 1))
                {
                    DistanceGrid[ex][ey] = DistanceGrid[sx][sy] + 1;
                    q.Enqueue(new Vector2Int(ex, ey));
                }
            }
        }

        private static int ManhattanDistance(int sx, int sy, int ex, int ey)
        {
            return Math.Abs(sx - ex) + Math.Abs(sy - ey);
        }

        private static bool ValidPositionOnMap(int x, int y)
        {
            if (x >= 0 && x < currentMap.Size.X && y >= 0 && y < currentMap.Size.Y)
            {
                return true;
            }

            return false;
        }

        private static bool ValidPositionOnMap(Vector2Int v)
        {
            return ValidPositionOnMap(v.x, v.y);
        }
    }
}
