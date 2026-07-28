using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeControl : MonoBehaviour
{
    public GameObject ScrollBar;
    float ScrollPos = 0;
    float [] Pos;
    int posisi = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void next()
    {
        if (posisi < Pos.Length - 1)
        {
            posisi += 1;
            ScrollPos = Pos[posisi];
        }
    }

    public void prev()
    {
        if (posisi > 0)
        {
            posisi -= 1;
            ScrollPos = Pos[posisi];
        }
    }

    // Update is called once per frame
    void Update()
    {
        Pos = new float[transform.childCount];
        float Distance = 1f / (Pos.Length - 1f);
        for (int i = 0; i < Pos.Length; i++)
        {
            Pos[i] = Distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            ScrollPos = ScrollBar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for (int i = 0; i < Pos.Length; i++)
            {
                if (ScrollPos < Pos[i] + (Distance / 2) && ScrollPos > Pos[i] - (Distance / 2))
                {
                    ScrollBar.GetComponent<Scrollbar>().value = Mathf.Lerp(ScrollBar.GetComponent<Scrollbar>().value, Pos[i], 0.15f);
                    posisi = i;
                }
            }
        }
    }
}
