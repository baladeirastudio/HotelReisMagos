using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadGame : MonoBehaviour
{
    private IEnumerator Start()
    {
        Destroy(NetworkManager.singleton.gameObject);
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene("Game_Lucena");
        //Game_Lucena
    }
}
