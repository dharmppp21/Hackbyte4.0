using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionReference interactAction;
    public float interactRadius = 2f;
    public LayerMask interactLayer;

    private RedButtonSpawn currentButton;

    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.Disable();
    }

    private void Update()
    {
        FindNearbyButton();

        if (currentButton != null)
        {
            currentButton.ShowPrompt();

            if (interactAction != null && interactAction.action.WasPressedThisFrame())
            {
                currentButton.Interact();
            }
        }
        else
        {
            if (currentButton != null)
                currentButton.HidePrompt();
        }
    }

    private void FindNearbyButton()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius, interactLayer);

        RedButtonSpawn nearest = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            RedButtonSpawn button = hit.GetComponent<RedButtonSpawn>();
            if (button != null)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearest = button;
                }
            }
        }

        if (currentButton != nearest)
        {
            if (currentButton != null)
                currentButton.HidePrompt();

            currentButton = nearest;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}