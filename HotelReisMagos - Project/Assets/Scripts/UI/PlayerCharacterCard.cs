using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Toggle selectionToggle;
    [SerializeField] private PlayerSetup player;
    
    [SerializeField] private int playerNumberId, playerListIndex;

    public PlayerSetup Player => player;
    
    public void OnToggle(bool isSelected)
    {
        NetworkGameUI.Instance.SelectTradePlayer(this);
    }

    public void Populate(int playerListIndex, PlayerSetup player)
    {
        this.player = player;
        Debug.LogError($"CharCardIndex: {player.CharacterInfoIndex}");    
        nameText.SetText(NetworkGameController.instance.CharacterList[player.CharacterInfoIndex].Name);
        playerNumberId = player.PlayerNumber;
        this.playerListIndex = playerListIndex;
    }

    public void Deselect()
    {
        selectionToggle.SetIsOnWithoutNotify(false);
    }

    public void Select()
    {
        selectionToggle.SetIsOnWithoutNotify(true);
    }
}