using TMPro;
using UnityEngine;

public class TradeCardInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText, idText;

    public TextMeshProUGUI DescriptionText => descriptionText;

    public TextMeshProUGUI IdText => idText;
}