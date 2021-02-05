using System;
using Mirror;
using Mirror.Authenticators;
using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private string playerName = "Player";
    [SerializeField] private TMP_InputField nameInput;
    
    public static string clientName;
    
    private void Awake()
    {
        if (PlayerPrefs.HasKey("CurrentPlayerName"))
        {
            playerName = PlayerPrefs.GetString("CurrentPlayerName");
            clientName = playerName;
            nameInput.SetTextWithoutNotify(playerName);
        }
    }

    public void UpdatePlayerName (string newName)
    {
        playerName = newName;
        clientName = playerName;
        PlayerPrefs.SetString("CurrentPlayerName", playerName);
        PlayerPrefs.Save();
    }

    public void Connect()
    {
        NetworkManager.singleton.StartClient();
    }
    
    public void Host()
    {
        NetworkManager.singleton.StartHost();
    }
}