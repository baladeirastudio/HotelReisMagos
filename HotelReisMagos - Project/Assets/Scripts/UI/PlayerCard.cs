using TMPro;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI politicalText, economicText, socialText, mediaText;
    [SerializeField] private int playerSetupIndex;
    [SerializeField] private PlayerSetup trackedPlayer;
    
    public void RefreshInfo()
    {
        politicalText.SetText($"{trackedPlayer.PoliticalResources}");
        economicText.SetText($"{trackedPlayer.EconomicResources}");
        socialText.SetText($"{trackedPlayer.SocialResources}");
        mediaText.SetText($"{trackedPlayer.MediaResources}");
    }

    public void Track(PlayerSetup playerController)
    {
        trackedPlayer = playerController;
        
        RefreshInfo();
        playerSetupIndex = playerController.PlayerNumber;
    }
}