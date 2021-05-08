using System;
using Mirror;
using Mirror.Authenticators;
using Steamworks;
using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private string playerName = "Player";
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private bool useSteam = false;
    [SerializeField] private TextMeshProUGUI addressText;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequest;
    protected Callback<LobbyEnter_t> lobbyEnter;
    
    public static string clientName;
    public static Texture2D clientPfp;
    [SerializeField] private string connectionAddress;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("CurrentPlayerName"))
        {
            //playerName = PlayerPrefs.GetString("CurrentPlayerName");
            //clientName = playerName;
            //nameInput.SetTextWithoutNotify(playerName);
        }
    }

    private void Start()
    {

    }

    public void EnableSteam()
    {
        if (SteamManager.Initialized)
        {
            clientName = SteamFriends.GetPersonaName();
            Debug.Log(clientName);
            
            int iAvatar = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());
            SteamUtils.GetImageSize(iAvatar, out uint imgWidth, out uint imgHeight);
            byte[] data = new byte[imgHeight * imgWidth * 4];
            var isValid = SteamUtils.GetImageRGBA(iAvatar, data, (int)( 4 * imgHeight * imgWidth));
            if (isValid)
            {
                clientPfp = new Texture2D((int)imgWidth, (int)imgHeight, TextureFormat.RGBA32, false, true);
                clientPfp.LoadRawTextureData(data);
                clientPfp.Apply();
        
                //steamAvatar = Sprite.Create(profilePicture, new Rect(0, 0, imgWidth, imgHeight), Vector2.one/2 );
        
                //Destroy(profilePicture);
            }
            else
            {
                Debug.LogError("Could not fetch Steam PFP.");
            }
            
        }
        (NetworkManager.singleton as NetworkManagerCardGame).EnableSteamTransport();
        useSteam = true;
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequest);
        lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void EnableKcp()
    {
        try
        {
            lobbyCreated.Dispose();
            gameLobbyJoinRequest.Dispose();
            lobbyEnter.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        useSteam = false;
        (NetworkManager.singleton as NetworkManagerCardGame).EnableKcpTransport();
    }
    
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) //Means the executing client is the host.
        {
            return;
        }
        else
        {
            string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), 
                "HostAddress");

            NetworkManager.singleton.networkAddress = hostAddress;
            NetworkManager.singleton.StartClient();
        }
    }

    private void OnGameLobbyJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to create lobby!");
            return;
        }
        else
        {
            NetworkManager.singleton.StartHost();
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress",
                SteamUser.GetSteamID().ToString());
            addressText.SetText(SteamUser.GetSteamID().ToString());
        }
    }

    public void UpdatePlayerName (string newName)
    {
        playerName = newName;
        clientName = playerName;
        PlayerPrefs.SetString("CurrentPlayerName", playerName);
        PlayerPrefs.Save();
    }
    
    public void UpdateConnectionAddress (string newID)
    {
        connectionAddress = newID;

        /*playerName = newName;
        clientName = playerName;
        PlayerPrefs.SetString("CurrentPlayerName", playerName);
        PlayerPrefs.Save();*/
    }

    public void Connect()
    {
        NetworkManager.singleton.networkAddress = connectionAddress;
        NetworkManager.singleton.StartClient();
    }
    
    public void Host()
    {
        if (useSteam)
        {
            Debug.Log("USING STEAM");
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
        }
        else
        {
            NetworkManager.singleton.StartHost();
        }
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}