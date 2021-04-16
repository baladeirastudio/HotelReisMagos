using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI politicalText, economicText, socialText, mediaText;
    [SerializeField] private int playerSetupIndex;
    [SerializeField] private PlayerSetup trackedPlayer;
    
    public void RefreshInfo()
    {
        StartCoroutine(RefreshInfoDelayed());
    }

    private IEnumerator RefreshInfoDelayed()
    {

        Debug.LogError($"Updating UI for player {trackedPlayer.PlayerNumber} - " +
                       $"Pol: {trackedPlayer.PoliticalResources} - " +
                       $"Eco: {trackedPlayer.EconomicResources} - " +
                       $"Soc: {trackedPlayer.SocialResources} - " +
                       $"Med: {trackedPlayer.MediaResources}");
        yield return new WaitForSeconds(0.5f);
        
        Debug.LogError($"New update for player {trackedPlayer.PlayerNumber} - " +
                       $"Pol: {trackedPlayer.PoliticalResources} - " +
                       $"Eco: {trackedPlayer.EconomicResources} - " +
                       $"Soc: {trackedPlayer.SocialResources} - " +
                       $"Med: {trackedPlayer.MediaResources}");
        
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