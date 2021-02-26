using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    [SerializeField] private string id;
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
        DummyServer.instance.RegisterSlot(this);
    }

    public void OnClick()
    {
        GameController gameController = GameController.instance;

        DummyServer.instance.SelectSlot(this);

    }

    public void SetSelectedSlot(Color color)
    {
        slotImage.color = color;

        slotImage.raycastTarget = false;
    }

    public void resetButton()
    {
        slotImage.color = Color.white;

        slotImage.raycastTarget = true;
    }
}
