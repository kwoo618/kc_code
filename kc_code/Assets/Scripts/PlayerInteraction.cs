using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bank")) GameManager.instance.bankPanel.SetActive(true);
        else if (other.CompareTag("Store")) GameManager.instance.storePanel.SetActive(true);
        else if (other.CompareTag("Academy")) GameManager.instance.academyPanel.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bank") || other.CompareTag("Store") || other.CompareTag("Academy"))
        {
            GameManager.instance.CloseAllPanels();
        }
    }
}