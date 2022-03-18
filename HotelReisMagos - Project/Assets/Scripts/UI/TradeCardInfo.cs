using TMPro;
using UnityEngine;

public class TradeCardInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI idText, politicalText, economicText, socialText, mediaText;

    public TextMeshProUGUI PoliticalText => politicalText;
    public TextMeshProUGUI EconomicText => economicText;
    public TextMeshProUGUI SocialText => socialText;
    public TextMeshProUGUI MediaText => mediaText;
    public TextMeshProUGUI IDText => idText;



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

            politicalText.SetText(cardInfo.PoliticoValue);
            economicText.SetText(cardInfo.EconomicoValue);
            socialText.SetText(cardInfo.SocialValue);
            mediaText.SetText(cardInfo.MidiaticoValue);
            idText.SetText(cardInfo.ID);
        }
        else
        {
            politicalText.SetText("???");
            economicText.SetText("???");
            socialText.SetText("???");
            mediaText.SetText("???");
            idText.SetText("¿S ou R ?");
        }
    }
}