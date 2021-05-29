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
    
    public void Populate(int cardIndex, int actNumber, bool isLuck = false)
    {
        this.cardIndex = cardIndex;
        this.actNumber = actNumber;
        
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
