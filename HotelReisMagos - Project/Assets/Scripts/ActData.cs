using System;
using UnityEngine;

[Serializable]
public class ActData
{
    public TextAsset actBeginText;
    public TextAsset actEndText;
    [Tooltip("Number of rounds the act will last.")]
    public int actDuration = 1; 
}