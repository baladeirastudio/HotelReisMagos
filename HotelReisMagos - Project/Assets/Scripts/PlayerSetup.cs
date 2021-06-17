using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Mirror.Examples.Chat;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerSetup : NetworkBehaviour
{
    public static List<PlayerSetup> playerControllers = new List<PlayerSetup>();

    [SyncVar(hook = nameof(HandleNameChange))] [SerializeField] private string playerName = "Player";
    [SerializeField] private PlayerListEntry myEntry;
    
    public Action<string, string, PlayerSetup> onChangeName;
    [SerializeField] private Texture2D profilePicture = null;
    [SerializeField] private Sprite steamAvatar = null;

    [SyncVar, SerializeField] private int politicalResources = 0;
    [SyncVar, SerializeField] private int economicResources = 0;
    [SyncVar, SerializeField] private int socialResources = 0;
    [SyncVar, SerializeField] private int mediaResources = 0;
    [SyncVar, SerializeField] private int luckCardAmount = 0;
    [Tooltip("Minimum and max value to be used in the luck card, inclusive and exclusive.")]
    [SerializeField] private int minLuck = 3, maxLuck = 5;

    public SyncList<int> CardsOnHand1 => cardsOnHand1;

    public SyncList<int> CardsOnHand2 => cardsOnHand2;

    public SyncList<int> CardsOnHand3 => cardsOnHand3;

    public int LuckCardAmount
    {
        get => luckCardAmount;
        set => luckCardAmount = value;
    }
    
    public int PoliticalResources
    {
        get => politicalResources;
        set => politicalResources = value;
    }

    public int EconomicResources
    {
        get => economicResources;
        set => economicResources = value;
    }

    public int SocialResources
    {
        get => socialResources;
        set => socialResources = value;
    }

    public int MediaResources
    {
        get => mediaResources;
        set => mediaResources = value;
    }
    
    public Sprite SteamAvatar => steamAvatar;
    
    public Texture2D ProfilePicture
    {
        get => profilePicture;
        set => profilePicture = value;
    }

    public string PlayerName
    {
        get => playerName;
        set => playerName = value;
    }
    
    public PlayerListEntry PlayerEntry
    {
        get => myEntry;
        set => myEntry = value;
    }

    public void HandleNameChange(string old, string newName)
    {
        Debug.Log($"My number is {playerNumber}", gameObject);
        if(myEntry)
            myEntry.ForceChangeDisplayName(newName);
        RpcChangeNameEvent(old, newName, this);
    }

    public void HandleEntryChange(int old, int newVal)
    {
        
    }
    
    /*[ClientRpc]
    public void RpcTrackEntry(PlayerListEntry entry)
    {
        myEntry = entry;
        myEntry.Track(this);
        Debug.Log("Spawning entry2");
    }*/

    [Server]
    public void SetDisplayName(string newVal)
    {
        playerName = newVal;
    }
    
    [Command]
    public void CmdSetDisplayName(string newVal)
    {
        SetDisplayName(newVal);
    }
    
    /*[ClientRpc]
    public void RpcSetEntryIndex(int newEntry)
    {
        playerEntry = newEntry;

        if (isServer && !isServerOnly)
        {
            if (hasAuthority)
            {
                
            }
        }
    }*/
    
    [ClientRpc]
    public void RpcChangeNameEvent(string old, string newVal, PlayerSetup player)
    {
        onChangeName?.Invoke(old, newVal, player);
    }
    
    
    public void FetchSteamData()
    {
        //playerName = SteamFriends.GetPersonaName();
        playerName = PlayerInfo.clientName;
        profilePicture = PlayerInfo.clientPfp;

        (NetworkManager.singleton as NetworkManagerCardGame).PrintSomething();
        
        
        /*int iAvatar = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());
        SteamUtils.GetImageSize(iAvatar, out uint imgWidth, out uint imgHeight);
        byte[] data = new byte[imgHeight * imgWidth * 4];
        var isValid = SteamUtils.GetImageRGBA(iAvatar, data, (int)( 4 * imgHeight * imgWidth));
        if (isValid)
        {
            profilePicture = new Texture2D((int)imgWidth, (int)imgHeight, TextureFormat.RGBA32, false, true);
            profilePicture.LoadRawTextureData(data);
            profilePicture.Apply();
            
            //steamAvatar = Sprite.Create(profilePicture, new Rect(0, 0, imgWidth, imgHeight), Vector2.one/2 );
            
            //Destroy(profilePicture);
        }
        else
        {
            Debug.LogError("Could not fetch Steam PFP.");
        }*/
        
    }
    
    
    
    
    
    
    
    
    
    [SyncVar, SerializeField] private int playerNumber;

    [SyncVar, SerializeField] private bool isMyTurn = false;

    [SerializeField] public SyncList<int> cardsOnHand1 = new SyncList<int>();
    [SerializeField] public SyncList<int> cardsOnHand2 = new SyncList<int>();
    [SerializeField] public SyncList<int> cardsOnHand3 = new SyncList<int>();
    [SyncVar, SerializeField] private int characterInfoIndex = -1;

    public enum TradeStatus
    {
        Pending, Denied, Accepted, NotUsed
    }
    
    [SyncVar, SerializeField] private TradeStatus tradeStatus = TradeStatus.NotUsed;

    [SyncVar, SerializeField] private Color color;

    public static PlayerSetup localPlayerSetup;
    private string PERMAslotId;
    [SyncVar, SerializeField] private bool choseSlot = false;
    [SyncVar, SerializeField] private bool usedLuckCard = false;

    public int CharacterInfoIndex
    {
        get => characterInfoIndex;
        set => characterInfoIndex = value;
    }

    public bool UsedLuckCard
    {
        get => usedLuckCard;
        set => usedLuckCard = value;
    }
    
    public bool ChoseSlot
    {
        get => choseSlot;
        set => choseSlot = value;
    }

    public int PlayerNumber { get => playerNumber; set => playerNumber = value; }

    public Color MyColor { get => color; set => color = value; }

    private void Awake()
    {
        
    }

    private void Start()
    {
        PlayerSetup.playerControllers.Add(this);
        Debug.Log($"Added {this}!!!!!!!!!!!! ");
        //myPlaers = playerControllers;
        //Debug.LogError("Adding self.");

        if (NetworkGameController.instance)
        {
            //GameController.instance.RegisterPlayer(this);
        }
        else
        {
            //Debug.LogError("Missing Game Controller Instance!");
        }
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        localPlayerSetup = this;
        
        StartCoroutine(GetRandomCharacterTwo());
        SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;

        //Debug.Log(characterInfoIndex);
    }

    private IEnumerator GetRandomCharacterTwo()
    {
        yield return new WaitForEndOfFrame();
        if(!choseCharacter)
            if(NetworkGameController.instance)
            CmdGetRandomCharacter();
    }

    private void SceneManagerOnactiveSceneChanged(Scene arg0, Scene arg1)
    {
        StartCoroutine(GetRandomCharacter());
    }

    private IEnumerator GetRandomCharacter()
    {
        yield return new WaitUntil(() => { return NetworkGameController.instance;});
        CmdGetRandomCharacter();
    }

    [SyncVar, SerializeField] private bool choseCharacter = false;

    [Command]
    private void CmdGetRandomCharacter()
    {
        if (!choseCharacter)
        {
            characterInfoIndex = NetworkGameController.instance.GetRandomCharacter();
            choseCharacter = true;
            RpcLocalLog(
                $"Nesta partida, você será o empresário {NetworkGameController.instance.CharacterList[characterInfoIndex].Name}." +
                $"\n'{NetworkGameController.instance.CharacterList[characterInfoIndex].Description}'");
        }
    }

        [ClientRpc]
    public void RpcStartYourTurn()
    {
        if (hasAuthority)
        {
            isMyTurn = true;
        }
    }
    
    [ClientRpc]
    public void RpcEndYourTurn()
    {
        if (hasAuthority)
        {
            isMyTurn = false;
        }
    }

    public void CmdSelectSlot(SlotController slot)
    {
        PERMAslotId = slot.ID;
        
        CmdCheckSelectedSlot(PERMAslotId);
    }

    [Command]
    private void CmdCheckSelectedSlot(string slotID)
    {
        var temp = NetworkManager.singleton as NetworkManagerCardGame;
        
        if(temp.IsSlotSelected(slotID))
        {
            Debug.LogError("Slot ALREADY SELECTED!");
            return;
        }

        //Debug.LogError("Testing.");
        try
        {
            if (characterInfoIndex == -1)
            {
                Debug.Log($"InfoIndex: {characterInfoIndex}");
                if (characterInfoIndex < 0)
                    characterInfoIndex = NetworkGameController.instance.GetRandomCharacter();
                Debug.Log("Getting random character");
            }
            
            //Debug.LogError($"Key: {slotID} - Value: {SlotController.slots[slotID]}");
            var tempSlot = SlotController.slots[slotID];
            var tempPlayer = PlayerSetup.playerControllers[NetworkGameController.instance.PlayerTurnID];
            var tempColor = tempPlayer.MyColor;
            tempSlot.SetSelectedSlot(tempColor, playerNumber);

            int reward = Random.Range(tempSlot.MinReward, tempSlot.MaxReward);

            switch (tempSlot.RewardType)
            {
                case SlotReward.PoliticalResource: //Political
                    politicalResources += reward;
                    NetworkGameUI.Instance.RpcLog($"Jogador {NetworkGameController.instance.CharacterList[characterInfoIndex].Name} ganhou {reward} pontos de recurso político.");
                    break;
                case SlotReward.EconomicalResource: //Economical
                    economicResources += reward;
                    NetworkGameUI.Instance.RpcLog($"Jogador {NetworkGameController.instance.CharacterList[characterInfoIndex].Name} ganhou {reward} pontos de recurso econômico.");
                    break;
                case SlotReward.SocialResource: //Social
                    socialResources += reward;
                    NetworkGameUI.Instance.RpcLog($"Jogador {NetworkGameController.instance.CharacterList[characterInfoIndex].Name} ganhou {reward} pontos de recurso social.");
                    break;
                case SlotReward.MediaResource: //Media
                    mediaResources += reward;
                    NetworkGameUI.Instance.RpcLog($"Jogador {NetworkGameController.instance.CharacterList[characterInfoIndex].Name} ganhou {reward} pontos de recurso midiático.");
                    break;
                case SlotReward.LuckCard:
                    NetworkGameUI.Instance.RpcLog($"Jogador {NetworkGameController.instance.CharacterList[characterInfoIndex].Name} ganhou uma carta de sorte ou revés.");
                    luckCardAmount++;
                    break;
            }

            if (tempSlot.GiveLuckCard)
            {
                luckCardAmount++;
                NetworkGameUI.Instance.RpcLog($"Jogador {NetworkGameController.instance.CharacterList[characterInfoIndex].Name} ganhou uma carta de sorte ou revés.");
            }

            CardInfo card = null;
            int cardIndex = 0;
            List<CardInfo> deck = null;
            SyncList<int> deckNum = null;

            switch (tempSlot.ActToUnlock)
            {
                case 1:
                    deckNum = NetworkGameController.instance.activeCards1;
                    deck = NetworkGameController.instance.cardList1;
                    cardIndex = Random.Range(0, deckNum.Count);
                    
                    //Debug.LogError($"Deck: {deck.Count}");
                    //Debug.LogError($"deckNum: {deckNum.Count}");
                    //Debug.LogError($"index: {cardIndex}");
                    
                    card = deck[cardIndex];
                    deckNum.Remove(cardIndex);
                    cardsOnHand1.Add(cardIndex);
                    
                    break;
                case 2:
                    deckNum = NetworkGameController.instance.activeCards2;
                    deck = NetworkGameController.instance.cardList2;
                    cardIndex = Random.Range(0, deckNum.Count);
                    card = deck[cardIndex];
                    deckNum.Remove(cardIndex);
                    cardsOnHand2.Add(cardIndex);
                    break;
                case 3:
                    deckNum = NetworkGameController.instance.activeCards3;
                    deck = NetworkGameController.instance.cardList3;
                    cardIndex = Random.Range(0, deckNum.Count);
                    card = deck[cardIndex];
                    deckNum.Remove(cardIndex);
                    cardsOnHand3.Add(cardIndex);
                    break;
            }

            NetworkGameUI.Instance.RpcLog($"O jogador {NetworkGameController.instance.CharacterList[characterInfoIndex].Name} obteve a seguinte carta: {card.Description}");
            NetworkGameUI.Instance.RpcRefreshPlayerCards();
        }
        catch (Exception e)
        {
            Debug.LogError("This shit won't fucking work");
            Debug.LogException(e);
        }

        //Debug.LogError("Enabling finish round");
        NetworkGameUI.Instance.RpcEnableFinishRound();
        choseSlot = true;
    }

    public void AdvanceTurn()
    {
        CmdNext();
    }

    [Command]
    private void CmdNext()
    {
        NetworkGameUI.Instance.RpcLog($"Jogador {NetworkGameController.instance.CharacterList[characterInfoIndex].Name} terminou o turno.");
        NetworkGameController.instance.RpcNextTurn();
    }

    [Command]
    private void CmdUseLuckCard()
    {
        bool isLuck = Random.Range(0, 2) > 0;

        SlotReward rewardType = (SlotReward)Random.Range(0, 4);
        int reward = Random.Range(minLuck, maxLuck);
        ref int resourceToChange = ref economicResources;

        string rewardName = String.Empty;
        
        switch (rewardType)
        {
            case SlotReward.EconomicalResource:
                resourceToChange = ref economicResources;
                rewardName = "econômico";
                break;
            case SlotReward.MediaResource:
                resourceToChange = ref mediaResources;
                rewardName = "midiático";
                break;
            case SlotReward.PoliticalResource:
                resourceToChange = ref politicalResources;
                rewardName = "político";
                break;
            case SlotReward.SocialResource:
                resourceToChange = socialResources;
                rewardName = "social";
                break;
        }

        if (isLuck)
        {
            resourceToChange += reward;
            NetworkGameUI.Instance.RpcLog($"Jogador {NetworkGameController.instance.CharacterList[characterInfoIndex].Name} tirou sorte e ganhou {reward} pontos de recurso {rewardName}!");
        }
        else
        {
            resourceToChange -= reward;
            NetworkGameUI.Instance.RpcLog($"Jogador {NetworkGameController.instance.CharacterList[characterInfoIndex].Name} tirou revés e perdeu {reward} pontos de recurso {rewardName}!");
        }

        luckCardAmount--;
        usedLuckCard = true;
        
        //Debug.LogError($"Added {reward * (isLuck ? 1 : -1)}");
    }

    public void UseLuckCard()
    {
        CmdUseLuckCard();
    }

    [Command]
    public void CmdStartMatch()
    {
        NetworkGameUI.Instance.RpcStartGame();
        //(NetworkManagerCardGame.singleton as NetworkManagerCardGame).ServerChangeScene("Game_Lucena"); //TODO: Don't use constants
    }

    public void UpdateSlots(string id, int number)
    {
        CmdUpdateSlots(id, number);
    }
    
    [Command]
    private void CmdUpdateSlots(string id, int playerNumber)
    {
        NetworkGameController.instance.RpcUpdateSlots(id, playerNumber);
    }

    public void SecondActBonus()
    {
        if (characterInfoIndex < 0)
        {
            Debug.LogError("Nope, not like this, won' work");
            Debug.Log($"InfoIndex: {characterInfoIndex}");
            Debug.LogWarning($"newwwwwwwwwwwwwwInfoIndex: {characterInfoIndex}");

            
            if (!NetworkGameController.instance)
            {
                Debug.LogError("There's no gameController");
            }
            else
            {
                Debug.LogError("There IS A gameController!!!???");
                if (characterInfoIndex < 0)
                    characterInfoIndex = NetworkGameController.instance.GetRandomCharacter();
            }
        }

        Debug.LogWarning($"NetworkGameInstance: {NetworkGameController.instance}");
        
        
        var charInfo = NetworkGameController.instance.CharacterList[characterInfoIndex].SecondActBonus;
        mediaResources += charInfo.mediaBonus;
        economicResources += charInfo.economicBonus;
        politicalResources += charInfo.politicBonus;
        socialResources += charInfo.socialBonus;
        try
        {
            RpcLocalLog($"Você recebeu o bônus de empresário! '{charInfo.onReceiveBonusText}'");
        }
        catch (Exception e)
        {
            Debug.LogError("CATCH EXAEAFASFAAAAAAAAAAA");
            Debug.LogException(e);
        }
    }
    
    public void ThirdActBonus()
    {
        var charInfo = NetworkGameController.instance.CharacterList[characterInfoIndex].ThirdActBonus;
        mediaResources += charInfo.mediaBonus;
        economicResources += charInfo.economicBonus;
        politicalResources += charInfo.politicBonus;
        socialResources += charInfo.socialBonus;
        RpcLocalLog($"Você recebeu o bônus de empresário! '{charInfo.onReceiveBonusText}'");
    }

    [ClientRpc]
    private void RpcLocalLog(string text)
    {
        if(hasAuthority)
            NetworkGameUI.Instance.LocalLog(text);
    }

    public void EnableTradeMenu()
    {
        var myCardList = NetworkGameUI.Instance.PlayerCardList;
        var cardPrefab = NetworkGameUI.Instance.CardPrefab;

        for (int i = 0; i < myCardList.childCount; i++)
        {
            Destroy(myCardList.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < cardsOnHand1.Count; i++)
        {
            var newCard = Instantiate(cardPrefab, myCardList);
            newCard.Populate(cardsOnHand1[i], 1, false, false);
        }
        
        for (int i = 0; i < cardsOnHand2.Count; i++)
        {
            var newCard = Instantiate(cardPrefab, myCardList);
            newCard.Populate(cardsOnHand2[i], 2, false, false);
        }
        
        for (int i = 0; i < cardsOnHand3.Count; i++)
        {
            var newCard = Instantiate(cardPrefab, myCardList);
            newCard.Populate(cardsOnHand3[i], 3, false, false);
        }
        
        for (int i = 0; i < luckCardAmount; i++)
        {
            var newCard = Instantiate(cardPrefab, myCardList);
            newCard.Populate(0, 0, false, true);
        }

    }

    public void EnableTargetTradeMenu()
    {
        var myCardList = NetworkGameUI.Instance.TargetCardList;
        var cardPrefab = NetworkGameUI.Instance.CardPrefab;
        
        for (int i = 0; i < myCardList.childCount; i++)
        {
            Destroy(myCardList.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < cardsOnHand1.Count; i++)
        {
            var newCard = Instantiate(cardPrefab, myCardList);
            newCard.Populate(cardsOnHand1[i], 1, true, false);
        }
        
        for (int i = 0; i < cardsOnHand2.Count; i++)
        {
            var newCard = Instantiate(cardPrefab, myCardList);
            newCard.Populate(cardsOnHand2[i], 2, true, false);
        }
        
        for (int i = 0; i < cardsOnHand3.Count; i++)
        {
            var newCard = Instantiate(cardPrefab, myCardList);
            newCard.Populate(cardsOnHand3[i], 3, true, false);
        }
        
        for (int i = 0; i < luckCardAmount; i++)
        {
            var newCard = Instantiate(cardPrefab, myCardList);
            newCard.Populate(0, 0, true, true);
        }
    }

    public void ProposeTrade(List<ResourceCardUI> selectedCards, List<ResourceCardUI> selectedTargetCards, PlayerCharacterCard chosenTradePlayer)
    {
        List<List<int>> selectedCards1 = new List<List<int>>();
        List<List<int>> selectedTargetCards1 = new List<List<int>>();

        for (int i = 0; i < 3; i++)
        {
            selectedCards1.Add(new List<int>());
        }
        
        for (int i = 0; i < 3; i++)
        {
            selectedTargetCards1.Add(new List<int>());
        }
        
        /*selectedCards1 = selectedCards.Select(((card, i) =>
        {
            new {i, c = card.CardIndex};
        }))*/
        for (int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards1[selectedCards[i].ActNumber - 1].Add(selectedCards[i].CardIndex);
        }
        
        for (int i = 0; i < selectedTargetCards.Count; i++)
        {
            selectedTargetCards1[selectedTargetCards[i].ActNumber - 1].Add(selectedTargetCards[i].CardIndex);
        }

        var player = chosenTradePlayer.Player.PlayerNumber;
        
        tradeStatus = TradeStatus.Pending;
        NetworkGameUI.Instance.WaitTradeResult();

        CmdProposeTrade(selectedCards1, selectedTargetCards1, player);

        StartCoroutine(WaitForTradeResult(selectedCards1, selectedTargetCards1, chosenTradePlayer.Player));
    }

    private IEnumerator WaitForTradeResult(List<List<int>> selectedCards, List<List<int>> selectedTargetCards, PlayerSetup chosenTradePlayerNumber)
    {
        while (tradeStatus == TradeStatus.Pending)
        {
            yield return new WaitForEndOfFrame();
            Debug.LogWarning("Waiting for trade...");
        }

        if (tradeStatus == TradeStatus.Accepted)
        {
            CmdConcludeTrade(selectedCards, selectedTargetCards, chosenTradePlayerNumber.PlayerNumber);
            var originName = NetworkGameController.instance.CharacterList[chosenTradePlayerNumber.characterInfoIndex];
            var myName = NetworkGameController.instance.CharacterList[characterInfoIndex];
            NetworkGameUI.Instance.RpcLog($"Os jogadores {myName.Name} e {originName.Name} firmaram uma troca!");
            NetworkGameUI.Instance.ReturnFromWaitTrade();
        }
        else
        {
            NetworkGameUI.Instance.LocalLog("Sua troca foi negada!");
            NetworkGameUI.Instance.ReturnFromWaitTrade();
        }

        tradeStatus = TradeStatus.NotUsed;
    }

    [Command]
    private void CmdConcludeTrade(List<List<int>> selectedMyCards, List<List<int>> selectedTargetCards, int chosenTradePlayerNumber)
    {
        PlayerSetup player = playerControllers.Where((setup => setup.PlayerNumber == chosenTradePlayerNumber)).First();

        for (int i = 0; i < selectedTargetCards[0].Count; i++)
        {
            player.cardsOnHand1.Remove(selectedTargetCards[0][i]);
        }
        
        for (int i = 0; i < selectedTargetCards[1].Count; i++)
        {
            player.cardsOnHand2.Remove(selectedTargetCards[1][i]);
        }
        for (int i = 0; i < selectedTargetCards[2].Count; i++)
        {
            player.cardsOnHand3.Remove(selectedTargetCards[2][i]);
        }
        
        player.cardsOnHand1.AddRange(selectedMyCards[0]);
        player.cardsOnHand2.AddRange(selectedMyCards[1]);
        player.cardsOnHand3.AddRange(selectedMyCards[2]);
        
        for (int i = 0; i < selectedMyCards[0].Count; i++)
        {
            cardsOnHand1.Remove(selectedMyCards[0][i]);
        }
        
        for (int i = 0; i < selectedMyCards[1].Count; i++)
        {
            cardsOnHand2.Remove(selectedMyCards[1][i]);
        }
        for (int i = 0; i < selectedMyCards[2].Count; i++)
        {
            cardsOnHand3.Remove(selectedMyCards[2][i]);
        }    
        
        cardsOnHand1.AddRange(selectedTargetCards[0]);
        cardsOnHand2.AddRange(selectedTargetCards[1]);
        cardsOnHand3.AddRange(selectedTargetCards[2]);
    }

    [Command]
    private void CmdProposeTrade(List<List<int>> selectedCards, List<List<int>> selectedTargetCards, int chosenTradePlayerNumber)
    {

        PlayerSetup player = playerControllers.Where((setup => setup.PlayerNumber == chosenTradePlayerNumber)).First();

        player.RpcPresentTrade(selectedCards, selectedTargetCards, playerNumber);
    }

    [ClientRpc]
    private void RpcPresentTrade(List<List<int>> originCard, List<List<int>> selectedMyCards, int playerNumber)
    {
        if (hasAuthority)
        {
            NetworkGameUI.Instance.PresentTrade(originCard, selectedMyCards, playerNumber);
        }
        else
        {
            Debug.Log("No authority to show window etc");
        }
    }

    [SyncVar, SerializeField] private PlayerSetup currentTradeOrigin;

    public void AcceptTrade(PlayerSetup tradeOrigin)
    {
        currentTradeOrigin = tradeOrigin;
        Debug.Log("Trade response!! POSITIVE");
        
        CmdAcceptTrade(currentTradeOrigin.PlayerNumber);
        NetworkGameUI.Instance.ReturnFromTradeProposal();
    }

    [Command]
    private void CmdAcceptTrade(int number)
    {
        PlayerSetup player = playerControllers.Where((setup => setup.PlayerNumber == number)).First();

        try
        {
            player._TradeStatus = TradeStatus.Accepted;
        } 
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void RefuseTrade(PlayerSetup tradeOrigin)
    {
        currentTradeOrigin = tradeOrigin;
        Debug.Log("Trade response!! NEGATIVE");
        CmdRefuseTrade(currentTradeOrigin.PlayerNumber);
        NetworkGameUI.Instance.ReturnFromTradeProposal();
    }

    [Command]
    private void CmdRefuseTrade(int number)
    {
        PlayerSetup player = playerControllers.Where((setup => setup.PlayerNumber == number)).First();

        try
        {
            player._TradeStatus = TradeStatus.Denied;
        } 
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }

    public TradeStatus _TradeStatus { get => tradeStatus; set => tradeStatus = value; }

    public void VoteOnCharacter(int i)
    {
        CmdVoteOnCharacter(i);
    }

    [Command]
    public void CmdVoteOnCharacter(int playerNumber)
    {
        NetworkGameController.instance.VoteOnCharacter(playerNumber);
    }
}
