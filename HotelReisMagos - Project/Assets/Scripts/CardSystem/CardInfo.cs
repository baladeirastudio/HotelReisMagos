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
    private string name;
    public string Name { get => name; set => name = value; }

    [SerializeField]
    private string classe;
    public string Classe { get => classe; set => classe = value; }
}
