﻿using System;
using Mirror;
using Mirror.Authenticators;
using Steamworks;
using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private string playerName = "Player";
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private bool useSteam = false;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequest;
    protected Callback<LobbyEnter_t> lobbyEnter;
    
    public static string clientName;
    [SerializeField] private string steamID;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("CurrentPlayerName"))
        {
            playerName = PlayerPrefs.GetString("CurrentPlayerName");
            clientName = playerName;
            nameInput.SetTextWithoutNotify(playerName);
        }
    }

    private void Start()
    {
        if (!useSteam)
        {
            return;
        }
        else
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequest);
            lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) //Means the executing client is the host.
        {
            return;
        }
        else
        {
            string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), 
                "HostAddress");

            NetworkManager.singleton.networkAddress = hostAddress;
            NetworkManager.singleton.StartClient();
        }
    }

    private void OnGameLobbyJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to create lobby!");
            return;
        }
        else
        {
            NetworkManager.singleton.StartHost();
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress",
                SteamUser.GetSteamID().ToString());
        }
    }

    public void UpdatePlayerName (string newName)
    {
        playerName = newName;
        clientName = playerName;
        PlayerPrefs.SetString("CurrentPlayerName", playerName);
        PlayerPrefs.Save();
    }
    
    public void UpdateSteamID (string newID)
    {
        steamID = newID;

        /*playerName = newName;
        clientName = playerName;
        PlayerPrefs.SetString("CurrentPlayerName", playerName);
        PlayerPrefs.Save();*/
    }

    public void Connect()
    {
        NetworkManager.singleton.networkAddress = steamID;
        NetworkManager.singleton.StartClient();
    }
    
    public void Host()
    {
        if (useSteam)
        {
            Debug.Log("USING STEAM");
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            return;
        }
        else
        {
            NetworkManager.singleton.StartHost();
        }
    }
}