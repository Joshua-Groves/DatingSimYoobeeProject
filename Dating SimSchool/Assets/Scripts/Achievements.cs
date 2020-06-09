using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements : MonoBehaviour
{
    public GameObject MihoImage;
    public GameObject KazukiImage;
    public GameObject HayateImage;
    public GameObject HideoImage;
    public GameObject MihoImageReveal;
    public GameObject KazukiImageReveal;
    public GameObject HayateImageReveal;
    public GameObject HideoImageReveal;
    public GlobalControl Image;
    void Start()
    {
        if( Image.Miho == true)
        {
            MihoImage.SetActive(true);
            MihoImageReveal.SetActive(false);
        }
        if (Image.Kazuki == true)
        {
            KazukiImage.SetActive(true);
            KazukiImageReveal.SetActive(false);
        }
        if (Image.Hideo == true)
        {
            HideoImage.SetActive(true);
            HideoImageReveal.SetActive(false);
        }
        if (Image.Hayate == true)
        {
            HayateImage.SetActive(true);
            HayateImageReveal.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
