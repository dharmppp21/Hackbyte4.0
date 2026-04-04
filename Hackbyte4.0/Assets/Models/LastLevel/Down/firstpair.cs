using UnityEngine;

public class firstpair : MonoBehaviour
{
    public Animator doorAnimator1;
    public Animator doorAnimator2;
    public Animator doorAnimator3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator1.SetBool("Open1", true);
            doorAnimator2.SetBool("Open3", true);
            doorAnimator3.SetBool("Open5", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator1.SetBool("Open1", false);
            doorAnimator2.SetBool("Open3", false);
            doorAnimator3.SetBool("Open5", false);
        }
    }
}