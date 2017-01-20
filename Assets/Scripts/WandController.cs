using UnityEngine;
using System.Collections;

public class WandController : MonoBehaviour {

	private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
	public bool gripButtonDown = false;
	public bool gripButtonPressed = false;
	public bool gripButtonUp = false;


	private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger; 
	public bool triggerButtonDown = false;
	public bool triggerButtonPressed = false;
	public bool triggerButtonUp = false;

	private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input ((int)trackedObj.index); }}
	private SteamVR_TrackedObject trackedObj;
	// Use this for initialization
	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (controller == null) {
			Debug.Log ("controller not initialized");
			return;
		}
		gripButtonDown = controller.GetPressDown (gripButton);
		gripButtonPressed = controller.GetPress (gripButton);
		gripButtonUp = controller.GetPressUp(gripButton);


		triggerButtonDown = controller.GetPressDown (triggerButton);
		triggerButtonPressed = controller.GetPress (triggerButton);
		triggerButtonUp = controller.GetPressUp(triggerButton);


		if(gripButtonDown){
			Debug.Log ("gripButton was just pressed");
		}
		if (gripButtonUp) {
			Debug.Log ("gripButton was just released");
		}
		if(triggerButtonDown){
			Debug.Log ("triggerButtonDown was just pressed");
		}
		if (triggerButtonUp) {
			Debug.Log ("triggerButtonUp was just released");
		}	

	}
		
}
