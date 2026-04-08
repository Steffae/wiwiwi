using UnityEngine;
using UnityEngine.SceneManagement;

public class DownCollider : MonoBehaviour
{
    private string targetSceneName = "End";
    private string triggerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
