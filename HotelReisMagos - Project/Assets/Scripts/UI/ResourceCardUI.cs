using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCardUI : MonoBehaviour
{
    [SerializeField] private Toggle selectionToggle;
    [SerializeField] private bool isLuckCard;
    [SerializeField] private TextMeshProUGUI descriptionText, idText;
    [SerializeField] private int cardIndex, actNumber;
    [SerializeField] private CardInfo cardInfo;
    [SerializeField] private bool isTarget;

    public bool IsLuckCard => isLuckCard;

    public int CardIndex => cardIndex;

    public int ActNumber => actNumber;

    public void Populate(int cardIndex, int actNumber, bool isTarget, bool isLuck = false)
    {
        this.cardIndex = cardIndex;
        this.actNumber = actNumber;
        this.isTarget = isTarget;
        
        if (!isLuck)
        {
            switch (this.actNumber)
            {
                case 1:
                    cardInfo = NetworkGameController.instance.cardList1[this.cardIndex];
                    break;
                case 2:
                    cardInfo = NetworkGameController.instance.cardList2[this.cardIndex];
                    break;
                case 3:
                    cardInfo = NetworkGameController.instance.cardList3[this.cardIndex];
                    break;
            }

            descriptionText.SetText(cardInfo.Description);
            idText.SetText(cardInfo.ID);
        }
        else
        {
            descriptionText.SetText("Sorte ou revés");
            idText.SetText("???");
        }
    }

    public void OnToggle(bool isSelected)
    {
        if (isTarget)
        {
            if (isSelected)
            {
                NetworkGameUI.Instance.SelectedTargetCards.Add(this);
            }
            else
            {
                NetworkGameUI.Instance.SelectedTargetCards.Remove(this);
            }
        }
        else
        {
            if (isSelected)
            {
                NetworkGameUI.Instance.SelectedCards.Add(this);
            }
            else
            {
                NetworkGameUI.Instance.SelectedCards.Remove(this);
            }
        }
    }
}
