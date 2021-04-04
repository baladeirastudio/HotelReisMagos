using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameState
{
    OBJECTIVE_CARDS = 0,
    GIVE_CHAR_OBJECTIVES = 1,
    CHOOSE_SLOT = 2,
    GIVE_RESOURCE = 3,
    READ_CARD = 4,
    CHANGE_ACTO = 5,
    USE_LUCKY_CARD = 6,
    DEFEND_IDEA = 7,
}

public class GameController : MonoBehaviour
{
    static public GameController Instance { get; set; }

    private List<Color> playersColors = new List<Color>();
    [SerializeField]
    private ActoController[] actos;

    private void Awake()
    {
        Singleton();        
    }

    private void Start()
    {
        Init();
    }

    private void Singleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }            
        else
        {
            Destroy(gameObject);
        }        
    }

    private void Init()
    {        
        InitColors();
        InitSlots();
    }

    public void RegisterPlayer(PlayerController player)
    {
        DummyServer.Instance.ConnectToSever(player);
    }

    private void InitColors()
    {
        playersColors.Add(new Color(0, 1, 1, 1));
        playersColors.Add(new Color(1, 0, 1, 1));
        playersColors.Add(new Color(1, 1, 0, 1));
        playersColors.Add(new Color(0, 0, 1, 1));
        playersColors.Add(new Color(0, 1, 0, 1));
    }

    public void InitSlots()
    {
        foreach (var item in actos)
        {
            if (item.GetID() <= DummyServer.Instance.GetActo())
            {
                item.ResetBoard();
                item.UnlockSlots();
            }
            else
            {
                item.LockSlots();
            }
        }
    }

    public Color GetPlayerColor(int playerID)
    {
        return playersColors[playerID];
    }

    public int NextActo()
    {
        return DummyServer.Instance.GetActo();
    }
}