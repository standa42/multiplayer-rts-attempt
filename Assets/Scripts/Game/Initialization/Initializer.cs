#define IS_ANDROID

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
        // Gameobject scripts binded in scene

        public TouchInput touchInput;
        public ResourceDisplay resourceDisplay;
        public FadeInScript FadeInScript;

        // Callback delegates used in network creation

        public delegate void NetworkCreationVariablesDelegate(List<Tuple<int, byte>> playerRaces, int mapId, int randomSeed);
        public delegate void NetworkStartGameDelegate();

        // Object for network creation/communication

        private NetworkCommunication networkCommunication;
        private MatchCreator matchCreator;
        private INetworkCreator networkCreator;
        private INetworkStarter networkStarter;

        // Variables gathered from network

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


        /// <summary>
        /// Mocks network initialization for editor
        /// </summary>
        private void EditorInitialization()
        {
            myId = 0;

            this.mapId = 0;
            this.randomSeed = 42;
            this.playerRaces = new List<RaceEnum>();
            playerRaces.Add(RaceEnum.Cubes);
            playerRaces.Add(RaceEnum.Spheres);

            SceneCreationEditor();
            FadeInScript.SetPlayerMaterial(myId);
            FadeInScript.FadeIn();
        }

        /// <summary>
        /// Creates match creator and network communication
        /// and sets callback for the time when room is safely created and this can continue
        /// </summary>
        private void StartInitilization()
        {
            networkCommunication = new NetworkCommunication(((uint)IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame) - 1);
            matchCreator = new MatchCreator(networkCommunication);

            matchCreator.RoomReady += RoomIsPrepared;
            matchCreator.CreateMatch();
        }

        /// <summary>
        /// Action made based on succesfull room creation
        /// </summary>
        /// <param name="myId">network and game id of device player</param>
        private void RoomIsPrepared(int myId)
        {
            Log.LogMessage("My is is: " + myId);
            Log.LogMessage("FadeInScript: setting material");
            FadeInScript.SetPlayerMaterial(myId);
            Log.LogMessage("Material set");

            this.myId = myId;

            if (myId == 0)
            {
                // if Autoritative player
                networkCreator = new AutoritativeNetworkCreator(networkCommunication);
                networkStarter = new AutoritativeNetworkStarter(networkCommunication);
            }
            else
            {
                // if Non-Autoritative player
                networkCreator = new NonAutoritativeNetworkCreator(networkCommunication);
                networkStarter = new NonAutoritativeNetworkStarter(networkCommunication);
            }

            // TODO: possible race condition, if sb sends packet before
            // sb else bind receive events - he wont get em
            StartCoroutine(DelayFunction(NetworkCreation, 2.5f));
        }

        /// <summary>
        /// Function for delaying given action
        /// </summary>
        /// <param name="function"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        IEnumerator DelayFunction(Action function, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            function();
        }

        /// <summary>
        /// Creates network creation objects -> they exchange player races, mapId and random seed
        /// </summary>
        private void NetworkCreation()
        {
            Log.LogMessage("Network creatin started");

            networkCreator.CreationVariablesAvailable += NetworkCreationComplete;
            networkCreator.Run(myId);
        }

        /// <summary>
        /// Callback for the time when races, mapId and random seed agreed and synchronized by all players
        /// </summary>
        /// <param name="playerRaces"></param>
        /// <param name="mapId"></param>
        /// <param name="randomSeed"></param>
        private void NetworkCreationComplete(List<Tuple<int, byte>> playerRaces, int mapId, int randomSeed)
        {
            this.mapId = mapId;
            this.randomSeed = randomSeed;
            var sortPlayerRaces = playerRaces.OrderBy(x => x.Item1);
            foreach (var playerRace in sortPlayerRaces)
            {
                this.playerRaces.Add((RaceEnum)playerRace.Item2);
            }

            Log.LogMessage($"Network creation complete map:{mapId} seed: {randomSeed} races: {playerRaces}");

            CreateScene();
        }

        /// <summary>
        /// Creates the whole game scene and than tries to start the game
        /// </summary>
        private void CreateScene()
        {
            Log.LogMessage("Scene creation started");
            
            SceneCreationAndroid();
            
            AttempToStart();
        }

        /// <summary>
        /// Creates object that handles synchonization of game start
        /// </summary>
        private void AttempToStart()
        {
            Log.LogMessage("Attemping to start network");
            networkStarter.NetworkStartGame += FadeIn;
            networkStarter.Run(((int)IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame) - 1);
        }

        /// <summary>
        /// Callback that starts fading in to the scene
        /// </summary>
        private void FadeIn()
        {
            Log.LogMessage("Fading in the scene");

            FadeInScript.FadeIn();
        }




        private Simulation simulation = null;
        private bool debugMarker = false;

        /// <summary>
        /// Handles the whole scene creation process and object creation and dependencies on android platform
        /// </summary>
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
                map = MapFactory.LoadMapFromFile("Showtime", IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame, gameManager, playerRaces);
            }
            else
            {
                map = MapFactory.LoadMapFromFile("TestingFour1", IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame, gameManager, playerRaces);
            }
            var cameraMovement = new CameraControl(touchInput);

            var commandsHolder = new CommandsHolder(IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            var playerProxies = new PlayerProxies.PlayerProxies(networkCommunication, commandsHolder, myId,
                IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            var inputAutomata = new InputCommandAutomata(myId, playerProxies, map, touchInput, gameManager);

            this.simulation = new Simulation(commandsHolder, inputAutomata, game);

            simulation.Run();
            Log.LogMessage("Scene creation end");

        }

        /// <summary>
        /// Handles the whole scene creation process in editor
        /// </summary>
        private void SceneCreationEditor()
        {
            Log.LogMessage("Scene creation start");
            CommonRandom.RandomGenerator = new System.Random(randomSeed);

            ResourcesManager resourcesManager = new ResourcesManager(myId, IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            resourcesManager.ResourcesUpdate += resourceDisplay.ActualAmount;
            resourcesManager.AddWoodToPlayer(myId, 0);

            var gameManager = new GameManager(myId, IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            var game = new Game(gameManager);

            var map = MapFactory.LoadMapFromFile("Showtime", NumberOfPlayersInGame.Two, gameManager, playerRaces);
            var cameraMovement = new CameraControl(touchInput);

            var commandsHolder = new CommandsHolder(IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            var playerProxies = new PlayerProxies.PlayerProxies(networkCommunication, commandsHolder, myId,
                IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame);
            var inputAutomata = new InputCommandAutomataMock(myId, playerProxies, map, touchInput, gameManager);
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
