using UnityEngine;

public class Single : MonoBehaviour
{
    public Animator doorAnimator1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator1.SetBool("Open2", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator1.SetBool("Open2", false);
        }
    }
}