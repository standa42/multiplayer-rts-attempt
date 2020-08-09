using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    /// <summary>
    /// Log for important things in gives scene
    /// Text field should be binded to it at the beggining of the scene (performed by LogTextInitializer in scene on certain Text object)
    /// </summary>
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
        private static int keptMessagesCount = 40;

        static Log()
        {
            messageList = new Queue<string>();
        }

        /// <summary>
        /// Adds message to log queue
        /// </summary>
        /// <param name="message"></param>
        public static void LogMessage(string message)
        {
            messageList.Enqueue(message);

            if (messageList.Count > keptMessagesCount)
            {
                messageList.Dequeue();
            }

            UpdateText();
        }

        /// <summary>
        /// Updates log messages in text field
        /// </summary>
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
