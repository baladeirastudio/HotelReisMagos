using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleNameChange))] [SerializeField] private string playerName = "Player";
    [SyncVar(hook = nameof(HandleEntryChange))] [SerializeField] private int playerEntry;

    public string PlayerName
    {
        get => playerName;
        set => playerName = value;
    }
    
    public int PlayerEntry
    {
        get => playerEntry;
        set => playerEntry = value;
    }

    public void HandleNameChange(string old, string newName)
    {
        Debug.Log($"My entry is {playerEntry}", gameObject);
        (NetworkManager.singleton as NetworkManagerCardGame).Entries[playerEntry].ForceChangeDisplayName(newName);
    }

    public void HandleEntryChange(int old, int newVal)
    {
        
    }

    private void Update()
    {
        Debug.Log("Authority-not.", gameObject);
        if (hasAuthority)
        {
            Debug.Log("Authority!", gameObject);
            CmdSetDisplayName($"Player {playerEntry + Time.deltaTime}");
        }
    }

    //Called on OnServerAddPlayer
    [Server]
    public void RpcTrackEntry(int newEntry)
    {
        //(NetworkManager.singleton as NetworkManagerCardGame).Entries[playerEntry].Enable();
        
        (NetworkManager.singleton as NetworkManagerCardGame).Entries[playerEntry].Track(this);
        Debug.Log("Spawning entry2");
    }

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
    
    [Server]
    public void SetEntryIndex(int newEntry)
    {
        playerEntry = newEntry;
    }

}
