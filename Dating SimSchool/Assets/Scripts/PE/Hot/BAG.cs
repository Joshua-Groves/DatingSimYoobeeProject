using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BAG : MonoBehaviour
{
    public GameObject TheBag;
    public HayatePE2 GotIt;
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
                    GotIt.HasBag = true;
                    StartCoroutine("Baggotten");
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
    IEnumerator Baggotten()
    {
        yield return new WaitForSeconds(0.1f);
        TheBag.SetActive(false);
    }
}
