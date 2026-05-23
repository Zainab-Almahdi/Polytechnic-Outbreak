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

        //TODO: add interact prompt

        [Header("Perks")] [SerializeField] private GameObject PerksContainer;


        private void Start()
        {
            if (interactTextLabel!= null)//default to hidden
            {
                interactTextLabel.gameObject.SetActive(false);
            }

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
