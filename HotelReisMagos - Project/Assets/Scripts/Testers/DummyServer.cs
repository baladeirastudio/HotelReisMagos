using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyServer : MonoBehaviour
{
    static public DummyServer Instance { get; set; }

    private List<PlayerController> players;

    private Dictionary<string, SlotController> slots;

    private int playerTurnID;

    private int currentTurn = 1;
    public int currentActo = 1;

    public int NumberOFPlayers { get => players.Count; }

    private void Awake()
    {
        Singleton();
        Init();
    }

    private void Singleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        players = new List<PlayerController>();

        slots = new Dictionary<string, SlotController>();

        playerTurnID = 0;
        currentTurn = 1;
        currentActo = 1;
    }

    public void SetPlayerID(PlayerController player)
    {
        int playerID = players.Count - 1;

        player.PlayerNumber = playerID; //Set the player number to match the index

        player.MyColor = GameController.Instance.GetPlayerColor(playerID);
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

        slots[slotID].SetSelectedSlot(players[playerTurnID].MyColor);

        NextTurn();
    }

    public void ConnectToSever(PlayerController player)
    {
        if (players.Count > 5)
        {
            Debug.LogError("Tryed to ADD MORE PLAYERS than can be add");
            return;
        }

        //Verify if the player is already in the list        
        foreach (var p in players)
        {
            if (p == player)
            {
                Debug.LogError("Tryed to add the SAME PLAYER more than once");
                return;
            }
        }

        players.Add(player);

        SetPlayerID(player);
    }

    public void RegisterSlot(SlotController slot)
    {
        if (!slots.TryGetValue(slot.ID, out SlotController value))
        {
            slots.Add(slot.ID, slot);
        }
        else
        {
            if (value == slot)
            {
                Debug.LogError("Slot ALREADY ADDED to the Dictionary!");
            }
            else
            {
                Debug.LogError("Slot with SAME ID!");
                return;
            }
        }
    }

    public void NextTurn()
    {
        players[playerTurnID].EndYourTurn();

        playerTurnID++;
        if(playerTurnID >= players.Count)
        {
            playerTurnID = 0;
        }
        currentTurn++;
        players[playerTurnID].StartYourTurn();
    }

    public void NextActo()
    {
        //TODO: Criar limite de atos
        currentActo += 1;
        GameController.Instance.InitSlots();
    }

    public int GetActo()
    {
        return currentActo;
    }
}
