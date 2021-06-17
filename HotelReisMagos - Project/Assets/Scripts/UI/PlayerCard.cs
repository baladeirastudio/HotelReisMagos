using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI politicalText, economicText, socialText, mediaText;
    [SerializeField] private Image background, selectionFrame, isMyPlayerIcon;

    [SerializeField] private int playerSetupIndex;
    [SerializeField] private PlayerSetup trackedPlayer;

    private void Awake()
    {
        NetworkGameController.OnChangePlayerTurnId += NetworkGameControllerOnOnChangePlayerTurnId;
        NetworkGameController.onLoadFakeScene += OnLoadFakeScene;
    }

    private void OnLoadFakeScene()
    {
        RefreshInfo();
    }

    private void NetworkGameControllerOnOnChangePlayerTurnId(int old, int newVal)
    {
        /*(if (newVal == playerSetupIndex)
        {
            selectionFrame.gameObject.SetActive(true);
        }
        else
        {
            selectionFrame.gameObject.SetActive(false);
        })*/
    }

    public void RefreshInfo()
    {
        if(gameObject.activeInHierarchy)
            StartCoroutine(RefreshInfoDelayed());
    }

    private IEnumerator RefreshInfoDelayed()
    {

        /*Debug.LogError($"Updating UI for player {trackedPlayer.PlayerNumber} - " +
                       $"Pol: {trackedPlayer.PoliticalResources} - " +
                       $"Eco: {trackedPlayer.EconomicResources} - " +
                       $"Soc: {trackedPlayer.SocialResources} - " +
                       $"Med: {trackedPlayer.MediaResources}");
        
        Debug.LogError($"New update for player {trackedPlayer.PlayerNumber} - " +
                       $"Pol: {trackedPlayer.PoliticalResources} - " +
                       $"Eco: {trackedPlayer.EconomicResources} - " +
                       $"Soc: {trackedPlayer.SocialResources} - " +
                       $"Med: {trackedPlayer.MediaResources}");*/
        yield return new WaitForSeconds(0.5f);

        Color playerColor = trackedPlayer.MyColor;
        playerColor.a = 0.2f;
        background.color = playerColor;
        nameText.SetText(NetworkGameController.instance.CharacterList[trackedPlayer.CharacterInfoIndex].Name);
        // TODO: Forma mais rápida e fácil de botar isso
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
        if (trackedPlayer.hasAuthority)
        {
            isMyPlayerIcon.gameObject.SetActive(true);
        }


    }
}