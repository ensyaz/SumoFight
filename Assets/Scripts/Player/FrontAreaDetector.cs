using UnityEngine;
using UnityEngine.Events;

public class FrontAreaDetector : MonoBehaviour
{
    public UnityAction<Collider> OnHit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sumo"))
            OnHit?.Invoke(other);   
    }
}
