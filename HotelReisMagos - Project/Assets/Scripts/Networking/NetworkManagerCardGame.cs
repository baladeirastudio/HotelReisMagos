using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Authenticators;
using UnityEngine;

public class NetworkManagerCardGame : NetworkManager
{
    [Header("The below variables refer to the game's variables.")] 
    [SerializeField] private int maxNumOfRounds;

    [SerializeField] private PlayerListEntry playerListEntryPrefab;
    [SerializeField] private List<PlayerListEntry> entries;
    
    [SerializeField] private RectTransform playerListGroup;
    [SerializeField] private GameObject loginPanel, lobbyPanel;

    public List<PlayerListEntry> Entries => entries;

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("Connected to server.");
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Debug.Log("Disconnected from server.");
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("A player was added");
        
        var player = conn.identity.gameObject.GetComponent<PlayerSetup>();
        
        player.SetEntryIndex(numPlayers-1);
        
        player.SetDisplayName($"Player {numPlayers}");
        player.RpcTrackEntry(numPlayers - 1);
    }

}
