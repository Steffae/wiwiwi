using UnityEngine;
using UnityEngine.SceneManagement;

public class MainToBossSceneMove : MonoBehaviour
{
    private string targetSceneName = "Location_boss";
    private string triggerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
