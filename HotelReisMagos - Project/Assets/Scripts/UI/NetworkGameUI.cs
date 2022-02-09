﻿using System;
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

    [SerializeField] private RectTransform playerCardList, playerAuctionList, targetCardList, playerList;
    [SerializeField] private ResourceCardUI cardPrefab;
    [SerializeField] private List<ResourceCardUI> selectedCards;
    [SerializeField] private List<ResourceCardUI> selectedTargetCards;

    [SerializeField] private List<ResourceCardUI> selectAuctionCards;
    [SerializeField] private List<ResourceCardUI> selectAuctionToGiveCards;

    [SerializeField] private RectTransform originTradeCardList, myTradeCardList;
    [SerializeField] private TradeCardInfo cardInfoPrefab;
    [SerializeField] private TextMeshProUGUI characterPlayerName;
    [SerializeField] private Transform tradeProposalWindow, tradeWaitWindow;

    [SerializeField] private List<PlayerSetup> players = new List<PlayerSetup>();
    [SerializeField] private GameObject tradeMenu;
    [SerializeField] private PlayerCharacterCard playerCharCardPrefab;
    [SerializeField] private PlayerCharacterCard chosenTradePlayer, chosenAuctionPlayer;

    public List<ResourceCardUI> SelectAuctionCards => selectAuctionCards;

    public List<ResourceCardUI> SelectAuctionToGiveCards => selectAuctionToGiveCards;
    
    public List<ResourceCardUI> SelectedCards => selectedCards;

    public List<ResourceCardUI> SelectedTargetCards => selectedTargetCards;
    
    public ResourceCardUI CardPrefab => cardPrefab;

    public TradeCardInfo CardInfoPrefab => cardInfoPrefab;
    
    public RectTransform PlayerCardList => playerCardList;

    public RectTransform PlayerAuctionList => playerAuctionList;

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

        //StartCoroutine(ResetLog());
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

    [SerializeField] private GameObject auctionMenu;
    
    public void EnableAuctionMenu()
    {
        if (PlayerSetup.playerControllers.Count <= 1)
        {
            LocalLog("Só há um jogador na partida. Não é possível fazer um leilão.");
            return;
        }
        
        actionMenu.SetActive(false);
        auctionMenu.SetActive(true);
        
        selectedAuctionCards.RemoveAll((ints => ints != null));
        
        for (int i = 0; i < playerAuctionList.childCount; i++)
        {
            Destroy(playerAuctionList.GetChild(i).gameObject);
        }
        PlayerSetup.localPlayerSetup.EnableAuctionMenu();
        //StartCoroutine(PopulateCharacterCards());
    }

    public void ReturnFromTradeMenu()
    {
        actionMenu.SetActive(true);
        tradeMenu.SetActive(false);
        if(chosenTradePlayer)
            chosenTradePlayer.Deselect();
    }
    
    public void ReturnFromAuctionMenu()
    {
        actionMenu.SetActive(true);
        auctionMenu.SetActive(false);
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
    
    public void ProposeAuction()
    {
        if(selectAuctionCards.Count > 0)
            PlayerSetup.localPlayerSetup.ProposeAuction(selectAuctionCards);
        else
        {
            Debug.LogWarning("You didn't add any cards");
            LocalLog("Você não está pedindo ou dando nenhuma carta");
        }
    }

    [SerializeField] private PlayerSetup currentTradeOrigin, currentAuctionOrigin;

    
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

    public void RefuseAuctionParticipation()
    {
        PlayerSetup.localPlayerSetup.RefuseAuctionParticipation(currentAuctionOrigin);
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
            //if (PlayerSetup.playerControllers[i] != PlayerSetup.localPlayerSetup)
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
    [SerializeField] private TextMeshProUGUI characterName, characterHistory;

    public void ShowPlayerCards(int playerNumber, int charInfoIndex)
    {
        var player = PlayerSetup.playerControllers.Where((setup => setup.PlayerNumber == playerNumber)).First();
        playerDeckWindow.SetActive(true);
        var charInfo = NetworkGameController.instance.CharacterList[charInfoIndex];
        
        characterName.SetText(charInfo.Name);
        characterHistory.SetText(charInfo.Description);

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

    [SerializeField] private Transform auctionProposalWindow;
    [SerializeField] private Transform auctionProposerCardList, auctionMyCardList, auctionChoiceProposerList;
    [SerializeField] private TextMeshProUGUI auctionProposalReceiveTitle;

    public void PresentAuction(List<List<int>> cardList, int playerNumber)
    {
        currentAuctionOrigin = PlayerSetup.playerControllers.Where((setup => setup.PlayerNumber == playerNumber)).First();
        auctionProposalWindow.gameObject.SetActive(true);
        
        for (int i = 0; i < auctionProposerCardList.childCount; i++)
        {
            var tempObject = auctionProposerCardList.GetChild(i).gameObject;
            Destroy(tempObject);
        }
        
        for (int i = 0; i < auctionMyCardList.childCount; i++)
        {
            var tempObject = auctionMyCardList.GetChild(i).gameObject;
            Destroy(tempObject);
        }

        //Cartas do proponente do leilão
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < cardList[j].Count; i++)
            {
                
                CardInfo card = null;
                if(j == 0) //TODO: Isso aqui devia ser um switch, mas um if é mais rápido de fazer.
                    card = NetworkGameController.instance.cardList1[cardList[j][i]];
                else if(j == 1)
                    card = NetworkGameController.instance.cardList2[cardList[j][i]];
                else if(j == 2)
                    card = NetworkGameController.instance.cardList3[cardList[j][i]];

                if (false)
                {
                    
                }
                var newCard = Instantiate(cardInfoPrefab, auctionProposerCardList);
                newCard.DescriptionText.SetText(card.Description);
                newCard.IdText.SetText(card.ID);
            }
        }
        
        //Cartas do jogador que viu a tela do leilão

        for (int i = 0; i < PlayerSetup.localPlayerSetup.cardsOnHand1.Count; i++)
        {
            var newCard = Instantiate(cardPrefab, auctionMyCardList);
            newCard.Populate(PlayerSetup.localPlayerSetup.cardsOnHand1[i], 1, true);
            newCard.IsAuction = true;
        }
        
        for (int i = 0; i < PlayerSetup.localPlayerSetup.cardsOnHand2.Count; i++)
        {
            var newCard = Instantiate(cardPrefab, auctionMyCardList);
            newCard.Populate(PlayerSetup.localPlayerSetup.cardsOnHand2[i], 2, true);
            newCard.IsAuction = true;
        }
        
        for (int i = 0; i < PlayerSetup.localPlayerSetup.cardsOnHand3.Count; i++)
        {
            var newCard = Instantiate(cardPrefab, auctionMyCardList);
            newCard.Populate(PlayerSetup.localPlayerSetup.cardsOnHand3[i], 3, true);
            newCard.IsAuction = true;
        }
        /*for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < cardList[j].Count; i++)
            {
                var newCard = Instantiate(cardPrefab, auctionMyCardList);
                CardInfo card = null;
                if(j == 0) //TODO: Isso aqui devia ser um switch, mas um if é mais rápido de fazer.
                    card = NetworkGameController.instance.cardList1[PlayerSetup.localPlayerSetup.cardsOnHand1[i]];
                else if(j == 1)
                    card = NetworkGameController.instance.cardList2[PlayerSetup.localPlayerSetup.cardsOnHand2[i]];
                else if(j == 2)
                    card = NetworkGameController.instance.cardList3[PlayerSetup.localPlayerSetup.cardsOnHand3[i]];
                
                newCard.Populate(i, j + 1, true);
                newCard.IsAuction = true;
            }
        }*/
        
        var charName = NetworkGameController.instance.CharacterList[currentAuctionOrigin.CharacterInfoIndex];
        
        auctionProposalReceiveTitle.SetText($"O empresário {charName.Name} propõe um leilão!");
    }

    public void AuctionBid()
    {
        if (selectAuctionToGiveCards.Count <= 0)
        {
            LocalLog("Você não selecionou nenhuma carta para dar o lance.");
        }
        else
        {
            PlayerSetup.localPlayerSetup.AuctionBid(selectAuctionToGiveCards, currentAuctionOrigin);
            ReturnFromAuctionAnswer();
        }
    }

    [ClientRpc]
    public void RpcResetAuction()
    {
        selectAuctionCards = new List<ResourceCardUI>();
        selectAuctionToGiveCards = new List<ResourceCardUI>();

        for (int i = 0; i < auctionProposerCardList.childCount; i++)
        {
            var tempObject = auctionProposerCardList.GetChild(i).gameObject;
            Destroy(tempObject);
        }
        
        for (int i = 0; i < auctionMyCardList.childCount; i++)
        {
            var tempObject = auctionMyCardList.GetChild(i).gameObject;
            Destroy(tempObject);
        }
    }

    public void ReturnFromAuctionAnswer()
    {
        auctionProposalWindow.gameObject.SetActive(false);
    }
    
    public void ReturnFromAuctionChoice()
    {
        auctionChoicelWindow.gameObject.SetActive(false);
        actionMenu.SetActive(true);
    }

    [SerializeField] private GameObject auctionWait;
    
    public void ReturnFromAuctionWait()
    {
        auctionWait.SetActive(false);
        actionMenu.SetActive(true);
    }

    public void GoToAuctionWait()
    {
        auctionMenu.SetActive(false);
        auctionWait.SetActive(true);
    }

    [SerializeField] private Transform auctionChoicelWindow;
    [SerializeField] private Transform auctionChoiceOfferList, auctionChoiceMyList, auctionChoicePlayerList;

    [SerializeField]private List<List<List<int>>> playerOFfers;

    public Transform AuctionChoicePlayerList => auctionChoicePlayerList;

    public Transform AuctionChoiceMyList => auctionChoiceMyList;

    public Transform AuctionChoiceOfferList => auctionChoiceOfferList;
    
    public void PresentAuctionChoice(List<List<List<int>>> playerAuctionOffers)
    {
        for (int i = 0; i < auctionChoiceOfferList.childCount; i++)
        {
            var temp = auctionChoiceOfferList.GetChild(i).gameObject;
            Destroy(temp);
        }
        
        for (int i = 0; i < auctionChoiceMyList.childCount; i++)
        {
            var temp = auctionChoiceMyList.GetChild(i).gameObject;
            Destroy(temp);
        }

        for (int i = 0; i < auctionChoicePlayerList.childCount; i++)
        {
            var temp = auctionChoicePlayerList.GetChild(i).gameObject;
            Destroy(temp);
        }

        var cards = PlayerSetup.localPlayerSetup.SelectedCards1.ToList();
        
        for (int j = 0; j < cards.Count; j++)
        {
            for (int i = 0; i < cards[j].Count; i++)
            {
                var newCard = Instantiate(cardInfoPrefab, auctionChoiceMyList);
                newCard.Populate(cards[j][i], j + 1, true, false);
            }
        }
        
        auctionWait.SetActive(false);
        playerOFfers = playerAuctionOffers;
        StartCoroutine(PopulateChoiceCharacters());
    }
    
    private IEnumerator PopulateChoiceCharacters()
    {
        Debug.Log("Clearing cards1 auction");

        for (int i = 0; i < auctionChoicePlayerList.childCount; i++)
        {
            Destroy(auctionChoicePlayerList.GetChild(i).gameObject);
        }

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
                    var tempList = PlayerSetup.localPlayerSetup.playerAuctionOffers
                        .Where((list => list[3][0] == PlayerSetup.playerControllers[i].PlayerNumber)).ToList();
                    if (tempList.Count != 0)
                    {
                        var temp = tempList.First()[3][0];   
                        if (PlayerSetup.playerControllers[i].PlayerNumber == temp)
                        {
                            var playerChar = Instantiate(playerCharCardPrefab, auctionChoicePlayerList);
                            yield return new WaitUntil(() =>
                            {
                                return PlayerSetup.playerControllers[i].CharacterInfoIndex != -1;
                            });
                            Debug.Log("Clearing cards222222222222222222222222");

                            playerChar.Populate(i, PlayerSetup.playerControllers[i]);
                            playerChar.IsAuction = true;
                        }
                    }
                }
            }
        }
        
        auctionChoicelWindow.gameObject.SetActive(true);
    }

    private List<List<int>> selectedAuctionCards = new List<List<int>>();
    public void SelectAuctionPlayer(PlayerCharacterCard playerCharacterCard)
    {
        if (chosenAuctionPlayer)
        {
            chosenAuctionPlayer.Deselect();
            chosenAuctionPlayer = playerCharacterCard;
            chosenAuctionPlayer.Select();
        }
        else
        {
            chosenAuctionPlayer = playerCharacterCard;
            chosenAuctionPlayer.Select();
        }

        var player = chosenAuctionPlayer.Player;
        
        /*for (int i = 0; i < auctionChoicePlayerList.childCount; i++)
        {
            Destroy(auctionChoicePlayerList.GetChild(i).gameObject);//TODO: Isso dá cancer, assim como outras partes desse código
        }*/
        
        PlayerSetup.localPlayerSetup.EnableTargetAuctionChoiceMenu(player);
        
        
    }

    public void AcceptAuctionSelectedOffer()
    {
        if (chosenAuctionPlayer)
        {
            PlayerSetup.localPlayerSetup.ConcludeAuction(chosenAuctionPlayer, selectedAuctionCards);
            ReturnFromAuctionChoice();
        }
        else
        {
            LocalLog("Você não selecionou um empresário para finalizar o leilão.");
        }
    }

    public void RefuseEveryBid()
    {
        for (int i = 0; i < auctionChoiceMyList.childCount; i++)
        {
            var tempObject = auctionChoiceMyList.GetChild(i).gameObject;
            Destroy(tempObject);
        }
        
        for (int i = 0; i < auctionChoiceOfferList.childCount; i++)
        {
            var tempObject = auctionChoiceOfferList.GetChild(i).gameObject;
            Destroy(tempObject);
        }

        PlayerSetup.localPlayerSetup.RefuseEveryBid();
        ReturnFromAuctionChoice();
    }

    // Isso não devia estar aqui, mas só quero terminar isso logo -Lucena
    
    [SerializeField] private AudioSource narrator;

    [ClientRpc]
    public void RpcPlayAudioEverywhere(int cardIndex, int act)
    {
        List<CardInfo> deck = null;
        CardInfo card;
        
        switch(act)
        {
            case 1:
                deck = NetworkGameController.instance.cardList1;
                break;
            case 2:
                deck = NetworkGameController.instance.cardList2;
                break;
            case 3:
                deck = NetworkGameController.instance.cardList3;
                break;
            default:
                deck = NetworkGameController.instance.cardList1;
                break;
        }

        card = deck[cardIndex];
        if(card != null)
            if (card.VoiceClip)
            {
                narrator.clip = card.VoiceClip;
                narrator.Play();
                //narrator.PlayOneShot(card.VoiceClip);
            }
            else
            {
                Debug.Log("No voiceclip.");
            }
    }

    [SerializeField] List<AudioClip> actAudios = new List<AudioClip>();

    [ClientRpc]
    public void RpcPlayActAudio(int number)
    {
        narrator.clip = actAudios[number-1];
        narrator.Play();
    }

    [SerializeField] private GameObject actionsHolder;
    
    [ClientRpc]
    public void RpcDisableActions()
    {
        actionsHolder.SetActive(false);    
    }
}