using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyServer : MonoBehaviour
{
    static public DummyServer instance;

    private List<PlayerController> players;

    private Dictionary<string, SlotController> slots;

    private GameController gameController;

    private int playerTurnID;

    private int turn;


    public int NumberOFPlayers { get => players.Count; }

    private void Awake()
    {
        Singleton();

        Init();
    }

    private void Singleton()
    {
        if (instance)
            Destroy(gameObject);
        instance = this;
    }

    private void Init()
    {
        DontDestroyOnLoad(this);

        gameController = GameController.instance;

        players = new List<PlayerController>();

        slots = new Dictionary<string, SlotController>();

        playerTurnID = 0;
        turn = 1;
    }

    public void SetPlayerID(PlayerController player)
    {
        int playerID = players.Count - 1;

        player.PlayerNumber = playerID; //Set the player number to match the index

        player.MyColor = gameController.GetPlayerColor(playerID);
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

        //slots[slotID].SetSelectedSlot(players[playerTurnID].MyColor);

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

    public void NextTurn()
    {
        players[playerTurnID].EndYourTurn();

        playerTurnID++;
        if(playerTurnID >= players.Count)
        {
            playerTurnID = 0;
        }
        turn ++;
        players[playerTurnID].StartYourTurn();
    }
}
