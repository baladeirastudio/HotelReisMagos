using System;
using System.Collections;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using Mirror.Authenticators;
using Mirror.FizzySteam;
using UnityEngine;

public class NetworkManagerCardGame : NetworkManager
{
    [Header("The below variables refer to the game's variables.")] 
    [SerializeField] private int maxNumOfRounds;
    [SerializeField] private GameObject loginPanel, lobbyPanel;
    [SerializeField] private NetworkLobbyUI lobbyUi;
    [SerializeField] private NetworkGameUI gameUi;
    [SerializeField] private KcpTransport kcpTransportPrefab;
    [SerializeField] private FizzySteamworks steamTransportPrefab;

    public List<PlayerSetup> players = new List<PlayerSetup>();
    public List<NetworkIdentity> identities = new List<NetworkIdentity>();

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("Connected to server.");
        
        if(loginPanel)
            loginPanel.SetActive(false);
        if(lobbyPanel)
            lobbyPanel.SetActive(true);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Debug.Log("Disconnected from server.");
        
        if(loginPanel)
            loginPanel.SetActive(true);
        if(lobbyPanel)
            lobbyPanel.SetActive(false);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("A player was added");
        var player = conn.identity.gameObject.GetComponent<PlayerSetup>();
        //player.FetchSteamData();
        //PlayerSetup.playerControllers.Add(player);
        players.Add(player);
        identities.Add(conn.identity);
        player.PlayerNumber = numPlayers - 1;

        RegisterOnPlayerList(player);
        
        player.MyColor = GetPlayerColor(player.PlayerNumber);

        if (lobbyUi)
        {
            lobbyUi.RpcSpawnPlayerEntry(conn.identity, identities);
        }

        if (gameUi)
        {
            gameUi.RpcSpawnPlayer(identities);
        }
        else
        {
            try
            {
                //NetworkGameUI.Instance.RpcSpawnPlayer(identities);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"Current networkgameUI: {NetworkGameUI.Instance}");
            }
        }

        /*var newEntry = Instantiate(playerListEntryPrefab, playerListGroup);
        NetworkServer.Spawn(newEntry.gameObject, conn.identity.connectionToClient);
        entries.Add(newEntry);
        
        
        player.RpcSetEntryIndex(numPlayers-1);
        player.RpcTrackEntry(newEntry);*/
        //player.SetDisplayName($"Player {numPlayers}");
    }

    public void PrintSomething()
    {
        Debug.Log("Something");
    }

    
    
    
    private List<Color> playersColors;

    private void InitColors()
    {
        playersColors = new List<Color>();

        playersColors.Add(new Color(0, 1, 1, 1));
        playersColors.Add(new Color(1, 0, 1, 1));
        playersColors.Add(new Color(1, 1, 0, 1));
        playersColors.Add(new Color(0, 0, 1, 1));
        playersColors.Add(new Color(0, 1, 0, 1));
        playersColors.Add(new Color(1, 0, 0, 1));
    }

    public Color GetPlayerColor(int playerID)
    {
        return playersColors[playerID];
    }
    
    
    
    

    //static public DummyServer instance;

    [SerializeField] private List<PlayerController> __playerControllers;

    private Dictionary<string, SlotController> slots;

    public NetworkGameController gameController;
    [SerializeField] private GameObject startMatchButton;

    public Dictionary<string, SlotController> Slots => slots;

    public int NumberOFPlayers { get => PlayerSetup.playerControllers.Count; }

    public override void Awake()
    {
        base.Awake();
        steamTransportPrefab = GetComponent<FizzySteamworks>();
        kcpTransportPrefab = GetComponent<KcpTransport>();

        slots = new Dictionary<string, SlotController>();
        InitColors();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (startMatchButton)
            startMatchButton.SetActive(true);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(InitGameController());

    }

    private IEnumerator InitGameController()
    {
        //DontDestroyOnLoad(this);
        yield return null;
        //yield return new WaitForEndOfFrame();
        if (NetworkGameController.instance)
        {
            try
            {
                gameController = NetworkGameController.instance;
                //gameController = FindObjectOfType<NetworkGameController>();
                gameController.PlayerTurnID = 0;
                gameController.Turn = 1;
                
                gameUi = NetworkGameUI.Instance;
                Debug.Log("SUCCESSSSSSSSSS!!!!!!!");
            }
            catch (Exception e)
            {
                Debug.LogError("CATCH EXCEPTION");
                Debug.LogException(e);
                //throw;
            }
        }
    }

    public IEnumerator SetPlayerID(PlayerSetup player)
    {
        //int playerID = PlayerSetup.playerControllers.Count - 1;

        //player.PlayerNumber = playerID; //Set the player number to match the index

        yield return new WaitUntil(() => gameController);
        
        player.MyColor = GetPlayerColor(player.PlayerNumber);

        try
        {
            player.MyColor = gameController.GetPlayerColor(player.PlayerNumber);
        }
        catch (Exception e)
        {
            Debug.Log("catch!");
            Debug.LogException(e);
        }
    }

    public void EnableSteamTransport()
    {
        kcpTransportPrefab.enabled = false;
        steamTransportPrefab.enabled = true;
        
        transport = steamTransportPrefab;
        Transport.activeTransport = steamTransportPrefab;

        //Destroy(transport.gameObject);
        //transport = Instantiate(steamTransportPrefab);
    }

    public void EnableKcpTransport()
    {
        steamTransportPrefab.Shutdown();
        steamTransportPrefab.enabled = false;
        kcpTransportPrefab.enabled = true;
        
        transport = kcpTransportPrefab;
        Transport.activeTransport = kcpTransportPrefab;
        
        //Destroy(transport.gameObject);
        //transport = Instantiate(kcpTransportPrefab);
    }

    public void SelectSlot(SlotController slot)
    {
        /*
        if (playerID != playerTurnID)
        {
            Debug.LogError("is NOT the PLAYER ROUND!");
            return;
        }*/

        string slotID = slot.ID;

        if(slots[slotID].isSelected)
        {
            Debug.LogError("Slot ALREADY SELECTED!");
            return;
        }

        //slots[slotID].SetSelectedSlot(PlayerSetup.playerControllers[gameController.PlayerTurnID].MyColor);

        //RpcNextTurn();
    }

    public void RegisterOnPlayerList(PlayerSetup player)
    {
        /*if (playerControllers.Count > 5)
        {
            Debug.LogError("Tryed to ADD MORE PLAYERS than can be add");
            return;
        }*/

        //Verify if the player is already in the list        
        foreach (var p in PlayerSetup.playerControllers)
        {
            if (p == player)
            {
                Debug.LogError("Tryed to add the SAME PLAYER more than once");
                return;
            }
        }


        StartCoroutine(SetPlayerID(player));
    }

    public void RegisterSlot(SlotController slot)
    {
        SlotController value;
        if (slots.TryGetValue(slot.ID, out value))
        {
            if(value == slot)
                Debug.LogError("Slot ALREADY ADDED to the Dictionary!");
            else
                Debug.LogError("Slot with SAME ID!");

            return;
        }

        slots.Add(slot.ID, slot);
    }

    public bool IsSlotSelected(string id)
    {
        return SlotController.slots[id].isSelected;
        //return slots[id].isSelected;
    }
}
