using UnityEngine;

public class Plate : MonoBehaviour
{
    public Animator doorAnimator1;
    public Animator doorAnimator2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator1.SetBool("POpen", true);
            doorAnimator2.SetBool("POpen", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator1.SetBool("POpen", false);
            doorAnimator2.SetBool("POpen", false);
        }
    }
}