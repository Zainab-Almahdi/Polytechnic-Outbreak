using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public TMP_Text floorText;
    public TMP_Text healthText;
    public TMP_Text currentMagText;
    public TMP_Text reserveAmmoText;
    public TMP_Text[] playerScores;
    public TMP_Text[] playerNames;
    public void SetFloor(int floor)
    {
        floorText.text =  floor.ToString();
    }

    public void SetHealth(int hp)
    {
        healthText.text = hp.ToString();
    }

    public void SetAmmo(int current, int reserve)
    {
        currentMagText.text = current.ToString();
        reserveAmmoText.text = reserve.ToString();
    }

    public void SetScore(int playerIndex, int score)
    {
        playerScores[playerIndex].text = score.ToString();
    }

    public void SetPlayerName(int playerIndex, string name)
    {
        playerNames[playerIndex].text = name;
    }
}