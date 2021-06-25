using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
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
    [SerializeField] private List<ResourceCardUI> selectedTargetCards;
    

    [SerializeField] private RectTransform originTradeCardList, myTradeCardList;
    [SerializeField] private TradeCardInfo cardInfoPrefab;
    [SerializeField] private TextMeshProUGUI characterPlayerName;
    [SerializeField] private Transform tradeProposalWindow, tradeWaitWindow;

    [SerializeField] private List<PlayerSetup> players = new List<PlayerSetup>();
    [SerializeField] private GameObject tradeMenu;
    [SerializeField] private PlayerCharacterCard playerCharCardPrefab;
    [SerializeField] private PlayerCharacterCard chosenTradePlayer;

    public List<ResourceCardUI> SelectedCards => selectedCards;

    public List<ResourceCardUI> SelectedTargetCards => selectedTargetCards;
    
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

    [SerializeField] private GameObject altChildList;
    [SerializeField] private GameObject emptyObject;

    private IEnumerator PopulateCharacterCards()
    {
        Debug.Log("Clearing cards1");
        
        Destroy(playerList.gameObject);

        playerList = Instantiate(emptyObject, altChildList.transform).transform as RectTransform;
        
        /*for (int j = 0; j < playerList.childCount; j++)
        {
            Destroy(targetCardList.GetChild(j).gameObject);//TODO: Isso dá cancer, assim como outras partes desse código
            Debug.Log("Clearing cards333333333333????????");

        }*/
        
        for (int i = 0; i < PlayerSetup.playerControllers.Count; i++)
        {

            if (PlayerSetup.playerControllers[i])
            {
                if (PlayerSetup.playerControllers[i] != PlayerSetup.localPlayerSetup)
                {
                    var playerChar = Instantiate(playerCharCardPrefab, playerList);
                    yield return new WaitUntil(() =>
                    {
                        return PlayerSetup.playerControllers[i].CharacterInfoIndex != -1;
                    });
                    Debug.Log("Clearing cards222222222222222222222222");

                    playerChar.Populate(i, PlayerSetup.playerControllers[i]);
                }
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
        
        selectedCards.RemoveAll((ui => ui));
        selectedTargetCards.RemoveAll((ui => ui));
        
        PlayerSetup.localPlayerSetup.EnableTradeMenu();
        for (int i = 0; i < targetCardList.childCount; i++)
        {
            Destroy(targetCardList.GetChild(i).gameObject);
        }
        StartCoroutine(PopulateCharacterCards());
    }

    public void ReturnFromTradeMenu()
    {
        actionMenu.SetActive(true);
        tradeMenu.SetActive(false);
        if(chosenTradePlayer)
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

    public void ProposeChange()
    {
        if(selectedTargetCards.Count > 0 || selectedCards.Count > 0)
            PlayerSetup.localPlayerSetup.ProposeTrade(selectedCards, selectedTargetCards, chosenTradePlayer);
        else
        {
            Debug.LogWarning("You didn't add any cards");
            LocalLog("Você não está pedindo ou dando nenhuma carta");
        }
    }

    [SerializeField] private PlayerSetup currentTradeOrigin;

    
    //TODO: Uau, quem diria! Mais uma função com alto teor de código cancerígeno! Quem será que escreveu esse Chernobyl?
    public void PresentTrade(List<List<int>> originCard, List<List<int>> selectedMyCards, int playerNumber)
    {
        currentTradeOrigin = PlayerSetup.playerControllers.Where((setup => setup.PlayerNumber == playerNumber)).First();
        tradeProposalWindow.gameObject.SetActive(true);
        
        for (int i = 0; i < originTradeCardList.childCount; i++)
        {
            var tempObject = originTradeCardList.GetChild(i).gameObject;
            Destroy(tempObject);
        }
        
        for (int i = 0; i < myTradeCardList.childCount; i++)
        {
            var tempObject = myTradeCardList.GetChild(i).gameObject;
            Destroy(tempObject);
        }
        
        #region ORIGINCARDS
        {
            for (int i = 0; i < originCard[0].Count; i++)
            {
                var newCard = Instantiate(cardInfoPrefab, originTradeCardList);
                var card = NetworkGameController.instance.cardList1[originCard[0][i]];
                newCard.DescriptionText.SetText(card.Description);
                newCard.IdText.SetText(card.ID);
            }

            for (int i = 0; i < originCard[1].Count; i++)
            {
                var newCard = Instantiate(cardInfoPrefab, originTradeCardList);
                var card = NetworkGameController.instance.cardList1[originCard[1][i]];
                newCard.DescriptionText.SetText(card.Description);
                newCard.IdText.SetText(card.ID);
            }

            for (int i = 0; i < originCard[2].Count; i++)
            {
                var newCard = Instantiate(cardInfoPrefab, originTradeCardList);
                var card = NetworkGameController.instance.cardList1[originCard[2][i]];
                newCard.DescriptionText.SetText(card.Description);
                newCard.IdText.SetText(card.ID);
            }
        }
        #endregion

        #region MYCARDS
        {
            for (int i = 0; i < selectedMyCards[0].Count; i++)
            {
                var newCard = Instantiate(cardInfoPrefab, myTradeCardList);
                var card = NetworkGameController.instance.cardList1[selectedMyCards[0][i]];
                newCard.DescriptionText.SetText(card.Description);
                newCard.IdText.SetText(card.ID);
            }

            for (int i = 0; i < selectedMyCards[1].Count; i++)
            {
                var newCard = Instantiate(cardInfoPrefab, myTradeCardList);
                var card = NetworkGameController.instance.cardList1[selectedMyCards[1][i]];
                newCard.DescriptionText.SetText(card.Description);
                newCard.IdText.SetText(card.ID);
            }

            for (int i = 0; i < selectedMyCards[2].Count; i++)
            {
                var newCard = Instantiate(cardInfoPrefab, myTradeCardList);
                var card = NetworkGameController.instance.cardList1[selectedMyCards[2][i]];
                newCard.DescriptionText.SetText(card.Description);
                newCard.IdText.SetText(card.ID);
            }
            
            
        }
        #endregion

        var charName = NetworkGameController.instance.CharacterList[currentTradeOrigin.CharacterInfoIndex];
        
        characterPlayerName.SetText($"O empresário {charName.Name} te propõe uma troca!");
    }

    public void AcceptTrade()
    {
        for (int i = 0; i < originTradeCardList.childCount; i++)
        {
            var tempObject = originTradeCardList.GetChild(i).gameObject;
            Destroy(tempObject);
        }
        
        for (int i = 0; i < myTradeCardList.childCount; i++)
        {
            var tempObject = myTradeCardList.GetChild(i).gameObject;
            Destroy(tempObject);
        }
        
        PlayerSetup.localPlayerSetup.AcceptTrade(currentTradeOrigin);
    }

    public void RefuseTrade()
    {
        for (int i = 0; i < originTradeCardList.childCount; i++)
        {
            var tempObject = originTradeCardList.GetChild(i).gameObject;
            Destroy(tempObject);
        }
        
        for (int i = 0; i < myTradeCardList.childCount; i++)
        {
            var tempObject = myTradeCardList.GetChild(i).gameObject;
            Destroy(tempObject);
        }
        
        PlayerSetup.localPlayerSetup.RefuseTrade(currentTradeOrigin);
    }

    public void WaitTradeResult()
    {
        tradeMenu.SetActive(false);
        tradeWaitWindow.gameObject.SetActive(true);
    }

    public void ReturnFromWaitTrade()
    {
        tradeWaitWindow.gameObject.SetActive(false);
        actionMenu.SetActive(true);
    }
    
    public void ReturnFromTradeProposal()
    {
        tradeProposalWindow.gameObject.SetActive(false);
    }

    [SerializeField] private Transform winnersList, winnerWindow;
    [SerializeField] private WinnerPlayerCard winnerCardPrefab;

    [ClientRpc]
    public void RpcSetWinnerElection(List<int> playerNumbers)
    {
        List<PlayerSetup> players = new List<PlayerSetup>();

        winnerWindow.gameObject.SetActive(true);
        
        for (int i = 0; i < playerNumbers.Count; i++)
        {
            if (PlayerSetup.playerControllers[i] != PlayerSetup.localPlayerSetup)
            {
                players.Add(PlayerSetup.playerControllers.Find((setup => setup.PlayerNumber == playerNumbers[i])));
                var winnerCard = Instantiate(winnerCardPrefab, winnersList);
                winnerCard.Populate(players.Last());
            }
        }
    }

    public void CloseVoteMenu()
    {
        winnerWindow.gameObject.SetActive(false);
    }

    [ClientRpc]
    public void RpcResetSlots()
    {
        NetworkGameController.OnResetSlots.Invoke();
    }

    [SerializeField] private GameObject gameScene, menuScene;
    
    [ClientRpc]
    public void RpcStartGame()
    {
        gameScene.SetActive(true);
        menuScene.SetActive(false);
        NetworkGameController.InvokeOnLoadFakeScene();
    }

    [SerializeField] private Transform playerDeckList;
    [SerializeField] private GameObject playerDeckWindow;

    public void ShowPlayerCards(int playerNumber)
    {
        var player = PlayerSetup.playerControllers.Where((setup => setup.PlayerNumber == playerNumber)).First();
        playerDeckWindow.SetActive(true);
        
        for (int i = 0; i < playerDeckList.childCount; i++)
        {
            var tempObject = playerDeckList.GetChild(i).gameObject;
            Destroy(tempObject);
        }

        #region ORIGINCARDS
        {
            for (int i = 0; i < player.cardsOnHand1.Count; i++)
            {
                var newCard = Instantiate(cardInfoPrefab, playerDeckList);
                var card = NetworkGameController.instance.cardList1[player.cardsOnHand1[i]];
                newCard.DescriptionText.SetText(card.Description);
                newCard.IdText.SetText(card.ID);
            }

            for (int i = 0; i < player.cardsOnHand2.Count; i++)
            {
                var newCard = Instantiate(cardInfoPrefab, playerDeckList);
                var card = NetworkGameController.instance.cardList1[player.cardsOnHand2[i]];
                newCard.DescriptionText.SetText(card.Description);
                newCard.IdText.SetText(card.ID);
            }

            for (int i = 0; i < player.cardsOnHand3.Count; i++)
            {
                var newCard = Instantiate(cardInfoPrefab, playerDeckList);
                var card = NetworkGameController.instance.cardList1[player.cardsOnHand3[i]];
                newCard.DescriptionText.SetText(card.Description);
                newCard.IdText.SetText(card.ID);
            }
            
            for (int i = 0; i < player.LuckCardAmount; i++)
            {
                var newCard = Instantiate(cardInfoPrefab, playerDeckList);
                newCard.DescriptionText.SetText("Sorte ou revés");
                newCard.IdText.SetText("???");
            }
        }
        #endregion

        //var charName = NetworkGameController.instance.CharacterList[currentTradeOrigin.CharacterInfoIndex];
        
        //characterPlayerName.SetText($"O empresário {charName.Name} te propõe uma troca!");
    }
}