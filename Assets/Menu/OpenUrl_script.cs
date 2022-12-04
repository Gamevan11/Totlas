using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUrl_script : MonoBehaviour
{
    //������ �� ����
    [SerializeField] string url;
    // ����� �������� URL
    public void OnOpenUrl()
    {
        Application.OpenURL(url);
    }
}