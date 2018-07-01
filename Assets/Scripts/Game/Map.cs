using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Menu;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Game
{
    public class MapFactory
    {
        private static List<string> mapForFourPlayers;
        private static List<string> mapForTwoPlayers;

        static MapFactory()
        {
            mapForTwoPlayers = new List<string>();
            mapForFourPlayers = new List<string>();

            mapForTwoPlayers.Add("TestingTwo1");
            mapForFourPlayers.Add("TestingFour1");
        }

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

        public static Map LoadMapFromFile(string name, NumberOfPlayersInGame numberOfPlayers)
        {
            // Loading
            TextAsset mapFile = Resources.Load(@"Map/Maps/" + name) as TextAsset;

            var stream = new StringReader(mapFile.ToString());

            stream.ReadLine();
            int x = int.Parse(stream.ReadLine().Split(' ')[1]);
            int y = int.Parse(stream.ReadLine().Split(' ')[1]);
            stream.ReadLine();

            var mapSize = new MapSize(x,y);

            stream.ReadLine();
            stream.ReadLine();

            for (int _x = 0; _x < x; _x++)
            {
                for (int _y = 0; _y < y; _y++)
                {
                    string[] tokens = stream.ReadLine().Split(' ');
                    int tokenX = int.Parse(tokens[0]);
                    int tokenY = int.Parse(tokens[1]);
                    FieldAction tokenType = (FieldAction) int.Parse(tokens[2]);
                    /*
                    map.typeGrid[tokenX, tokenY] = FieldAction.Clear;

                    if (tokenType != FieldAction.Clear)
                    {
                        PlaceEntityOnPosition(tokenType, tokenX, tokenY);
                    }
                    */
                }
            }

            var map = PrepareMap(mapSize);

            return map;
        }

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


        public class Map
        {
            public MapSize Size { get; }
            private GameObject ground;
            private GameObject gridMesh;

            public Map(MapSize size, GameObject ground, GameObject gridMesh)
            {
                this.Size = size;
                this.ground = ground;
                this.gridMesh = gridMesh;
            }
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
    
}