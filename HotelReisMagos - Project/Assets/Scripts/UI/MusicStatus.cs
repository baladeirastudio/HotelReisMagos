using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStatus : MonoBehaviour     
{
    public new AudioSource audio;
    public float value;

    private void Start()
    {
        OnChange(true);
    }
   

    public void OnChange(bool val)
    {
        audio.volume = val ? value : 0.0f;
    }
}
