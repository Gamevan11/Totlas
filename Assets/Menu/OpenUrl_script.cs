using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUrl_script : MonoBehaviour
{
    //ссылка на сайт
    [SerializeField] string url;
    // метод открытия URL
    public void OnOpenUrl()
    {
        Application.OpenURL(url);
    }
}