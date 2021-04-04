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
        slotImage = GetComponent<Image>();

        if (!slotImage)
            Debug.LogError("Missing Image component!");
    }

    private void Start()
    {
        DummyServer.Instance.RegisterSlot(this);

    }

    public void OnClick()
    {
        DummyServer.Instance.SelectSlot(this);  
    }

    public void SetSelectedSlot(Color color)
    {
        isSelected = true;
        slotImage.color = color;

        slotImage.raycastTarget = false;
    }

    public void ResetSlot()
    {
        isSelected = false;
        slotImage.color = Color.white;

        slotImage.raycastTarget = true;
    }
}
