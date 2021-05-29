using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class NetworkGameUI : NetworkBehaviour
{
    private static NetworkGameUI instance;

    public static NetworkGameUI Instance => instance;
    
    [SerializeField] private PlayerCard playerCardPrefab;
    [SerializeField] private Transform playerListGroup;
    [SerializeField] private GameObject actionMenu;
    [SerializeField] private Button useLuckCard;
    [SerializeField] private Button finishTurn;
    [SerializeField] private Button tradeCard;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private Scrollbar logScroll;
    [SerializeField] private List<PlayerCard> playerCards;

    [SerializeField] private RectTransform playerCardList, targetCardList, playerList;
    [SerializeField] private ResourceCardUI cardPrefab;
    [SerializeField] private List<ResourceCardUI> selectedCards;

    [SerializeField] private List<PlayerSetup> players = new List<PlayerSetup>();
    [SerializeField] private GameObject tradeMenu;
    [SerializeField] private PlayerCharacterCard playerCharCardPrefab;
    [SerializeField] private PlayerCharacterCard chosenTradePlayer;

    public List<ResourceCardUI> SelectedCards => selectedCards;
    
    public ResourceCardUI CardPrefab => cardPrefab;
    
    public RectTransform PlayerCardList => playerCardList;

    public RectTransform TargetCardList => targetCardList;

    public RectTransform PlayerList => playerList;

    private void Awake()
    {
        instance = this;

        NetworkGameController.OnChangePlayerTurnId += UpdateGameState;
        NetworkGameController.OnChangeCurrentAct += UpdateTradeButton;
    }

    private void UpdateTradeButton(int old, int newval)
    {
        //Debug.LogError($"New value: {newval}");
        
        if (newval >= 2)
        {
            tradeCard.interactable = true;
        }
    }

    private void UpdateGameState(int old, int val)
    {
        RpcUpdateActionsMenu();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        StartCoroutine(SetupActionsMenu());
    }

    private IEnumerator SetupActionsMenu()
    {
        yield return new WaitUntil( () => { return NetworkGameController.instance && PlayerSetup.localPlayerSetup; });
        
        //Setting up logText
        LocalLog(NetworkGameController.instance.FirstActData.actBeginText.text);
        //LocalLog($"Nesta partida, você será o empresário {NetworkGameController.instance.CharacterList[PlayerSetup.localPlayerSetup.CharacterInfoIndex].Name}. \n'{NetworkGameController.instance.CharacterList[PlayerSetup.localPlayerSetup.CharacterInfoIndex].Description}'");
        //TODO: Esse trecho de cima também dá câncer
        
        if (NetworkGameController.instance.PlayerTurnID == PlayerSetup.localPlayerSetup.PlayerNumber)
        {
            if (PlayerSetup.localPlayerSetup.hasAuthority)
            {
                actionMenu.SetActive(true);
                EnableActions();
            }
        }
        else
        {
            actionMenu.SetActive(false);
        }

    }

    private IEnumerator PopulateCharacterCards()
    {
        for (int j = 0; j < playerList.childCount; j++)
        {
            Destroy(targetCardList.GetChild(j).gameObject);//TODO: Isso dá cancer, assim como outras partes desse código
        }
        
        for (int i = 0; i < PlayerSetup.playerControllers.Count; i++)
        {

            if (PlayerSetup.playerControllers[i])
            {
                var playerChar = Instantiate(playerCharCardPrefab, playerList);
                yield return new WaitUntil(() =>
                {
                    return PlayerSetup.playerControllers[i].CharacterInfoIndex != -1;
                });

                playerChar.Populate(i, PlayerSetup.playerControllers[i]);
            }
        }
    }

    private void EnableActions()
    {
        useLuckCard.interactable = PlayerSetup.localPlayerSetup.LuckCardAmount > 0 ;
        finishTurn.interactable = false;

        //if(PlayerSetup.localPlayerSetup)
    }

    [ClientRpc]
    public void RpcRefreshPlayerCards()
    {
        for (int i = 0; i < players.Count; i++)
        {
            playerCards[i].RefreshInfo();
        }
    }

    [ClientRpc]
    public void RpcRefreshPlayerCard(int index)
    {
        playerCards[index].RefreshInfo();
    }

    [ClientRpc]
    public void RpcSpawnPlayer(List<NetworkIdentity> newPlayers)
    {
        players.Clear();
        playerCards.Clear();
        //Debug.LogError(players);
        for (int i = 0; i < newPlayers.Count; i++)
        {
            try
            {
                if(newPlayers[i])
                    //Debug.LogError($"There is no player at index {i}. This will fail.");
                players.Add(newPlayers[i].GetComponent<PlayerSetup>());
            }
            catch (Exception e)
            {
                //Debug.LogException(e);
            }

            //CmdPlayersAddPlayer(player);
        }

        for (int i = 0; i < playerListGroup.childCount; i++)
        {
            Destroy(playerListGroup.GetChild(i).gameObject);
        }

        for (int i = 0; i < players.Count; i++)
        {
            TrackPlayer(i);
        }
    }
    
    private void TrackPlayer(int index)
    {
        if (players[index])
        {
            var newEntry = Instantiate(playerCardPrefab, playerListGroup);
            newEntry.Track(players[index]);
            playerCards.Add(newEntry);
            //PlayerSetup.playerControllers[index].onChangeName += UpdatePlayerName;
        }
    }

    [ClientRpc]
    public void RpcEnableFinishRound()
    {
        if (PlayerSetup.localPlayerSetup.hasAuthority)
        {
            finishTurn.interactable = true;
            useLuckCard.interactable = false;
            //Debug.LogError("Has auth.");
        }
        else
        {
            //Debug.LogError("Not reaLLY THE CASEEEEE");    
        }
    }

    public void FinishTurn()
    {
        finishTurn.interactable = false;
        PlayerSetup.localPlayerSetup.AdvanceTurn();
    }

    //[ClientRpc]
    public void RpcUpdateActionsMenu()
    {
        /*Debug.LogError($"Current playerturn: {NetworkGameController.instance.PlayerTurnID} - " +
                       $"Player number: {PlayerSetup.localPlayerSetup.PlayerNumber}");*/
        if (NetworkGameController.instance.PlayerTurnID == PlayerSetup.localPlayerSetup.PlayerNumber)
        {
            if (PlayerSetup.localPlayerSetup.hasAuthority)
            {
                actionMenu.SetActive(true);
                EnableActions();
            }
        }
        else
        {
            actionMenu.SetActive(false);
        }
    }

    [ClientRpc]
    public void RpcLog(string text)
    {
        LocalLog(text);
    }

    public void LocalLog(string text)
    {
        
        if (logText.text != String.Empty)
        {
            logText.SetText(logText.text + "\n" + text);
            StartCoroutine(ResetLog());
        }
        else
        {
            logText.SetText(text.ToString());
        }
        Debug.Log($"Logging: {text}");

    }

    private IEnumerator ResetLog()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        logScroll.value = 0.0f;
    }
    
    public void UseLuckCard()
    {
        useLuckCard.interactable = false;
        PlayerSetup.localPlayerSetup.UseLuckCard();
    }

    public void EnableTradeMenu()
    {
        actionMenu.SetActive(false);
        tradeMenu.SetActive(true);
        PlayerSetup.localPlayerSetup.EnableTradeMenu();
        StartCoroutine(PopulateCharacterCards());
    }

    public void ReturnFromTradeMenu()
    {
        actionMenu.SetActive(true);
        tradeMenu.SetActive(false);
        chosenTradePlayer.Deselect();
    }

    public void SelectTradePlayer(PlayerCharacterCard playerCharacterCard)
    {
        if (chosenTradePlayer)
        {
            chosenTradePlayer.Deselect();
            chosenTradePlayer = playerCharacterCard;
            chosenTradePlayer.Select();
        }
        else
        {
            chosenTradePlayer = playerCharacterCard;
            chosenTradePlayer.Select();
        }

        var player = chosenTradePlayer.Player;
        
        for (int i = 0; i < targetCardList.childCount; i++)
        {
            Destroy(targetCardList.GetChild(i).gameObject);//TODO: Isso dá cancer, assim como outras partes desse código
        }
        
        player.EnableTargetTradeMenu();
        
        
    }
}