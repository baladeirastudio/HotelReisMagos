using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;

public class PlayerListEntry : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private GameObject view;

    private PlayerSetup myPlayer;

    public void Track(PlayerSetup playerSetup)
    {
        myPlayer = playerSetup;
        nameLabel.gameObject.SetActive(true);

        /*if (playerSetup.hasAuthority)
        {
            nameLabel.gameObject.SetActive(false);
            nameInputField.gameObject.SetActive(true);
        }
        else
        {
            nameLabel.gameObject.SetActive(true);
        }*/
        ForceChangeDisplayName(playerSetup.PlayerName);
    }

    public void ForceChangeDisplayName(string newName)
    {
        nameLabel.SetText(newName);
        nameInputField.SetTextWithoutNotify(newName);
    }
}