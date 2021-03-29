using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerNumber;

    private bool isMyTurn = false;

    private Color color;

    public int PlayerNumber { get => playerNumber; set => playerNumber = value; }

    public Color MyColor { get => color; set => color = value; }

    private void Start()
    {
        if (tempThing.instance)
        {
            //GameController.instance.RegisterPlayer(this);
        }
        else
            Debug.LogError("Missing Game Controller Instance!");
    }

    public void StartYourTurn()
    {
        isMyTurn = true;
    }
    
    public void EndYourTurn()
    {
        isMyTurn = false;
    }
}
