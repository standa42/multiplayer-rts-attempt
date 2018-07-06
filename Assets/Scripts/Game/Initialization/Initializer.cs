//#define IS_ANDROID

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Menu;
using Assets.Scripts.Game.NetworkConnection;
using Assets.Scripts.Game.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Initialization
{
    public class Initializer : MonoBehaviour
    {
        public TouchInput touchInput;
        public ResourceDisplay resourceDisplay;

        public delegate void NetworkCreationVariablesDelegate(List<Tuple<int, byte>> playerRaces, int mapId, int randomSeed);
        public delegate void NetworkStartGameDelegate();

        private NetworkCommunication networkCommunication;
        private MatchCreator matchCreator;
        private INetworkCreator networkCreator;
        private INetworkStarter networkStarter;

        private int myId;
        private int mapId;
        private int randomSeed;
        private List<RaceEnum> playerRaces = new List<RaceEnum>();

        void Start()
        {
#if !IS_ANDROID
            myId = 0;
            networkCommunication = new NetworkCommunicationMock((uint)IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            EditorInitialization();
#endif

#if IS_ANDROID
            StartInitilization();
#endif
        }

        private void EditorInitialization()
        {
            myId = 0;

            this.mapId = 0;
            this.randomSeed = 42;
            this.playerRaces = new List<RaceEnum>();
            playerRaces.Add(RaceEnum.Universal);
            playerRaces.Add(RaceEnum.Second);

            SceneCreationEditor();
        }

        private void StartInitilization()
        {
            networkCommunication = new NetworkCommunication(((uint)IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame) - 1);
            matchCreator = new MatchCreator(networkCommunication);

            matchCreator.RoomReady += RoomIsPrepared;
            matchCreator.CreateMatch();
        }

        private void RoomIsPrepared(int myId)
        {
            Log.LogMessage("My is is: " + myId);

            this.myId = myId;

            if (myId == 0)
            {
                // Autoritative player
                networkCreator = new AutoritativeNetworkCreator(networkCommunication);
                networkStarter = new AutoritativeNetworkStarter(networkCommunication);
            }
            else
            {
                // Non-Autoritative player
                networkCreator = new NonAutoritativeNetworkCreator(networkCommunication);
                networkStarter = new NonAutoritativeNetworkStarter(networkCommunication);
            }

            // TODO: possible race condition, if sb sends packet before
            // sb else bind receive events - he wont get em
            StartCoroutine(DelayFunction(NetworkCreation, 1.5f));
        }

        IEnumerator DelayFunction(Action function, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            function();
        }

        private void NetworkCreation()
        {
            Log.LogMessage("Network creatin started");

            networkCreator.CreationVariablesAvailable += NetworkCreationComplete;
            networkCreator.Run(myId);
        }

        private void NetworkCreationComplete(List<Tuple<int, byte>> playerRaces, int mapId, int randomSeed)
        {
            this.mapId = mapId;
            this.randomSeed = randomSeed;
            var sortPlayerRaces = playerRaces.OrderBy(x => x.Item1);
            foreach (var playerRace in playerRaces)
            {
                this.playerRaces.Add((RaceEnum)playerRace.Item2);
            }

            Log.LogMessage($"Network creation complete map:{mapId} seed: {randomSeed} races: {playerRaces}");

            CreateScene();
        }

        private void CreateScene()
        {
            Log.LogMessage("Scene creation started");


            SceneCreationAndroid();


            AttempToStart();
        }

        private void AttempToStart()
        {
            Log.LogMessage("Attemping to start network");
            networkStarter.NetworkStartGame += FadeIn;
            networkStarter.Run(((int)IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame) - 1);
        }

        private void FadeIn()
        {
            // todo fade in to the scene
            Log.LogMessage("Fading in the scene");


        }




        private Simulation simulation = null;
        private bool debugMarker = false;

        private void SceneCreationAndroid()
        {
            Log.LogMessage("Scene creation start");
            CommonRandom.RandomGenerator = new System.Random(randomSeed);

            ResourcesManager resourcesManager = new ResourcesManager(myId, IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            resourcesManager.ResourcesUpdate += resourceDisplay.ActualAmount;
            resourcesManager.AddWoodToPlayer(myId, 0);

            var gameManager = new GameManager(myId, IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            var game = new Game(gameManager);

            Map map;
            if (IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame == NumberOfPlayersInGame.Two)
            {
                map = MapFactory.LoadMapFromFile("TestingTwo1", IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame, gameManager, playerRaces);
            }
            else
            {
                map = MapFactory.LoadMapFromFile("TestingFour1", IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame, gameManager, playerRaces);
            }
            var cameraMovement = new CameraControl(touchInput);

            var commandsHolder = new CommandsHolder(IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            var playerProxies = new PlayerProxies.PlayerProxies(networkCommunication, commandsHolder, myId,
                IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            var inputAutomata = new InputCommandAutomata(playerProxies, map, touchInput);

            this.simulation = new Simulation(commandsHolder, inputAutomata, game);

            simulation.Run();
            Log.LogMessage("Scene creation end");

        }

        private void SceneCreationEditor()
        {
            Log.LogMessage("Scene creation start");
            CommonRandom.RandomGenerator = new System.Random(randomSeed);

            ResourcesManager resourcesManager = new ResourcesManager(myId, IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            resourcesManager.ResourcesUpdate += resourceDisplay.ActualAmount;
            resourcesManager.AddWoodToPlayer(myId, 0);

            var gameManager = new GameManager(myId, IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            var game = new Game(gameManager);

            var map = MapFactory.LoadMapFromFile("TestingTwo1", NumberOfPlayersInGame.Two, gameManager, playerRaces);
            var cameraMovement = new CameraControl(touchInput);

            var commandsHolder = new CommandsHolder(IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            var playerProxies = new PlayerProxies.PlayerProxies(networkCommunication, commandsHolder, myId,
                IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            var inputAutomata = new InputCommandAutomataMock(playerProxies, map, touchInput);
            this.simulation = new Simulation(commandsHolder, inputAutomata, game);

            simulation.Run();
            Log.LogMessage("Scene creation end");
        }

        void Update()
        {
            try
            {
                simulation?.Update();
            }
            catch (Exception e)
            {
                //Log.LogMessage($"SimException: {e.Message} {Environment.NewLine} {e.StackTrace}");
                throw;
            }

        }




    }
}
