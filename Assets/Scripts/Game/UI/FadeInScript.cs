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
        public Image MainCoverageImage;
        public Image PlayerColorImage;
        public Text ChangingText;
        public Text loadingText;

        private IEnumerator textChangingCoroutine;
        private Random rnd = new Random();
        private List<string> texts = new List<string>()
        {
            "initializing game",
            "harvesting Hawking radiation",
            "fighting Skynet",
            "establishing quantum network",
            "determining determinizer",
            "collecting human attention",
            "generating map generator",
            "solving Rubik's cube",
            "haunting ghosts",
            "haunting humans",
            "jitting JIT",
            "playing golf",
            "watching Netflix",
            "surfing internet",
            "watching kittens"
        };

        void Start()
        {
            ChangingText.text = texts[0];
            textChangingCoroutine = ChangeText();
            StartCoroutine(textChangingCoroutine);
        }

        public void SetPlayerMaterial(int playerId)
        {
            PlayerColorImage.material = PlayerMaterials.GetPlayerMaterialByPlayerId(playerId);
        }

        public void FadeIn()
        {
            StopCoroutine(textChangingCoroutine);
            StartCoroutine(FadeTo(0.0f, 3.5f));
        }

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
