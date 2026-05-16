using UnityEngine;

public class DownCollider : MonoBehaviour
{
    private string triggerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            SceneLoader.LoadEnd();
        }
    }
}