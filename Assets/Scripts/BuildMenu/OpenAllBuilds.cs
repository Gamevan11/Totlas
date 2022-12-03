using UnityEngine;

public class OpenAllBuilds : MonoBehaviour
{
    public void SetActive()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(transform.GetChild(i).gameObject.activeSelf);
        }
    }
}
