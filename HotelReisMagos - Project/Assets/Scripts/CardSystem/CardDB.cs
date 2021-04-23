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
     * 
     *  O dicionario Pai, é uma lista de dicionarios dos tipos de cartas (decks)
     *  Para consultar uma carta, acessamos o deck do tipo dela, e depois procuramos a carta, dentro do dicionario filho.
     *  Uma Matriz 3D.
     *  O dicionario pai recebe uma string, e um outro dicionario.
     *  E o dicionario filho, recebe uma stirng e um CardInfo
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
            throw new ArgumentException("Missing Data in CSV file");
        }        

        /** Calculando a quantidade total de cartas: 
         * 
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
             * 
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
        /** Encontrando uma carta em Deck especifico:
         * 
         * Primeiro recebemos a carta.
         * Depois verificamos se existe um deck com o tipo da carta, dentro do dicionario pai (utilizando o método TryGetValue).
         * Caso não exista, é criado um novo dicionario filho com o tipo de carta mencionada (out deck).
         * Depois de existir o novo deck (ou caso já exista), a carta é adicionada a ele.
         */
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
        /** Encontrando uma carta em Deck especifico:
         * 
         * Primeiro recebemos o tipo da carta.
         * Depois verificamos se esse tipo de carta existe dentro do dicionario pai, utilizando o metodo TryGetValue
         * Caso exista, é feita uma referencia para um dicionario similar ao dicionario filho (out deck).
         * Uma vez tendo a informação do deck em mãos, buscamos a carta dentro do mesmo.
         */
        Dictionary<string, CardInfo> deck;
        if (!decks.TryGetValue(type, out deck))
        {
            throw new ArgumentException("Deck Type " + type + " not found");
        }
        else
        {
            /** Pegando uma Carta aleatório dentro de um deck;
             * 
             * Criamos uma lista de string com todas as Keys do dicionario (as palavras que são usadas como referencia para encontrar a carta [Em caso de duvida, veja como funciona o dicionario])
             * Depois pegamos o total de elementos dentro da lista (ou seja, o total de Keys que foram armazenadas anteriormente) e criamos uma função que gera um valor randomico entre 0 e o total de chaves.
             * Usamos esse valor como indice (Vale salientar que o valor é do tipo inteiro, caso não seja, não serve como indice) para encontrar a Key da carta e busca-la dentro do dicionario.
             * 
             */
            List<string> listKey = new List<string>(deck.Keys);
            int idx = UnityEngine.Random.Range(0, listKey.Count);

            CardInfo randomCard;
            deck.TryGetValue(listKey[idx], out randomCard);            
            return randomCard;
        }
    }

    public void RemoveDeckCard(CardInfo card)
    {
        /** Removendo uma carta em Deck especifico:
         * 
         * Primeiro recebemos a carta.
         * Depois verificamos se o tipo da carta existe dentro do dicionario pai, utilizando o metodo TryGetValue
         * Caso exista, é feita uma referencia para um dicionario similar ao dicionario filho (out deck).
         * Uma vez tendo a informação do deck em mãos, tentamos remover a carta dentro do mesmo.
         */
        Dictionary<string, CardInfo> deck;
        if (!decks.TryGetValue(card.Type, out deck))
        {
            throw new ArgumentException("Deck Type " + card.GetType() + " not found");
        }
        else
        {
            try 
            {    
                deck.Remove(card.ID);   
            }
            catch 
            {  
                throw new ArgumentException("Card ID " + card.ID + " not found");   
            }            
        }
    }
}