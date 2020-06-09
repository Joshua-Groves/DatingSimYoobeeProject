﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash4 : MonoBehaviour
{
    public GameObject TheTrash;
    public HayateBreak GotIt;
    public bool Active;
    public bool Obtainable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Obtainable == true)
        {
            if (Active == true)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GotIt.HasTrash4 = true;
                    StartCoroutine("Trashgotten");
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Active = true;
    }
    private void OnTriggerExit(Collider other)
    {
        Active = false;
    }
    IEnumerator Trashgotten()
    {
        yield return new WaitForSeconds(0.1f);
        TheTrash.SetActive(false);
    }


}
