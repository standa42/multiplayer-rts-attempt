using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    public static class Log
    {
        private static Text _logTextInstance;

        /// <summary>
        /// Member to register logging text field in given scene
        /// </summary>
        public static Text LogTextInstance
        {
            get
            {
                return _logTextInstance;

            }
            set
            {
                _logTextInstance = value;
                messageList.Clear();
            }
        }

        private static Queue<string> messageList;
        private static int keptMessagesCount = 25;

        static Log()
        {
            messageList = new Queue<string>();
        }

        public static void LogMessage(string message)
        {
            messageList.Enqueue(message);

            if (messageList.Count > keptMessagesCount)
            {
                messageList.Dequeue();
            }

            UpdateText();
        }

        private static void UpdateText()
        {
            if (LogTextInstance != null)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var message in messageList.Reverse())
                {
                    sb.Append(message);
                    sb.Append(Environment.NewLine);
                }

                LogTextInstance.text = sb.ToString();
            }
        }
    }
}
