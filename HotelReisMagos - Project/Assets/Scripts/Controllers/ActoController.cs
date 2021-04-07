using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActoController : MonoBehaviour
{
    [SerializeField]
    private int ID;

    public void LockSlots()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            child.gameObject.GetComponentInChildren<Button>().interactable = false;
        }
    }

    public void UnlockSlots()
    {        
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            child.gameObject.GetComponentInChildren<Button>().interactable = true;
        }
    }

    public void ResetBoard()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            child.gameObject.GetComponentInChildren<SlotController>().ResetSlot();
        }
    }

    public int GetID()
    {
        return ID;
    }
}
