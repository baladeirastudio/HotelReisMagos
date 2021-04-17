using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkGameUI : NetworkBehaviour
{
    private static NetworkGameUI instance;

    public static NetworkGameUI Instance => instance;
    
    [SerializeField] private PlayerCard playerCardPrefab;
    [SerializeField] private Transform playerListGroup;
    [SerializeField] private List<PlayerCard> playerCards;
    
    [SerializeField] private List<PlayerSetup> players = new List<PlayerSetup>();
    
    private void Awake()
    {
        instance = this;
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
        for (int i = 0; i < newPlayers.Count; i++)
        {
            players.Add(newPlayers[i].GetComponent<PlayerSetup>());
        }
        //CmdPlayersAddPlayer(player);
        
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
}