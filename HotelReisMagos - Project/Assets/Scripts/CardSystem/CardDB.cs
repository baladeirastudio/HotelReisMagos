using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDB : MonoBehaviour
{
    static public CardDB Instance { get => instance; set => instance = value; }
    static private CardDB instance;

    [SerializeField]
    private TextAsset cardsCSV;

    [SerializeField]
    private int totalColumns = 4;

    /** Dicionario de dicionarios:
     *  O dicionario Pai, é uma lista de dicionarios dos tipos de cartas (decks)
     *  Para consultar uma carta, acessamos o deck do tipo dela, e depois procuramos a carta, dentro do dicionario filho.
     *  É uma Matrix 3D.
     */
    private Dictionary<string, Dictionary<string, CardInfo>> decks;

    private string[] data;

    private void Awake()
    {
        decks = new Dictionary<string, Dictionary<string, CardInfo>>();
        ReadCSVFile(cardsCSV);
    }

    void Start()
    {
        Singleton();
    }    

    private void Singleton()
    {
        if (instance)
            Destroy(gameObject);
        instance = this;
    }

    private void ReadCSVFile(TextAsset Data)
    {
        try
        {
            data = Data.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        }
        catch
        {
            throw new ArgumentException("Missing Data in TSV file");
        }
        

        /** Calculando a quantidade total de cartas: 
         *  A quantiadde total é igual a quatidade de linhas, menos a linha de indices. 
         *  Para fazer esse calculo fazemos o número total de itens da tabela, dividido pelo total de colunas.
         *  Como nossa tabela tem uma linha indice, colocamos - 1 para ignorarmos essa linha
         */ 
        int cardsQuantity = (data.Length / totalColumns) - 1;

        CardInfo tempCard;

        for (int i = 0; i < cardsQuantity; i++) 
        {
            tempCard = new CardInfo();
            /** Achando a informação correta:
             *  Cada informação dentro da linha recebe um indice, sendo iniciado em zero, e finalizado no total de colunas - 1.
             *  A próxima linha iniciará com o indices do ultimo elemento da linha anterior + 1.
             *  Como queremos ignorar a primeira linha, temos totalDeColunas * (i + 1), adicionamos o + 1 pois o loop inicia a partir do elemento 0.
             *  Seguimos acrescentando + 1, para percorrer cada coluna.
             */
            tempCard.Type = data[totalColumns * (i + 1)];
            tempCard.ID = data[totalColumns * (i + 1) + 1];
            tempCard.Name = data[totalColumns * (i + 1) + 2];
            tempCard.Classe = data[totalColumns * (i + 1) + 3];

            AddCard(tempCard);

        }
    }

    private void AddCard(CardInfo card)
    {
        Dictionary<string, CardInfo> deck;

        if (!decks.TryGetValue(card.Type, out deck))
        {
            deck = new Dictionary<string, CardInfo>();
            decks.Add(card.Type, deck);
        }

        deck.Add(card.ID,card);
    }

    public CardInfo GetRandomCard(string type)
    {
        Dictionary<string, CardInfo> deck;
        if (!decks.TryGetValue(type, out deck))
        {
            throw new ArgumentException("Deck Type " + type + " not found");
        }
        else
        {
            List<string> listKey = new List<string>(deck.Keys);
            int idx = UnityEngine.Random.Range(0, listKey.Count);

            CardInfo randomCard;
            deck.TryGetValue(listKey[idx], out randomCard);            
            return randomCard;
        }
    }

    public void RemoveDeckCard(CardInfo card)
    {
        Dictionary<string, CardInfo> deck;
        if (!decks.TryGetValue(card.Type, out deck))
        {
            throw new ArgumentException("Deck Type " + card.GetType() + " not found");
        }
        else
        {
            deck.Remove(card.ID);
        }
    }
}

