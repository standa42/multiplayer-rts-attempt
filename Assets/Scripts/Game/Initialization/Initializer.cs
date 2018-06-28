using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Game.Races;
using Assets.Scripts.Menu;
using Assets.Scripts.Game.NetworkConnection;
using UnityEngine;

namespace Assets.Scripts.Game.Initialization
{
    public class Initializer : MonoBehaviour
    {
        public delegate void NetworkCreationVariablesDelegate(List<Tuple<int, byte>> playerRaces, int mapId, int randomSeed);
        public delegate void NetworkStartGameDelegate();

        private NetworkCommunication networkCommunication;
        private MatchCreator matchCreator;
        private INetworkCreator networkCreator;
        private INetworkStarter networkStarter;

        private int myId;
        private int mapId;
        private int randomSeed;
        private List<Tuple<int, byte>> playerRaces;

        void Start()
        {
            StartInitilization();
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
            this.playerRaces = playerRaces;

            Log.LogMessage($"Network creation complete map:{mapId} seed: {randomSeed} races: {playerRaces}");

            CreateScene();
        }

        private void CreateScene()
        {
            Log.LogMessage("Scene creation started");
            // TODO scene creation



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

    }
}
