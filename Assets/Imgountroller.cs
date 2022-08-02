using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Imgountroller : MonoBehaviour
{
    public Image img;
    public Text txt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(Sprite sp,string str)
    {
        img.sprite = sp;
        txt.text = str;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
