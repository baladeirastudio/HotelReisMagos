using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/*enum GameState
{
    OBJECTIVE_CARDS = 0,
    GIVE_CHAR_OBJECTIVES = 1,
    CHOOSE_SLOT = 2,
    GIVE_RESOURCE = 3,
    READ_CARD = 4,
    CHANGE_ACTO = 5,
    USE_LUCKY_CARD = 6,
    DEFEND_IDEA = 7,
}*/

public class GameController : MonoBehaviour
{
    static public GameController Instance { get => instance; set => instance = value; }
    static public GameController instance;

    [SerializeField] private int playerTurnID;
    [SerializeField] private int turn;

    public NetworkManagerCardGame server;

    public int PlayerTurnID
    {
        get => playerTurnID;
        set => playerTurnID = value;
    }
    private List<Color> playersColors = new List<Color>();
    [SerializeField]
    private ActoController[] actos;

    public int Turn
    {
        get => turn;
        set => turn = value;
    }
    //private List<Color> playersColors;
    
    private void Awake()
    {
        Singleton();        
    }

    private void Start()
    {
        Init();
        server = NetworkManagerCardGame.singleton as NetworkManagerCardGame;
    }

    /*private void Start()
    {
        server = NetworkManagerCardGame.singleton as NetworkManagerCardGame;
    }*/

    private void Singleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }            
        else
        {
            Destroy(gameObject);
            instance = this;
        }
    }

    private void Init()
    {        
        InitColors();
        InitSlots();
    }

    public void RegisterPlayer(PlayerSetup player)
    {
        server.RegisterOnPlayerList(player);
        //DummyServer.Instance.ConnectToSever(player);
    }
    
    public void RegisterPlayer(PlayerController player)
    {
        //server.RegisterOnPlayerList(player);
        DummyServer.Instance.ConnectToSever(player);
    }

    private void InitColors()
    {
        playersColors.Add(new Color(0, 1, 1, 1));
        playersColors.Add(new Color(1, 0, 1, 1));
        playersColors.Add(new Color(1, 1, 0, 1));
        playersColors.Add(new Color(0, 0, 1, 1));
        playersColors.Add(new Color(0, 1, 0, 1));
    }

    public void InitSlots()
    {
        foreach (var item in actos)
        {
            if (item.GetID() <= DummyServer.Instance.GetActo())
            {
                item.ResetBoard();
                item.UnlockSlots();
            }
            else
            {
                item.LockSlots();
            }
        }
    }

    public Color GetPlayerColor(int playerID)
    {
        return playersColors[playerID];
    }
    
    public void RpcNextTurn()
    {
        PlayerSetup.playerControllers[playerTurnID].RpcEndYourTurn();

        playerTurnID++;
        if(playerTurnID >= PlayerSetup.playerControllers.Count)
        {
            playerTurnID = 0;
        }
        turn ++;
        PlayerSetup.playerControllers[playerTurnID].RpcStartYourTurn();
    }

    public int NextActo()
    {
        return DummyServer.Instance.GetActo();
    }
}
