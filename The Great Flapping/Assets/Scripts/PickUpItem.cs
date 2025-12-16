using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public float spinSpeed = 120f;
    public string playerTag = "Player";
    public bool destroyOnPickup = true;

    private void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other) 
   {
    if (!other.CompareTag(playerTag)) return;

    if (destroyOnPickup)
        Destroy(gameObject);
   }

}