using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    [SerializeField] private string id;
    [SerializeField] private int actNumber, actId;
    
    public string ID { get => id; }

    public bool isSelected;

    private Image slotImage;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        isSelected = false;

        slotImage = GetComponent<Image>();

        if (!slotImage)
            Debug.LogError("Missing Image component!");
    }

    private void Start()
    {
        var server = NetworkManager.singleton as NetworkManagerCardGame;
        server.RegisterSlot(this);
    }

    public void OnClick()
    {
        var server = NetworkManager.singleton as NetworkManagerCardGame;
        //server.CmdSelectSlot(this);
        
        if(tempThing.instance.PlayerTurnID == PlayerSetup.localPlayerSetup.PlayerNumber)
            PlayerSetup.localPlayerSetup.CmdSelectSlot(this);
        else
        {
            Debug.LogError("Not your turn.");
        }
    }

    public void SetSelectedSlot(Color color)
    {
        isSelected = true;
        slotImage.color = color;

        slotImage.raycastTarget = false;
    }

    public void resetButton()
    {
        isSelected = false;
        slotImage.color = Color.white;

        slotImage.raycastTarget = true;
    }
}
