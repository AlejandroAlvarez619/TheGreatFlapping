using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score Instance;

    public int currentScore = 0;
    public GameObject exit;

    private bool exitActivated = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            
            DontDestroyOnLoad(gameObject);
            return;
        }
        else
        {
            Instance = this;
             DontDestroyOnLoad(gameObject);

        }
    }
    
    public void AddScore(int amount)
    {
        currentScore += amount;

        if(!exitActivated && exit != null)
        {
            exit.SetActive(true);
            exitActivated = true;
        }
    }
}
