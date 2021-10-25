using TMPro;
using UnityEngine;

public class TradeCardInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText, idText;

    public TextMeshProUGUI DescriptionText => descriptionText;

    public TextMeshProUGUI IdText => idText;

    public void Populate(int cardIndex, int actNumber, bool isTarget, bool isLuck = false)
    {

        if (!isLuck)
        {
            CardInfo cardInfo = null;

            switch (actNumber)
            {
                case 1:
                    cardInfo = NetworkGameController.instance.cardList1[cardIndex];
                    break;
                case 2:
                    cardInfo = NetworkGameController.instance.cardList2[cardIndex];
                    break;
                case 3:
                    cardInfo = NetworkGameController.instance.cardList3[cardIndex];
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
}