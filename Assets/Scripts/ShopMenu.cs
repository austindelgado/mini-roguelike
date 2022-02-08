using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
    public GameObject shopUI;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
    }

    public void AbilitySelect(int num)
    {

    }

    public void Play()
    {
        shopUI.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
