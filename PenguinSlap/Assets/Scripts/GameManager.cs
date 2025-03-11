using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject restartSign;

    public void Update()
    {
        // if (restartSign.activeSelf && Input.GetKeyDown(KeyCode.R))
        // {
        //     RestartLevel();
        // }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
