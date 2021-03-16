using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListEntry : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private Image pfpImage;
    [SerializeField] private RawImage pfpTex;
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
        ForceChangeDisplayPfp(playerSetup.SteamAvatar);
    }

    private void ForceChangeDisplayPfp(Sprite playerSetupTex)
    {
        //pfpImage.sprite = playerSetupTex;
        pfpTex.texture = myPlayer.ProfilePicture;
    }

    public void ForceChangeDisplayName(string newName)
    {
        nameLabel.SetText(newName);
        nameInputField.SetTextWithoutNotify(newName);
    }
}