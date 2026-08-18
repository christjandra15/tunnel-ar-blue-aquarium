using UnityEngine;
using UnityEngine.UI;

// the script is for display the tunnel info
public class infoUIController : MonoBehaviour
{
    public GameObject info;      // this is the info
    public Button infoButton;    // this is the button to let the tank info pop up

    void Start()
    {
        // info is hidden at start

        if (info != null)
            info.SetActive(false);

       
        if (infoButton != null)
            infoButton.onClick.AddListener(ToggleInfo);
    }

    // Toggle the info panel visibility
    void ToggleInfo()
    {
        if (info != null)
        {
            bool isActive = !info.activeSelf; // invert current state
            info.SetActive(isActive);
        }
    }
}
