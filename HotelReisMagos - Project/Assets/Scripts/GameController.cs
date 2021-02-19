using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static public GameController instance;

    private List<PlayerController> players;

    private void Awake()
    {
        if(instance)
            Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void AddPlayer(PlayerController player)
    {
        if(players.Count > 5)
        {
            Debug.LogError("Tryed to ADD MORE PLAYERS than can be add");
            return;
        }

        //Verify if the player is already in the list        
        foreach (var p in players)
        {
            if(p == player)
            {
                Debug.LogError("Tryed to add the SAME PLAYER more than once");
                return;
            }
        }

        players.Add(player);
        player.PlayerNumber = players.Count - 1; //Set the player number to match the index
    }

    public void CleanPlayers()
    {
        players.Clear();
    }
}
