using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public enum SlotReward
{
    MediaResource, SocialResource, EconomicalResource, PoliticalResource,
    LuckCard
}

public class SlotController : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private string id;
    [SerializeField] private int actToUnlock;
    [SerializeField] private int actNumber, actId;
    [SerializeField] private SlotReward rewardType;
    [Tooltip("Set to true and the slot will give a luck card as well as the resource.")]
    [SerializeField] private bool giveLuckCard;
    [Tooltip("Minimum and max reward, inclusive and exclusive, respectively.")]
    [SerializeField] private int minReward = 10, maxReward = 16;

    [SerializeField] private Image resourcePreview;

    [Header("Sprites")] 
    [SerializeField] 
    private Sprite economic;

    [SerializeField] private Sprite social, media, political;
    
    

    public static Dictionary<string, SlotController> slots = new Dictionary<string, SlotController>();
    
    public string ID { get => id; }

    public int ActToUnlock => actToUnlock;

    public SlotReward RewardType => rewardType;

    public bool GiveLuckCard => giveLuckCard;

    public int MinReward
    {
        get => minReward;
        set => minReward = value;
    }

    public int MaxReward
    {
        get => maxReward;
        set => maxReward = value;
    }

    public bool isSelected;

    private Image slotImage;

    private void Awake()
    {
        Init();
    }

    public void RegisterSlot(SlotController slot)
    {
        if (!button)
        {
            button = GetComponent<Button>();
        }
        
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
        NetworkGameController.OnResetSlots.AddListener(ResetSlot);
    }

    private void Init()
    {
        slotImage = GetComponent<Image>();

        if (!slotImage)
            Debug.LogError("Missing Image component!");

        NetworkGameController.OnChangeCurrentAct += (old, val) =>
        {
            if (val == actToUnlock)
            {
                button.interactable = true;
            }
        };

        switch (rewardType)
        {
            case SlotReward.EconomicalResource:
                resourcePreview.sprite = economic;
                break;
            case SlotReward.MediaResource:
                resourcePreview.sprite = media;
                break;
            case SlotReward.PoliticalResource:
                resourcePreview.sprite = political;
                break;
            case SlotReward.SocialResource:
                resourcePreview.sprite = social;
                break;
        }
    }

    private void Start()
    {
        //var server = NetworkManager.singleton as NetworkManagerCardGame;
        //server.RegisterSlot(this);
        RegisterSlot(this);
        //DummyServer.Instance.RegisterSlot(this);

    }

    private void Update()
    {
        
    }

    public void OnClick()
    {
        var server = NetworkManager.singleton as NetworkManagerCardGame;
        //server.CmdSelectSlot(this);

        if (NetworkGameController.instance.CurrentAct < actToUnlock)
        {
            NetworkGameUI.Instance.LocalLog($"Este espaço só abre no ato {actToUnlock}. O ato atual é {NetworkGameController.instance.CurrentAct}");
            Debug.LogWarning($"Wrong act. This slot only opens on act {actToUnlock}. " +
                             $"Current act: {NetworkGameController.instance.CurrentAct}");
            return;
        }
        if (PlayerSetup.localPlayerSetup.ChoseSlot)
        {
            NetworkGameUI.Instance.LocalLog("Você já escolheu um espaço neste turno.");
            Debug.LogWarning("You already chose a slot.");
            return;
        }
        
        if(NetworkGameController.instance.PlayerTurnID == PlayerSetup.localPlayerSetup.PlayerNumber)
            PlayerSetup.localPlayerSetup.CmdSelectSlot(this);
        else
        {
            NetworkGameUI.Instance.LocalLog("Não é seu turno.");
            Debug.LogWarning("Not your turn.");
        }
        //DummyServer.Instance.SelectSlot(this);  
    }

    public void SetSelectedSlot(Color color, int playerNum=-1)
    {
        isSelected = true;
        slotImage.color = color;

        slotImage.raycastTarget = false;
        if(playerNum != -1)
            PlayerSetup.localPlayerSetup.UpdateSlots(id, playerNum);
        
        //PlayerSetup.playerControllers.Find((setup => setup.PlayerNumber == playerNum)).UpdateSlots(id, playerNum);
    }

    public void ResetSlot()
    {
        isSelected = false;
        slotImage.color = Color.white;

        slotImage.raycastTarget = true;
    }

    public void TryPreviewResource(bool enable)
    {
        if (button.interactable)
        {
            resourcePreview.gameObject.SetActive(enable);
        }
    }
}
