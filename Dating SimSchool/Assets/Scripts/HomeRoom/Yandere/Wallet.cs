using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public GameObject TheWallet;
    public KazukiInHomeRoom GotIt;
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
                    GotIt.HasWallet = true;
                    StartCoroutine("walletgotten");
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
    IEnumerator walletgotten()
    {
        yield return new WaitForSeconds(0.1f);
        TheWallet.SetActive(false);
    }
}
