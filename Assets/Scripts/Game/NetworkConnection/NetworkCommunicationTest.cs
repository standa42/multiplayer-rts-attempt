using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.Multiplayer;
using System;
using Assets.Scripts.Common;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class NetworkCommunicationTest : MonoBehaviour, RealTimeMultiplayerListener
    {
        private static bool showingWaitingRoom = true;


        public void SignIn()
        {
            if (!PlayGamesPlatform.Instance.localUser.authenticated)
            {
                PlayGamesPlatform.Instance.localUser.Authenticate((bool success) =>
                {
                    if (success)
                    {
                        Log.LogMessage("Signing in succeded");
                    }
                    else
                    {
                        Log.LogMessage("Signing in Failed");
                    }
                });
            }
            else
            {
                Log.LogMessage("Player already signed in");
            }
        }

        public void StartGame()
        {
            Log.LogMessage("Game started");

            const int MinOpponents = 1, MaxOpponents = 1;
            const int GameVariant = 42;
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(MinOpponents, MaxOpponents, GameVariant, this);
        }

        private IEnumerator StartGameCoroutine()
        {
            yield return new WaitForSeconds(5);
            StartGame();
        }

        // Use this for initialization
        void Start()
        {
            SignIn();
            StartCoroutine(StartGameCoroutine());
        }

        //
        private byte playerNumber; // from sorted list of participant id's

        // Lists of things
        List<int> updateTimes = new List<int>();

        private float textsUpdate = 0;
        private float textsUpdateTime = 1.5f;
        Queue<string> texts = new Queue<string>();

        SentMessages[] unreliableOnes = new SentMessages[30000];
        SentMessages[] reliableOnes = new SentMessages[30000];

        // Sending
        private float startTime = 0;
        private float updateTimeTick = 0.2f;
        private float elapsedTimeCounter = 0f; // time since start, used for reliability change
        private float reliabilityChangeTime = 100f;

        private bool messageReliability = true;

        private int UnreliableMessagesSent = 0;
        private int ReliableMessagesSent = 0;

        // Recieving
        private int UnreliableMessagesRecieved = 0;
        private int ReliableMessagesRecieved = 0;

        // 
        private bool processing = false;

        private struct SentMessages
        {
            public int sendTime;
            public int receivedTime;
        }

        // Update is called once per frame
        void Update()
        {
            if (processing)
            {
                elapsedTimeCounter += Time.deltaTime;

                if (elapsedTimeCounter > updateTimeTick)
                {
                    // switch reliability
                    if (((Time.timeSinceLevelLoad - startTime) > reliabilityChangeTime) && messageReliability)
                    {
                        messageReliability = false;
                    }

                    // main loop
                    if ((Time.timeSinceLevelLoad - startTime) < (reliabilityChangeTime * 2))
                    {
                        updateTimes.Add((int)(elapsedTimeCounter * 1000));

                        if (messageReliability)
                        {
                            SendBothReliableAndUnreliableMessages(messageReliability, reliableOnes, ref ReliableMessagesSent);
                        }
                        else
                        {
                            SendBothReliableAndUnreliableMessages(messageReliability, unreliableOnes, ref UnreliableMessagesSent);
                        }

                    }
                    // ending
                    else
                    {
                        processing = false;
                    }

                    // reset time counter
                    elapsedTimeCounter = elapsedTimeCounter - updateTimeTick;
                }
            }

            // actualize text infos
            textsUpdate += Time.deltaTime;

            if (textsUpdate > textsUpdateTime)
            {
                updateTextInfo();
                textsUpdate = 0;
            }

        }

        void SendBothReliableAndUnreliableMessages(bool reliable, SentMessages[] field, ref int count)
        {
            field[count].sendTime = (int)(Time.timeSinceLevelLoad * 1000);

            byte[] intBytes = BitConverter.GetBytes(count);

            byte[] message = new byte[] { playerNumber, intBytes[0], intBytes[1], intBytes[2], intBytes[3] };

            SendMessageWithGooglePlay(reliable, message);

            //
            if (texts.Count > 10)
            {
                texts.Dequeue();
            }
            Log.LogMessage("Sent " + reliable + count);
            //

            count++;
        }

        private void DeterminePlayerNumber()
        {
            List<Participant> participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
            Participant myself = PlayGamesPlatform.Instance.RealTime.GetSelf();

            for (byte i = 0; i < participants.Count; i++)
            {
                if (participants[i].ParticipantId == myself.ParticipantId)
                {
                    playerNumber = i;
                }
            }

            Log.LogMessage("MyPlayerNumber is: " + playerNumber);
        }

        void SendMessageWithGooglePlay(bool reliable, byte[] message)
        {
            PlayGamesPlatform.Instance.RealTime.SendMessageToAll(reliable, message);
        }

        private void updateTextInfo()
        {
            string s = "";

            for (int i = texts.Count - 1; i >= 0; i--)
            {
                s += Environment.NewLine + texts.ElementAt(i);
            }


            ////////////////////////
            int approxUpdate = 0;
            int approxRel = 0;
            int approxUnrel = 0;

            if (updateTimes.Count != 0)
            {
                for (int i = 0; i < updateTimes.Count; i++)
                {
                    approxUpdate += updateTimes[i];
                }
                approxUpdate = approxUpdate / updateTimes.Count;
            }
            //
            if (ReliableMessagesRecieved != 0)
            {
                for (int i = 0; i < ReliableMessagesSent; i++)
                {
                    if ((reliableOnes[i].receivedTime != 0) && (reliableOnes[i].sendTime != 0))
                    {
                        approxRel += reliableOnes[i].receivedTime - reliableOnes[i].sendTime;
                    }
                }
                approxRel = approxRel / ReliableMessagesRecieved;
            }
            //
            if (UnreliableMessagesRecieved != 0)
            {
                for (int i = 0; i < UnreliableMessagesSent; i++)
                {
                    if ((unreliableOnes[i].receivedTime != 0) && (unreliableOnes[i].sendTime != 0))
                    {
                        approxUnrel += unreliableOnes[i].receivedTime - unreliableOnes[i].sendTime;
                    }
                }
                approxUnrel = approxUnrel / UnreliableMessagesRecieved;
            }


        }


        void RealTimeMultiplayerListener.OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
        {
            if (true) //data[0] == playerNumber
            {
                // message arrived
                byte[] bytes = new byte[] { data[1], data[2], data[3], data[4], };
                int number = BitConverter.ToInt32(bytes, 0);

                if (texts.Count > 10)
                {
                    texts.Dequeue();
                }

                if (isReliable)
                {
                    reliableOnes[number].receivedTime = (int)(Time.timeSinceLevelLoad * 1000);
                    ReliableMessagesRecieved++;
                    Log.LogMessage("Received reliable" + number);
                }
                else
                {
                    unreliableOnes[number].receivedTime = (int)(Time.timeSinceLevelLoad * 1000);
                    UnreliableMessagesRecieved++;
                    Log.LogMessage("Received unreliable" + number);
                }
            }
            else
            {
                // send message back to sender
                SendMessageWithGooglePlay(isReliable, data);
            }
        }



        void RealTimeMultiplayerListener.OnLeftRoom()
        {
        }

        void RealTimeMultiplayerListener.OnParticipantLeft(Participant participant)
        {
            Log.LogMessage("Participant Left");
        }

        void RealTimeMultiplayerListener.OnPeersConnected(string[] participantIds)
        {
            Log.LogMessage("Somebody Connected");
        }

        void RealTimeMultiplayerListener.OnPeersDisconnected(string[] participantIds)
        {
            Log.LogMessage("Somebody Disconnected");
        }

        void RealTimeMultiplayerListener.OnRoomConnected(bool success)
        {
            if (success)
            {
                Log.LogMessage("I have connected the room");

                DeterminePlayerNumber();
                processing = true;
                startTime = Time.timeSinceLevelLoad;
            }
            else
            {
                Log.LogMessage("error not-connected the room");
            }
        }

        void RealTimeMultiplayerListener.OnRoomSetupProgress(float percent)
        {
            if (showingWaitingRoom)
            {
                Log.LogMessage("Waiting room showed");
                //PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI();
            }
        }


    }


}