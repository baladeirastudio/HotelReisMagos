using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Chat;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerSetup : NetworkBehaviour
{
    public static List<PlayerSetup> playerControllers = new List<PlayerSetup>();

    [SyncVar(hook = nameof(HandleNameChange))] [SerializeField] private string playerName = "Player";
    [SerializeField] private PlayerListEntry myEntry;
    
    public Action<string, string, PlayerSetup> onChangeName;
    [SerializeField] private Texture2D profilePicture = null;
    [SerializeField] private Sprite steamAvatar = null;

    [SyncVar, SerializeField] private int politicalResources = 0;
    [SyncVar, SerializeField] private int economicResources = 0;
    [SyncVar, SerializeField] private int socialResources = 0;
    [SyncVar, SerializeField] private int mediaResources = 0;
    [SyncVar, SerializeField] private int luckCardAmount = 0;
    [Tooltip("Minimum and max value to be used in the luck card, inclusive and exclusive.")]
    [SerializeField] private int minLuck = 3, maxLuck = 5;

    public int LuckCardAmount
    {
        get => luckCardAmount;
        set => luckCardAmount = value;
    }
    
    public int PoliticalResources
    {
        get => politicalResources;
        set => politicalResources = value;
    }

    public int EconomicResources
    {
        get => economicResources;
        set => economicResources = value;
    }

    public int SocialResources
    {
        get => socialResources;
        set => socialResources = value;
    }

    public int MediaResources
    {
        get => mediaResources;
        set => mediaResources = value;
    }
    
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
        Debug.Log($"My number is {playerNumber}", gameObject);
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
    
    /*[ClientRpc]
    public void RpcSetEntryIndex(int newEntry)
    {
        playerEntry = newEntry;

        if (isServer && !isServerOnly)
        {
            if (hasAuthority)
            {
                
            }
        }
    }*/
    
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
    private string PERMAslotId;
    [SyncVar, SerializeField] private bool choseSlot = false;
    [SyncVar, SerializeField] private bool usedLuckCard = false;

    public bool UsedLuckCard
    {
        get => usedLuckCard;
        set => usedLuckCard = value;
    }
    
    public bool ChoseSlot
    {
        get => choseSlot;
        set => choseSlot = value;
    }

    public int PlayerNumber { get => playerNumber; set => playerNumber = value; }

    public Color MyColor { get => color; set => color = value; }

    private void Start()
    {
        PlayerSetup.playerControllers.Add(this);
        //Debug.LogError("Adding self.");

        if (NetworkGameController.instance)
        {
            //GameController.instance.RegisterPlayer(this);
        }
        else
        {
            //Debug.LogError("Missing Game Controller Instance!");
        }
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
        PERMAslotId = slot.ID;
        
        CmdCheckSelectedSlot(PERMAslotId);
    }

    [Command]
    private void CmdCheckSelectedSlot(string slotID)
    {
        var temp = NetworkManager.singleton as NetworkManagerCardGame;
        
        if(temp.IsSlotSelected(slotID))
        {
            Debug.LogError("Slot ALREADY SELECTED!");
            return;
        }

        //Debug.LogError("Testing.");
        try
        {
            //Debug.LogError($"Key: {slotID} - Value: {SlotController.slots[slotID]}");
            var tempSlot = SlotController.slots[slotID];
            var tempPlayer = PlayerSetup.playerControllers[NetworkGameController.instance.PlayerTurnID];
            var tempColor = tempPlayer.MyColor;
            tempSlot.SetSelectedSlot(MyColor);

            int reward = Random.Range(tempSlot.MinReward, tempSlot.MaxReward);

            switch (tempSlot.RewardType)
            {
                case SlotReward.PoliticalResource: //Political
                    politicalResources += reward;
                    break;
                case SlotReward.EconomicalResource: //Economical
                    economicResources += reward;
                    break;
                case SlotReward.SocialResource: //Social
                    socialResources += reward;
                    break;
                case SlotReward.MediaResource: //Media
                    mediaResources += reward;
                    break;
                case SlotReward.LuckCard:
                    luckCardAmount++;
                    break;
            }

            if (tempSlot.GiveLuckCard)
            {
                luckCardAmount++;
            }

            NetworkGameUI.Instance.RpcRefreshPlayerCards();
        }
        catch (Exception e)
        {
            Debug.LogError("This shit won't fucking work");
            Debug.LogException(e);
            Debug.LogError($"Player controllers amount: {PlayerSetup.playerControllers.Count} - Player turn ID: {NetworkGameController.instance.PlayerTurnID}");
            Debug.LogError($"Player at previously stated index: {PlayerSetup.playerControllers[NetworkGameController.instance.PlayerTurnID]}");
            Debug.LogError($"Player color: {PlayerSetup.playerControllers[NetworkGameController.instance.PlayerTurnID].MyColor}");
            Debug.LogError($"Slots: {SlotController.slots}");
            Debug.LogError($"Slot ID: {SlotController.slots[slotID]}");
        }

        Debug.LogError("Enabling finish round");
        NetworkGameUI.Instance.RpcEnableFinishRound();
        choseSlot = true;
    }

    public void AdvanceTurn()
    {
        CmdNext();
    }

    [Command]
    private void CmdNext()
    {
        NetworkGameController.instance.RpcNextTurn();
    }

    [Command]
    private void CmdUseLuckCard()
    {
        bool isLuck = Random.Range(0, 2) > 0;

        SlotReward rewardType = (SlotReward)Random.Range(0, 4);
        int reward = Random.Range(minLuck, maxLuck);
        ref int resourceToChange = ref economicResources;
        
        switch (rewardType)
        {
            case SlotReward.EconomicalResource:
                resourceToChange = ref economicResources;
                break;
            case SlotReward.MediaResource:
                resourceToChange = ref mediaResources;
                break;
            case SlotReward.PoliticalResource:
                resourceToChange = ref politicalResources;
                break;
            case SlotReward.SocialResource:
                resourceToChange = socialResources;
                break;
        }

        if (isLuck)
            resourceToChange += reward;
        else
            resourceToChange -= reward;

        luckCardAmount--;
        usedLuckCard = true;
        
        Debug.LogError($"Added {reward * (isLuck ? 1 : -1)}");
    }

    public void UseLuckCard()
    {
        CmdUseLuckCard();
    }

    [Command]
    public void CmdStartMatch()
    {
        (NetworkManagerCardGame.singleton as NetworkManagerCardGame).ServerChangeScene("Game_Lucena"); //TODO: Don't use constants
    }

    public void UpdateSlots(string id)
    {
        CmdUpdateSlots(id);
    }
    
    [Command]
    private void CmdUpdateSlots(string id)
    {
        NetworkGameController.instance.RpcUpdateSlots(id, color);
    }
}
