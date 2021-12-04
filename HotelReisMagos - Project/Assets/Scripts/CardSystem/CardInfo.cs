using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardInfo 
{
    [SerializeField]
    private string type;
    public string Type { get => type; set => type = value; }

    [SerializeField]
    private string id;
    public string ID { get => id; set => id = value; }
    
    [SerializeField]
    private string classe;
    public string Classe { get => classe; set => classe = value; }

    [SerializeField]
    private string name;
    public string Name { get => name; set => name = value; }

    [SerializeField]
    private string description;
    public string Description { get => description; set => description = value; }
    
    [SerializeField]
    private string social;
    public string SocialValue { get => social; set => social = value; }

    [SerializeField]
    private string politico;
    public string PoliticoValue { get => politico; set => politico = value; }

    [SerializeField]
    private string economico;
    public string EconomicoValue { get => economico; set => economico = value; }

    [SerializeField]
    private string midiatico;
    public string MidiaticoValue { get => midiatico; set => midiatico = value; }

    [SerializeField] 
    private AudioClip voiceClip;
    public AudioClip VoiceClip { get => voiceClip; set => voiceClip = value; }
}
