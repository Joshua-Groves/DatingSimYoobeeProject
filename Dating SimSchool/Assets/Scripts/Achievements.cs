using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements : MonoBehaviour
{
    public GameObject MihoImage;
    public GameObject KazukiImage;
    public GameObject HayateImage;
    public GameObject HideoImage;
    public GameObject MihoImage2;
    public GameObject KazukiImage2;
    public GameObject HayateImage2;
    public GameObject HideoImage2;
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
            MihoImage2.SetActive(false);
            MihoImageReveal.SetActive(false);
        }
        if (Image.Kazuki == true)
        {
            KazukiImage.SetActive(true);
            KazukiImage2.SetActive(false);
            KazukiImageReveal.SetActive(false);
        }
        if (Image.Hideo == true)
        {
            HideoImage.SetActive(true);
            HideoImage2.SetActive(false);
            HideoImageReveal.SetActive(false);
        }
        if (Image.Hayate == true)
        {
            HayateImage.SetActive(true);
            HayateImage2.SetActive(false);
            HayateImageReveal.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
