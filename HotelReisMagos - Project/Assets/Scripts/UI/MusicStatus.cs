using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStatus : MonoBehaviour
{
    public void OnChange(bool val)
    {
        AudioListener.volume = val ? 1.0f : 0.0f;
    }
}
