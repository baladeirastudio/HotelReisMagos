using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworLobbyUI : NetworkBehaviour
{
    [SerializeField] private RectTransform playerListGroup;
    [SerializeField] private PlayerListEntry playerListEntryPrefab;
    [SerializeField] private List<PlayerSetup> players = new List<PlayerSetup>();

    [ClientRpc]
    public void RpcSpawnPlayerEntry(NetworkIdentity conn, List<NetworkIdentity> newPlayers)
    {
        var player = conn.GetComponent<PlayerSetup>();
        players.Clear();
        
        for (int i = 0; i < newPlayers.Count; i++)
        {
            players.Add(newPlayers[i].GetComponent<PlayerSetup>());
        }
        //CmdPlayersAddPlayer(player);
        
        for (int i = 0; i < playerListGroup.childCount; i++)
        {
            Destroy(playerListGroup.GetChild(i).gameObject);
        }

        StartCoroutine(RefreshListCoroutine());
        
    }

    private IEnumerator RefreshListCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < players.Count; i++)
        {
            TrackPlayer(i);
        }
    }

    private void TrackPlayer(int index)
    {
        if (players[index])
        {
            var newEntry = Instantiate(playerListEntryPrefab, playerListGroup);
            newEntry.Track(players[index]);
            players[index].onChangeName += UpdatePlayerName;
        }
    }

    private void UpdatePlayerName(string old, string newVal, PlayerSetup player)
    {
        player.PlayerEntry.ForceChangeDisplayName(newVal);
    }
}
