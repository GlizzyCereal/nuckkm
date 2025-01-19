using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    // Reference to the UI GameObject that represents the inventory screen
    public GameObject inventoryScreenUI;
    public bool isOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        isOpen = false;
    }
 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            inventoryScreenUI.SetActive(true);
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            inventoryScreenUI.SetActive(false);
            isOpen = false;
        }
    }
}