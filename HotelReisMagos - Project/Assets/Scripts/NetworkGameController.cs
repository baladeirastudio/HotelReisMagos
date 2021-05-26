using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkGameController : NetworkBehaviour
{
    public static NetworkGameController instance;

    [SyncVar(hook =nameof(OnChangePlayerTurn)), SerializeField] private int playerTurnID;
    [SyncVar, SerializeField] private int turn;
    [Tooltip("Starts with 1.")]
    [SyncVar(hook = nameof(OnChangeAct)), SerializeField] private int currentAct;

    [SerializeField] public SyncList<CardInfo> cardList1 = new SyncList<CardInfo>(),
        cardList2 = new SyncList<CardInfo>(),
        cardList3 = new SyncList<CardInfo>();
    [SerializeField] public SyncList<int> activeCards1 = new SyncList<int>(),
        activeCards2 = new SyncList<int>(), 
        activeCards3 = new SyncList<int>();
    [SerializeField] private SyncList<int> takenCards = new SyncList<int>();
    
    [SerializeField] private ActData firstActData, secondActData, thirdActData;

    public NetworkManagerCardGame server;

    public delegate void DelegateIntInt(int old, int newVal);

    public static event DelegateIntInt OnChangePlayerTurnId;
    public static event DelegateIntInt OnChangeTurn;
    public static event DelegateIntInt OnChangeCurrentAct;

    public ActData FirstActData => firstActData;

    public ActData SecondActData => secondActData;

    public ActData ThirdActData => thirdActData;
    
    public int CurrentAct
    {
        get => currentAct;
        set => currentAct = value;
    }

    public int PlayerTurnID
    {
        get => playerTurnID;
        set => playerTurnID = value;
    }

    public int Turn
    {
        get => turn;
        set => turn = value;
    }
    private List<Color> playersColors;

    private void Awake()
    {
        //Debug.LogError("HELLOOOOOOOOOOOOO");
        Singleton();
        //Debug.LogError("hasllo?");

        Init();
        //server = NetworkManagerCardGame.singleton as NetworkManagerCardGame;
    }

    private void Start()
    {
        server = NetworkManagerCardGame.singleton as NetworkManagerCardGame;
        CardDB currentCards = CardDB.Instance;
        cardList1.AddRange(currentCards.cardList1);
        cardList2.AddRange(currentCards.cardList2);
        cardList3.AddRange(currentCards.cardList3);
        
        activeCards1.AddRange(currentCards.activeCards1);
        activeCards2.AddRange(currentCards.activeCards2);
        activeCards3.AddRange(currentCards.activeCards3);
        
        //TODO: Isso aqui dá cancer.
    }

    private void Singleton()
    {
        if (instance)
            if (instance != this)
            {
                Destroy(gameObject);
                Debug.Log("Nope, already exists a game controller.");
            }

        instance = this;
        
        //Debug.LogError($"Set instance. New instance: {instance}");
    }

    private void Init()
    {
        //DontDestroyOnLoad(this);
        InitColors();
    }

    private void InitColors()
    {
        playersColors = new List<Color>();

        playersColors.Add(new Color(0, 1, 1, 1));
        playersColors.Add(new Color(1, 0, 1, 1));
        playersColors.Add(new Color(1, 1, 0, 1));
        playersColors.Add(new Color(0, 0, 1, 1));
        playersColors.Add(new Color(0, 1, 0, 1));
        playersColors.Add(new Color(1, 0, 0, 1));
    }

    public Color GetPlayerColor(int playerID)
    {
        return playersColors[playerID];
    }
    
    //[ClientRpc]
    public void RpcNextTurn()
    {
        //PlayerSetup.playerControllers[playerTurnID].RpcEndYourTurn();

        playerTurnID++;
        if(playerTurnID >= NetworkManager.singleton.numPlayers)
        {
            playerTurnID = 0;
            turn++;
            for (int i = 0; i < PlayerSetup.playerControllers.Count; i++)
            {
                PlayerSetup.playerControllers[i].ChoseSlot = false;
                PlayerSetup.playerControllers[i].UsedLuckCard = false;
            }
        }

        switch (currentAct)
        {
            case 1: //Avanço do ato 1.
                if (turn >= 3)
                {
                    NetworkGameUI.Instance.RpcLog(firstActData.actEndText.text);
                    turn = 1;
                    currentAct++;
                    NetworkGameUI.Instance.RpcLog(secondActData.actBeginText.text);
                }
                break;
            case 2: //Avanço do ato 2.
                if (turn >= 4)
                {
                    NetworkGameUI.Instance.RpcLog(secondActData.actEndText.text);
                    turn = 1;
                    currentAct++;
                    NetworkGameUI.Instance.RpcLog(thirdActData.actBeginText.text);
                }
                break;
            case 3: //Avanço do ato 3.
                if (turn >= 4)
                {
                    NetworkGameUI.Instance.RpcLog(thirdActData.actEndText.text);
                    
                }
                break;
        }

        //NetworkGameUI.Instance.RpcUpdateActionsMenu();
        //PlayerSetup.playerControllers[playerTurnID].RpcStartYourTurn();
    }

    private void OnDisable()
    {
        //Debug.Log("lol");
    }

    private void OnChangePlayerTurn(int old, int val)
    {
        //Debug.LogError($"Old val: {old} - New: {val}");
        
        OnChangePlayerTurnId?.Invoke(old, val);
    }
    
    private void OnChangeAct(int old, int val)
    {
        //Debug.LogError($"Old val: {old} - New: {val}");
        
        OnChangeCurrentAct?.Invoke(old, val);
    }

    [ClientRpc]
    public void RpcUpdateSlots(string slotId, Color slotColor)
    {
        if(!SlotController.slots[slotId].isSelected)
            SlotController.slots[slotId].SetSelectedSlot(slotColor);
    }
}
