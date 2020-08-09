using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Game.Entities;
using Assets.Scripts.Menu;
using UnityEngine;
using Random = System.Random;
using Tree = Assets.Scripts.Game.Entities.Tree;

namespace Assets.Scripts.Game
{
    /// <summary>
    /// Creator of maps
    /// </summary>
    public class MapFactory
    {
        private static List<string> mapForFourPlayers;
        private static List<string> mapForTwoPlayers;

        static MapFactory()
        {
            mapForTwoPlayers = new List<string>();
            mapForFourPlayers = new List<string>();

            mapForTwoPlayers.Add("TestingTwo1");
            mapForTwoPlayers.Add("PathfindingTest");
            mapForFourPlayers.Add("TestingFour1");
        }

        /// <summary>
        /// Generates random map name based on existing maps and number of players
        /// </summary>
        /// <param name="number">number of players to play the game</param>
        /// <returns>map name that should be loaded</returns>
        public static string GetRandomMapName(NumberOfPlayersInGame number)
        {
            Random rnd = new Random();

            switch (number)
            {
                case NumberOfPlayersInGame.Two:
                    return (from map in mapForTwoPlayers orderby rnd.Next() select map).First();
                    break;
                case NumberOfPlayersInGame.Four:
                    return (from map in mapForFourPlayers orderby rnd.Next() select map).First();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(number), number, null);
            }
        }

        /// <summary>
        /// Loads map structure, generates natural units to gameManager, generates initial player entities
        /// </summary>
        /// <param name="name">map to be createds</param>
        /// <param name="numberOfPlayers">number of players</param>
        /// <param name="gameManager">game manager</param>
        /// <param name="playerRaces">races of players - sorted by player number (player network id)</param>
        /// <returns></returns>
        public static Map LoadMapFromFile(string name, NumberOfPlayersInGame numberOfPlayers, GameManager gameManager, List<RaceEnum> playerRaces)
        {
            // Loading
            TextAsset mapFile = Resources.Load(@"Map/Maps/" + name) as TextAsset;

            var stream = new StringReader(mapFile.ToString());

            stream.ReadLine();
            int x = int.Parse(stream.ReadLine().Split(' ')[1]);
            int y = int.Parse(stream.ReadLine().Split(' ')[1]);
            stream.ReadLine();

            var mapSize = new MapSize(x, y);

            stream.ReadLine();
            stream.ReadLine();

            var map = PrepareMap(mapSize);

            int playerId = 0;

            for (int _x = 0; _x < x; _x++)
            {
                for (int _y = 0; _y < y; _y++)
                {
                    string[] tokens = stream.ReadLine().Split(' ');
                    int tokenX = int.Parse(tokens[0]);
                    int tokenY = int.Parse(tokens[1]);
                    FieldAction tokenType = (FieldAction)int.Parse(tokens[2]);

                    switch (tokenType)
                    {
                        case FieldAction.TreeSpawn:
                            var treeSpawn = new TreeSpawn(-1,new Vector2Int(tokenX,tokenY),map,gameManager);
                            gameManager.TreeSpawns.Add(treeSpawn);
                            break;
                        case FieldAction.Tree:
                            var tree = new Tree(-1, new Vector2Int(tokenX, tokenY), map, gameManager);
                            map.AddEntityToPosition(tokenX, tokenY, tree);
                            gameManager.NaturalEntities.Add(tree);
                            break;
                        case FieldAction.Resources:
                            var resource = new Resource(-1, new Vector2Int(tokenX, tokenY), map, gameManager);
                            map.AddEntityToPosition(tokenX, tokenY, resource);
                            gameManager.NaturalEntities.Add(resource);
                            break;
                        case FieldAction.Obstacle:
                            var obstacle = new Obstacle(-1, new Vector2Int(tokenX, tokenY), map, gameManager);
                            map.AddEntityToPosition(tokenX, tokenY, obstacle);
                            gameManager.NaturalEntities.Add(obstacle);
                            break;
                        case FieldAction.Player:
                            for (int plX = tokenX; plX < tokenX+3; plX++)
                            {
                                for (int plY = tokenY; plY < tokenY+3; plY++)
                                {
                                    PlayerWorker unit;
                                    if (playerRaces[playerId] == RaceEnum.Cubes)
                                    {
                                        unit = new PlayerCubeWorker(playerId, new Vector2Int(plX, plY), map, gameManager);
                                    }
                                    else
                                    {
                                        unit = new PlayerSphereWorker(playerId, new Vector2Int(plX, plY), map, gameManager);
                                    }
                                    map.AddEntityToPosition(plX, plY, unit);
                                    gameManager.PlayerWorkers.Add(unit);
                                }
                            }

                            playerId++;
                            break;
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// Generates gameobjects for map and map class itself
        /// </summary>
        /// <param name="mapSize"></param>
        /// <returns></returns>
        private static Map PrepareMap(MapSize mapSize)
        {
            var ground = GameObject.Instantiate(Resources.Load("Map/ground", typeof(GameObject)) as GameObject);
            var gridMesh = GameObject.Instantiate(Resources.Load("Map/GridMesh") as GameObject);

            gridMesh.GetComponent<GridMesh>().Initialize(mapSize);

            gridMesh.transform.position = new Vector3(0, 0f, 0);
            gridMesh.transform.rotation = Quaternion.identity;
            gridMesh.transform.eulerAngles = new Vector3(-90, 0, 0);

            ground.transform.position = new Vector3(mapSize.X / 2f, mapSize.Y / 2f + 0.025f, 0.22f);
            ground.transform.rotation = Quaternion.identity;
            ground.transform.localScale = new Vector3(mapSize.X / 10f + 0.05f, 0, mapSize.Y / 10f + 0.01f);
            ground.transform.eulerAngles = new Vector3(-90, 0, 0);

            var map = new Map(mapSize, ground, gridMesh);

            return map;
        }
        
    }

    /// <summary>
    /// Reprezentation of map - holds both objects and GameObjects of map
    /// </summary>
    public class Map
    {
        public MapSize Size { get; }
        private GameObject ground;
        private GameObject gridMesh;

        public Entity[][] Entities;

        public Map(MapSize size, GameObject ground, GameObject gridMesh)
        {
            this.Size = size;
            this.ground = ground;
            this.gridMesh = gridMesh;

            Entities = new Entity[size.X][];
            for (int i = 0; i < size.X; i++)
            {
                Entities[i] = new Entity[size.Y];
            }
        }

        public void AddEntityToPosition(int x, int y, Entity entity)
        {
            Entities[x][y] = entity;
        }
    }

    public struct MapSize
    {
        public MapSize(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }

    /// <summary>
    /// Types of fields on map, share with map editor
    /// </summary>
    public enum FieldAction : byte
    {
        TreeSpawn = 5,
        Tree = 4,
        Resources = 3,
        Obstacle = 2,
        Clear = 0,
        Player = 240,
        None = 1
    }

    /// <summary>
    /// Movement direction in four axes
    /// </summary>
    public enum MoveDirection
    {
        Left,
        Right,
        Up,
        Down,
        None
    }

}