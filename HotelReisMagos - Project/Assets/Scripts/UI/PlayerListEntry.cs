using Mirror;
using TMPro;
using UnityEngine;

public class PlayerListEntry : NetworkBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI nameLabel;

    private PlayerSetup myPlayer;

    public void Track(PlayerSetup playerSetup)
    {
        myPlayer = playerSetup;
        //nameLabel.SetText(myPlayer.PlayerName);
        nameLabel.gameObject.SetActive(true);
        //nameInputField.gameObject.SetActive(true);
    }

    [SerializeField] private GameObject view;

    [Command]
    public void CmdChangeDisplayName(string newName)
    {
        nameLabel.SetText(newName);
    }
    
    public void ForceChangeDisplayName(string newName)
    {
        nameLabel.SetText(newName);
    }
}