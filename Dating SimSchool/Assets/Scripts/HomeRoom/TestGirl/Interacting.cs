using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

public class Interacting : MonoBehaviour
{
    public GameObject Chat;
    public GameObject ChatReplyNo;
    public GameObject ChatReplyYes;
    public bool Active;
    public bool Ignore;
    public bool Acknowledge;
    public VP_Dialog No;
    public VP_Dialog Yes;
    public RelationLevels Relation;
    public Inter_uses Use;
    public bool minusControl;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (minusControl == true)
        {
            StopCoroutine("minusUse");
        }
        if (No.IsCurrent == true)
        {       
            StartCoroutine("Bad");
        }
        if (Yes.IsCurrent == true)
        {
            StartCoroutine("Good");
        }
        
        if (Active == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Chat.SetActive(true);
                
                
                if (Ignore == true)
                {
                    Chat.SetActive(false);
                    ChatReplyNo.SetActive(true);
                    minusControl = false;
                    StartCoroutine("minusUse");
                }
                if (Acknowledge == true)
                {
                    Chat.SetActive(false);
                    ChatReplyYes.SetActive(true);
                    minusControl = false;

                    StartCoroutine("minusUse");
                }
                if (Chat.activeInHierarchy)
                {
                    StartCoroutine("minusUse");
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

    IEnumerator Bad()
    {
        yield return new WaitForSeconds(2f);
        Use.Homeroom -= 1;
        Ignore = true;
        No.IsCurrent = false;
        
        StopCoroutine("Bad");
    }
    IEnumerator Good()
    {
        yield return new WaitForSeconds(2f);
        Use.Homeroom -= 1;
        Acknowledge = true;
        Relation.TestGirl += 25;
        Yes.IsCurrent = false;
        
        StopCoroutine("Good");
    }
    IEnumerator minusUse()
    {
        yield return new WaitForSeconds(1f);
        Use.Homeroom -= 1;
        yield return new WaitForSeconds(0.5f);
        minusControl = true;
    }
}
