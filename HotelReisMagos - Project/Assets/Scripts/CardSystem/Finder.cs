using System;
using UnityEngine;

public class Finder : MonoBehaviour
{    
    public TextAsset textData;
    public CardDB myCardList = new CardDB();

    void Start()
    {
        ReadCSVFile();
    }

    private void ReadCSVFile()
    {
        string[] data = textData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        int tableSize = data.Length / 3 - 1;
        myCardList.card = new CardInfo[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            myCardList.card[i] = new CardInfo();
            myCardList.card[i].Setid((data[3 * (i + 1)]));
            myCardList.card[i].Setname((data[3 * (i + 1) + 1]));
            myCardList.card[i].Setclasse((data[3 * (i + 1) + 2]));
        }
    }
}
