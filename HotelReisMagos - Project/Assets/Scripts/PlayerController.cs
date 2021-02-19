using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerNumber;

    private bool isMyTurn = false;

    public int PlayerNumber { get => playerNumber; set => playerNumber = value; }

    private void Start()
    {
        GameController.instance.AddPlayer(this);
    }

    public void IsYourTurn()
    {
        isMyTurn = true;
    }
}
