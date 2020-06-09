using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vial : MonoBehaviour
{
    public GameObject TheVial;
    public MihoMath GotIt;
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
                    GotIt.HasVial = true;
                    StartCoroutine("Vialgotten");
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
    IEnumerator Vialgotten()
    {
        yield return new WaitForSeconds(0.1f);
        TheVial.SetActive(false);
    }
}
