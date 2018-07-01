using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using NetworkObjects;
using NetworkObjects.Commands;

namespace Assets.Scripts.Game.NetworkConnection
{
    public class NetworkCommunication : RealTimeMultiplayerListener
    {
        public ProtoBuffCommandReceiver Receiver { get; set; }
        public ProtoBuffCommandSender Sender { get; set; }

        private uint gameVariant;
        private uint opponentsCount;

        private IdsConverter idsConverter;

        public delegate void StringPassingDelegate(string value);
        public delegate void IntPassingDelegate(int value);
        public delegate void ProtoByteDelegate(int id, byte[] data);

        public event IntPassingDelegate RoomConnected;
        public event StringPassingDelegate RoomFailed;
        public event ProtoByteDelegate ReceivedMessage;
        
        public NetworkCommunication(uint opponents)
        {
            opponentsCount = opponents;
            gameVariant = 42 + opponents;

            Receiver = new ProtoBuffCommandReceiver(this);
            Sender = new ProtoBuffCommandSender();
        }

        #region Input functions

        public void CreateQuickMatch()
        {
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(opponentsCount, opponentsCount, gameVariant, this);
        }

        public virtual void SendCommands(List<Command> commands)
        {
            Log.LogMessage("Sending Commands!");
            this.SendMessages(new List<Packet>()
            {
                new CommandsPacket {Commands = commands}
            });
        }

        public virtual void SendMessages(List<Packet> packetList)
        {
            try
            {
                PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, Sender.WrapCommandMessages(packetList));
            }
            catch (Exception e)
            {
                Log.LogMessage($"Exception while sending msg {e.Message}");
                throw;
            }
            
        }

        public void LeaveRoom()
        {
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        }

        #endregion
        
        #region Google Interface implementation

        void RealTimeMultiplayerListener.OnRoomSetupProgress(float percent)
        {
            Log.LogMessage("Waiting room set up");

            // disable because it was freezing when one user connected significantly later than the other
            //PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI();
        }

        void RealTimeMultiplayerListener.OnRoomConnected(bool success)
        {
            if (success)
            {
                Log.LogMessage("OnRoomConnected succeeded");
                Log.LogMessage("My player number is: " + DeterminePlayerNumber());

                RoomConnected?.Invoke(DeterminePlayerNumber());
            }
            else
            {
                Log.LogMessage("OnRoomConnected failed");
            }
        }

        void RealTimeMultiplayerListener.OnLeftRoom()
        {
            Log.LogMessage("I have left the room");
            RoomFailed?.Invoke("I have left the room");
        }

        void RealTimeMultiplayerListener.OnParticipantLeft(Participant participant)
        {
            Log.LogMessage("Participant Left");
            RoomFailed?.Invoke("Participant Left");
        }

        void RealTimeMultiplayerListener.OnPeersConnected(string[] participantIds)
        {
            Log.LogMessage("Somebody Connected");
        }

        void RealTimeMultiplayerListener.OnPeersDisconnected(string[] participantIds)
        {
            Log.LogMessage("Somebody Disconnected");
            RoomFailed?.Invoke("Somebody Disconnected");
        }

        void RealTimeMultiplayerListener.OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
        {
            ReceivedMessage?.Invoke(idsConverter.GoogleToMy(senderId), data);
        }

        #endregion
        
        #region Helper functions

        private int DeterminePlayerNumber()
        {
            List<Participant> participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
            Participant myself = PlayGamesPlatform.Instance.RealTime.GetSelf();

            int searchedId = -1;

            for (byte i = 0; i < participants.Count; i++)
            {
                Log.LogMessage("Found participant number: " + i + " " + participants[i].ParticipantId);

                if (participants[i].ParticipantId == myself.ParticipantId)
                {
                    Log.LogMessage("myself id: " + i + " " + myself.ParticipantId);
                    searchedId = i;
                }
            }

            CreateIdsConverter();

            if (searchedId != -1)
            {
                return searchedId;
            }
            else
            {
                throw new Exception("DeterminePlayerNumber returned -1");
            }
        }

        private void CreateIdsConverter()
        {
            List<Participant> participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();

            var googleIdToMyIdDictionary = new Dictionary<string, int>();
            var myIdToGoogleIdDictionary = new Dictionary<int, string>();

            for (byte i = 0; i < participants.Count; i++)
            {
                googleIdToMyIdDictionary.Add(participants[i].ParticipantId, i);
                myIdToGoogleIdDictionary.Add(i, participants[i].ParticipantId);
            }

            idsConverter = new IdsConverter(googleIdToMyIdDictionary, myIdToGoogleIdDictionary);
        }

        internal class IdsConverter
        {
            private Dictionary<string, int> googleIdToMyIdDictionary;
            private Dictionary<int, string> myIdToGoogleIdDictionary;

            public IdsConverter(Dictionary<string, int> googleIdToMyIdDictionary, Dictionary<int, string> myIdToGoogleIdDictionary)
            {
                this.googleIdToMyIdDictionary = googleIdToMyIdDictionary;
                this.myIdToGoogleIdDictionary = myIdToGoogleIdDictionary;
            }

            public int GoogleToMy(string stringId)
            {
                int intId;
                googleIdToMyIdDictionary.TryGetValue(stringId, out intId);
                return intId;
            }

            public string MyToGoogle(int intId)
            {
                string stringId;
                myIdToGoogleIdDictionary.TryGetValue(intId, out stringId);
                return stringId;
            }
        }
        #endregion
    }




}
