﻿using System;
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
    USE_LUCKY_CARD = 5,
    DEFEND_IDEA = 6,
}*/

public class GameController : MonoBehaviour
{
    static public GameController instance;

    [SerializeField] private int playerTurnID;
    [SerializeField] private int turn;

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
        Singleton();

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
            Destroy(gameObject);
        instance = this;
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
}
