using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Chat;
using Steamworks;
using TMPro;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleNameChange))] [SerializeField] private string playerName = "Player";
    [SyncVar(hook = nameof(HandleEntryChange))] [SerializeField] private int playerEntry;
    [SerializeField] private PlayerListEntry myEntry;
    
    public Action<string, string, PlayerSetup> onChangeName;

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
        Debug.Log($"My entry is {playerEntry}", gameObject);
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
    
    [ClientRpc]
    public void RpcSetEntryIndex(int newEntry)
    {
        playerEntry = newEntry;
    }
    
    [ClientRpc]
    public void RpcChangeNameEvent(string old, string newVal, PlayerSetup player)
    {
        onChangeName?.Invoke(old, newVal, player);
    }
    
    
    public void SetupSteamUserName()
    {
        playerName = SteamFriends.GetPersonaName();
    }
}
