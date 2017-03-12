using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{
	protected void Update()
    {
        // Reload current scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadCurrentScene();
        }
    }

    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(0);
    }
}