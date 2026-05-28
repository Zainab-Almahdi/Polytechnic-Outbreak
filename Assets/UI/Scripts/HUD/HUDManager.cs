using System;
using TMPro;
using UnityEngine;

namespace Assets.UI.Scripts
{
    public class HUDManager : MonoBehaviour
    {
        public static HUDManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        [Header("Floor")]
        [SerializeField] private TMP_Text floorLabelText;

        [Header("Money")]
        [SerializeField] private TMP_Text player1MoneyLabelText;
        [SerializeField] private TMP_Text player2MoneyLabelText;

        [Header("Health")]
        [SerializeField] private TMP_Text healthValueText;

        [Header("Ammo")]
        [SerializeField] private TMP_Text currentMagValueText;
        [SerializeField] private TMP_Text reserveMagValueText;

        [Header("Objective")]
        [SerializeField] private TMP_Text objectiveTextLabel;

        [Header("Interact")]
        [SerializeField] private TMP_Text interactTextLabel;


        [Header("Perks")] [SerializeField] private GameObject PerksContainer;


        private void Start()
        {
            if (interactTextLabel!= null)//default to hidden
            {
                interactTextLabel.gameObject.SetActive(false);
            }

            StartCoroutine(InitialFadeIn());
        }

        private System.Collections.IEnumerator InitialFadeIn()
        {
            // Create a temporary black overlay for the fade-in effect
            GameObject fadeObj = new GameObject("InitialFadeOverlay");
            fadeObj.transform.SetParent(this.transform.parent, false); // Parent to 'HUD' or 'Gampelay Canvas'
            fadeObj.transform.SetAsLastSibling(); // Ensure it's on top

            UnityEngine.UI.Image fadeImage = fadeObj.AddComponent<UnityEngine.UI.Image>();
            fadeImage.color = Color.black;

            RectTransform rect = fadeImage.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            float duration = 2f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            Destroy(fadeObj);
        }

        public void SetFloorLabel(string value)
        {
            if (floorLabelText != null)
            {
                floorLabelText.text = value;
            }
        }

        public void SetPlayer1MoneyLabel(string value)
        {
            if (player1MoneyLabelText != null)
            {
                player1MoneyLabelText.text = value;
            }
        }

        public void SetPlayer2MoneyLabel(string value)
        {
            if (player2MoneyLabelText != null)
            {
                player2MoneyLabelText.text = value;
            }
        }

        public void SetHealthValue(string value)
        {
            if (healthValueText != null)
            {
                healthValueText.text = value;
            }
        }

        public void SetCurrentMagValue(string value)
        {
            if (currentMagValueText != null)
            {
                currentMagValueText.text = value;
            }
        }

        public void SetReserveMagValue(string value)
        {
            if (reserveMagValueText != null)
            {
                reserveMagValueText.text = value;
            }
        }

        public void SetObjectiveText(string value)
        {
            if (objectiveTextLabel != null)
            {
                objectiveTextLabel.text = value;
            }
        }

        public void SetInteractText(string value, bool visible)
        {
            if (interactTextLabel == null)
            {
                return;
            }

            interactTextLabel.gameObject.SetActive(visible);

            if (visible)
            {
                interactTextLabel.text = value;
            }
        }
    }
}
