using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinnerPlayerCard : MonoBehaviour
{
    public TextMeshProUGUI nameText, resourceText;
    public int playerNumber;
    public PlayerSetup player;
    
    public void Populate(PlayerSetup player)
    {
        playerNumber = player.PlayerNumber;
        this.player = player;
        var charInfo = NetworkGameController.instance.CharacterList[player.CharacterInfoIndex];
        
        nameText.SetText(charInfo.Name);
        resourceText.SetText($"Midia - {player.MediaResources} |" +
                         $" Politico - {player.PoliticalResources} |" +
                         $" Social - {player.SocialResources} |" +
                         $"Economico - {player.EconomicResources}");

        Debug.Log("Number: playerNumber");
    }

    public void Vote()
    {
        PlayerSetup.localPlayerSetup.VoteOnCharacter(playerNumber);
        NetworkGameUI.Instance.LocalLog("Voto feito.");
        NetworkGameUI.Instance.CloseVoteMenu();
    }
}
