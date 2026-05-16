using TMPro;
using UnityEngine;

namespace Assets.UI.Scripts
{
    public class HUDManager : MonoBehaviour
    {
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

        //TODO: add interact prompt

        [Header("Perks")] [SerializeField] private GameObject PerksContainer;

        // TODO: make a hashmap of the perks and their icons from UI/icons/Perks Icons

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
    }
}
