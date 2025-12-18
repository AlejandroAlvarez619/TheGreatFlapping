using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Exit : MonoBehaviour
{
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        Debug.Log("Exit reached!");
        SceneManager.LoadScene("Win");
    }
}
