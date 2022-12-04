using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public static Toggle FpsToggle;

    void Start() 
    {
        FpsToggle = GetComponent<Toggle>();
    }
}
