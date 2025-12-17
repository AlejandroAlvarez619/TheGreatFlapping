using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Exit : MonoBehaviour
{
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        Debug.Log("Exit reached! Closing game...");

        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
