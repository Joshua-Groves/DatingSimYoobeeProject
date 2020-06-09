using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VirtualPhenix.Dialog;

public class RelationLevels : MonoBehaviour
{
    public float YandereGirl = 0f;
    public float GamerGuy = 0f;
    public float DelinquentGirl = 0f;
    public float HotGuy = 0f;
    public float TestGirl = 0f;
    public GameObject LevelUI;
    public Text TestGirlResult;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            LevelUI.SetActive(true);
            TestGirlResult.text = TestGirl.ToString();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            LevelUI.SetActive(false);
            
        }







    }
}
