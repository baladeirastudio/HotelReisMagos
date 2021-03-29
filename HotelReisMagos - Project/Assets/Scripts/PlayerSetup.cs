using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Chat;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PlayerSetup : NetworkBehaviour
{
    
    public static List<PlayerSetup> playerControllers;

    [SyncVar(hook = nameof(HandleNameChange))] [SerializeField] private string playerName = "Player";
    [SyncVar(hook = nameof(HandleEntryChange))] [SerializeField] private int playerEntry;
    [SerializeField] private PlayerListEntry myEntry;
    
    public Action<string, string, PlayerSetup> onChangeName;
    [SerializeField] private Texture2D profilePicture = null;
    [SerializeField] private Sprite steamAvatar = null;

    public Sprite SteamAvatar => steamAvatar;
    
    public Texture2D ProfilePicture
    {
        get => profilePicture;
        set => profilePicture = value;
    }

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

        if (isServer && !isServerOnly)
        {
            if (hasAuthority)
            {
                
            }
        }
    }
    
    [ClientRpc]
    public void RpcChangeNameEvent(string old, string newVal, PlayerSetup player)
    {
        onChangeName?.Invoke(old, newVal, player);
    }
    
    
    public void FetchSteamData()
    {
        //playerName = SteamFriends.GetPersonaName();
        playerName = PlayerInfo.clientName;
        profilePicture = PlayerInfo.clientPfp;

        (NetworkManager.singleton as NetworkManagerCardGame).PrintSomething();
        
        
        /*int iAvatar = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());
        SteamUtils.GetImageSize(iAvatar, out uint imgWidth, out uint imgHeight);
        byte[] data = new byte[imgHeight * imgWidth * 4];
        var isValid = SteamUtils.GetImageRGBA(iAvatar, data, (int)( 4 * imgHeight * imgWidth));
        if (isValid)
        {
            profilePicture = new Texture2D((int)imgWidth, (int)imgHeight, TextureFormat.RGBA32, false, true);
            profilePicture.LoadRawTextureData(data);
            profilePicture.Apply();
            
            //steamAvatar = Sprite.Create(profilePicture, new Rect(0, 0, imgWidth, imgHeight), Vector2.one/2 );
            
            //Destroy(profilePicture);
        }
        else
        {
            Debug.LogError("Could not fetch Steam PFP.");
        }*/
        
    }
    
    
    
    
    [SyncVar, SerializeField] private int playerNumber;

    [SyncVar, SerializeField] private bool isMyTurn = false;

    [SyncVar, SerializeField] private Color color;

    public static PlayerSetup localPlayerSetup;

    public int PlayerNumber { get => playerNumber; set => playerNumber = value; }

    public Color MyColor { get => color; set => color = value; }

    private void Start()
    {
        PlayerSetup.playerControllers.Add(this);

        if (tempThing.instance)
        {
            //GameController.instance.RegisterPlayer(this);
        }
        else
            Debug.LogError("Missing Game Controller Instance!");
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        localPlayerSetup = this;

    }

    [ClientRpc]
    public void RpcStartYourTurn()
    {
        if (hasAuthority)
        {
            isMyTurn = true;
        }
    }
    
    [ClientRpc]
    public void RpcEndYourTurn()
    {
        if (hasAuthority)
        {
            isMyTurn = false;
        }
    }

    public void CmdSelectSlot(SlotController slot)
    {
        var temp = NetworkManager.singleton as NetworkManagerCardGame;
        string slotID = slot.ID;

        if(temp.Slots[slotID].isSelected)
        {
            Debug.LogError("Slot ALREADY SELECTED!");
            return;
        }

        temp.Slots[slotID].SetSelectedSlot(PlayerSetup.playerControllers[tempThing.instance.PlayerTurnID].MyColor);

        CmdNext();
    }

    [Command]
    private void CmdNext()
    {
        tempThing.instance.RpcNextTurn();
    }
}
