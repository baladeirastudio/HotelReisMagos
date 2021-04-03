using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    [SerializeField] private string id;
    public string ID { get => id; }

    public bool isSelected;

    private Button button;

    private Image slotImage;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        button = GetComponent<Button>();

        isSelected = false;

        slotImage = GetComponent<Image>();

        if (!slotImage)
            Debug.LogError("Missing Image component!");
    }

    private void Start()
    {
        GameController.Instance.server.RegisterSlot(this);

    }

    public void OnClick()
    {
        if(!button.interactable)
        {
            GameController.Instance.server.SelectSlot(this);       
        }
    }

    public void SetSelectedSlot(Color color)
    {
        isSelected = true;
        slotImage.color = color;

        slotImage.raycastTarget = false;
    }

    public void ResetButton()
    {
        isSelected = false;
        slotImage.color = Color.white;

        slotImage.raycastTarget = true;
    }
}
