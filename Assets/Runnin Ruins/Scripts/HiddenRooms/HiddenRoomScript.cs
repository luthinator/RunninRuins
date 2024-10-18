using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenRoomScript : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject areaCover;

    public bool foundSecret;


    void Start()
    {
        if (foundSecret)
        {
            areaCover.SetActive(false);
        }
        else
        {
            areaCover.SetActive(true);
        }
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foundSecret = true;
            areaCover.SetActive(false);
        }
        Debug.Log(other.tag);
    }
}
