using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

public class EntranceToPE : MonoBehaviour
{
    public GameObject HomeRoomChat1;
    public GameObject FadeFromBlack;
    // Start is called before the first frame update
    void Start()
    {
        FadeFromBlack.SetActive(true);
        StartCoroutine("TurnOffblack");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        HomeRoomChat1.SetActive(true);
    }

    IEnumerator TurnOffblack()
    {
        yield return new WaitForSeconds(4f);
        FadeFromBlack.SetActive(false);

    }
}
