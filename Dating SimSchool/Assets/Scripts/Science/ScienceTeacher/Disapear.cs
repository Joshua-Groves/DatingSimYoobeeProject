using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disapear : MonoBehaviour
{
    //I know the name is spelt wrong just too lazy to fix
    public Soda hayate;
    public GameObject HayateGone;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hayate.Obtainable == true)
        {
            HayateGone.SetActive(false);
        }

    }
}
