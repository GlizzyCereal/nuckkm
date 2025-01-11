using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InteractableObject : MonoBehaviour
{
    private bool playerInRange;
    public bool PlayerInRange
    {
        get { return playerInRange; }
        set { playerInRange = value; }
    }
    public string itemName;
 
    public string GetItemName()
    {
        return itemName;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            PlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            PlayerInRange = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange && SelectionManager.instance != null && SelectionManager.instance.onTarget)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                        Debug.Log("Interacting with " + itemName);
            #endif
        }
    }
}