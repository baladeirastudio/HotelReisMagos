using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class tempThing : NetworkBehaviour
{
    public static tempThing instance;

    [SyncVar(hook =nameof(OnChangePlayerTurn)), SerializeField] private int playerTurnID;
    [SyncVar, SerializeField] private int turn;

    public NetworkManagerCardGame server;

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
        Debug.LogError("HELLOOOOOOOOOOOOO");
        Singleton();
        Debug.LogError("hasllo?");

        Init();
        //server = NetworkManagerCardGame.singleton as NetworkManagerCardGame;
    }

    private void Start()
    {
        server = NetworkManagerCardGame.singleton as NetworkManagerCardGame;
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
        
        Debug.LogError($"Set instance. New instance: {instance}");
    }

    private void Init()
    {
        DontDestroyOnLoad(this);
        InitColors();
    }

    public void RegisterPlayer(PlayerSetup player)
    {
        server.RegisterOnPlayerList(player);
    }

    private void InitColors()
    {
        playersColors = new List<Color>();

        playersColors.Add(new Color(0, 1, 1, 1));
        playersColors.Add(new Color(1, 0, 1, 1));
        playersColors.Add(new Color(1, 1, 0, 1));
        playersColors.Add(new Color(0, 0, 1, 1));
        playersColors.Add(new Color(0, 1, 0, 1));
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
        if(playerTurnID >= PlayerSetup.playerControllers.Count)
        {
            playerTurnID = 0;
        }
        turn++;
        //PlayerSetup.playerControllers[playerTurnID].RpcStartYourTurn();
    }

    private void OnDisable()
    {
        Debug.Log("lol");
    }

    private void OnChangePlayerTurn(int old, int val)
    {
        Debug.LogError($"Old val: {old} - New: {val}");
    }
}
