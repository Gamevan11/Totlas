using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public void OnPause()
    {
        SceneManager.LoadScene(0);
    }
}
