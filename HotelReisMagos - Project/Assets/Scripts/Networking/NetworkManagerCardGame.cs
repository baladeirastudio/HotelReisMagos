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
    [SerializeField] private GameObject loginPanel, lobbyPanel;
    [SerializeField] private NetworkLobbyUI lobbyUi;

    public List<PlayerSetup> players = new List<PlayerSetup>();
    public List<NetworkIdentity> identities = new List<NetworkIdentity>();

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
        player.FetchSteamData();
        players.Add(player);
        identities.Add(conn.identity);

        lobbyUi.RpcSpawnPlayerEntry(conn.identity, identities);

        /*var newEntry = Instantiate(playerListEntryPrefab, playerListGroup);
        NetworkServer.Spawn(newEntry.gameObject, conn.identity.connectionToClient);
        entries.Add(newEntry);
        
        
        player.RpcSetEntryIndex(numPlayers-1);
        player.RpcTrackEntry(newEntry);*/
        //player.SetDisplayName($"Player {numPlayers}");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
    }
}
