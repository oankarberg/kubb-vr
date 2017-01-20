using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
    public GameObject leftController;
    public GameObject rightController;
    void OnDisable()
    {
        if (rightController != null)
        {
            rightController.GetComponent<SteamVR_FirstPersonController>().TogglePointer(false);
        }
        if (leftController != null)
        {
            leftController.GetComponent<SteamVR_FirstPersonController>().TogglePointer(false);
        }
        
       
    }
}
