using UnityEngine;

public class secondpair1 : MonoBehaviour
{
    public Animator doorAnimator1;
    public Animator doorAnimator2;
    public Animator doorAnimator3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator1.SetBool("Open2", true);
            doorAnimator2.SetBool("Open4", true);
            doorAnimator3.SetBool("Open6", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator1.SetBool("Open2", false);
            doorAnimator2.SetBool("Open4", false);
            doorAnimator3.SetBool("Open6", false);
        }
    }
}