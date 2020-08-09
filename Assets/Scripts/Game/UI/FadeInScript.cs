using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game.Entities;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Assets.Scripts.Game.UI
{
    public class FadeInScript : MonoBehaviour
    {
        // UI elements

        public Image MainCoverageImage;
        public Image PlayerColorImage;
        public Text ChangingText;
        public Text loadingText;

        private IEnumerator textChangingCoroutine;
        private Random rnd = new Random();
        /// <summary>
        /// Texts displayed to user during loading of the game
        /// </summary>
        private List<string> texts = new List<string>()
        {
            "harvesting Hawking radiation",
            "fighting Skynet",
            "establishing quantum network",
            "collecting human attention",
            "generating map generator",
            "solving Rubik's cube",
            "haunting ghosts",
            "haunting humans",
            "jitting JIT",
            "playing golf",
            "watching Netflix",
            "surfing internet",
            "watching kitten videos"
        };

        void Start()
        {
            ChangingText.text = "Initializing game";
            textChangingCoroutine = ChangeText();
            StartCoroutine(textChangingCoroutine);
        }

        /// <summary>
        /// Shows player color using image with given material
        /// </summary>
        /// <param name="playerId"></param>
        public void SetPlayerMaterial(int playerId)
        {
            PlayerColorImage.material = PlayerMaterials.GetPlayerMaterialByPlayerId(playerId);
        }

        /// <summary>
        /// Starts fading in to the game
        /// </summary>
        public void FadeIn()
        {
            StopCoroutine(textChangingCoroutine);
            StartCoroutine(FadeTo(0.0f, 3.5f));
        }

        /// <summary>
        /// Alpha fade of image
        /// </summary>
        /// <param name="aValue"></param>
        /// <param name="aTime"></param>
        /// <returns></returns>
        IEnumerator FadeTo(float aValue, float aTime)
        {
            float alpha = MainCoverageImage.color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
                MainCoverageImage.color = newColor;
                yield return null;
            }
            Destroy(PlayerColorImage);
            Destroy(ChangingText);
            Destroy(loadingText);
            Destroy(MainCoverageImage);
        }

        /// <summary>
        /// Changes text displayed to user while loading
        /// </summary>
        /// <returns></returns>
        IEnumerator ChangeText()
        {
            while (true)
            {
                yield return new WaitForSeconds(2.5f);
                if (texts.Count > 0)
                {
                    int rndNumber = rnd.Next(0, texts.Count);
                    ChangingText.text = texts[rndNumber];
                    texts.RemoveAt(rndNumber);
                }
                else
                {
                    ChangingText.text = "just waiting..";
                }
                
            }

            yield return null;
        }

    }
}
