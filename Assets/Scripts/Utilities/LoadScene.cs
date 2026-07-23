using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void Load()
    {
        SceneManager.LoadScene(sceneName);
    }
}
