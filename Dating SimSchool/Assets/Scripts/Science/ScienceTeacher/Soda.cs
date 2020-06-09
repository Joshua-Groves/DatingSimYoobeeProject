using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soda : MonoBehaviour
{
    public GameObject TheSoda;
    public GetMeaSoda GotIt;
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
                    GotIt.HasSoda = true;
                    StartCoroutine("Sodagotten");
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
    IEnumerator Sodagotten()
    {
        yield return new WaitForSeconds(0.1f);
        TheSoda.SetActive(false);
    }
}
