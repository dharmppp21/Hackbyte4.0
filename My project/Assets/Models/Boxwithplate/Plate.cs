using UnityEngine;

public class Plate : MonoBehaviour
{
    public Animator doorAnimator1;
    public Animator doorAnimator2;

    private void OnTriggerEnter(Collider other)
    {
    

        if (other.CompareTag("Box"))
        {
            doorAnimator1.SetBool("Open", true);
            doorAnimator2.SetBool("Open2", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        

        if (other.CompareTag("Box"))
        {
            doorAnimator1.SetBool("Open", false);
            doorAnimator2.SetBool("Open2", false);
        }
    }
}