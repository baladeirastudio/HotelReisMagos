using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardInfo 
{
    public string id1;
    public string name1;
    public string classe1;

    public string Getid()
    {
        return id1;
    }

    public void Setid(string value)
    {
        id1 = value;
    }

    public string Getname()
    {
        return name1;
    }

    public void Setname(string value)
    {
        name1 = value;
    }        

    public string Getclasse()
    {
        return classe1;
    }

    public void Setclasse(string value)
    {
        classe1 = value;
    }
}
