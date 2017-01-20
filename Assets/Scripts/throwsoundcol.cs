using UnityEngine;
using System.Collections;

public class throwsoundcol : MonoBehaviour {
	public AudioSource audiosourcewood; 
	public AudioSource audiosourcegrass;

	private bool objectIsClicked;

	//public GameObject fpsGameController;

	void Start(){
		//Physics.IgnoreCollision (GetComponent<Collider> (), fpsGameController.GetComponent<Collider> ());
		objectIsClicked = false;
	}

//	public void Awake(){
//		audiosourcewood = GetComponent<AudioSource> ();
//		audiosourcegrass = GetComponent<AudioSource> ();
//	}

	public void OnCollisionEnter(Collision col){
		//Debug.Log (transform.tag + " Collide With = " + col.gameObject.tag );
		if (col.gameObject.CompareTag ("tower")) {
			audiosourcewood.volume = col.relativeVelocity.magnitude * col.relativeVelocity.magnitude / 20;
			audiosourcewood.Play ();
		}
		if (col.gameObject.CompareTag ("grass")) {
			audiosourcegrass.volume = col.relativeVelocity.magnitude * col.relativeVelocity.magnitude / 10;
			audiosourcegrass.Play ();
		}
	}
    /* Code without HTC Vive controls
	void OnMouseDown ()
	{
		Debug.Log ("CLicked object"); 
		objectIsClicked = true;
		transform.forward = new Vector3 (0.0f, 0.0f, 1.0f);
		transform.Rotate(Vector3.right * 45, Space.World);
		transform.position = fpsGameController.gameObject.transform.position;
		transform.position += new Vector3(0.0f, 0.3f,0.0f);

		GetComponent<Rigidbody>().useGravity = false;
	}

	void Update(){
		if (objectIsClicked && Input.GetMouseButton (0)) {

			float y = transform.position.y;
			float x = fpsGameController.gameObject.transform.position.x;
			float z = fpsGameController.gameObject.transform.position.z;
			transform.position = new Vector3(x, y,z);


			Debug.Log ("transform.position " + transform.position);

			Debug.Log ("fpsGameController.gameObject.rigidbody " + fpsGameController.gameObject.GetComponent<Rigidbody>().velocity);
		}
		if (objectIsClicked && Input.GetMouseButtonUp (0)) {
			Debug.Log ("OBject is false");
			Debug.Log ("hastighet x" + Input.GetAxis ("Mouse X"));
			Debug.Log ("hastighet Y" + Input.GetAxis ("Mouse Y"));
			objectIsClicked = false;
			float armRadius = 0.63f;
			float yVel = Input.GetAxis ("Mouse Y");
			float xVel = Input.GetAxis ("Mouse X");
			float angleRadians = Mathf.Atan2 (xVel,yVel);
			Debug.Log ("Angle" + angleRadians);
			float speed = Mathf.Sqrt (yVel * yVel + xVel * xVel);
			Vector3 forw = new Vector3 (0.0f, 0.0f, 1.0f) ;
			forw = Quaternion.Euler(0, angleRadians * 180.0f / Mathf.PI, 0) * forw;
			Debug.Log ("forw" + forw);
			GetComponent<Rigidbody>().AddForce(forw * speed * 40f);
			GetComponent<Rigidbody>().useGravity = true;

		}
        
	
	}
    */
//	void onCollisionStay(Collision collision){
//		Debug.Log (collision.relativeVelocity.magnitude);
//		if (!audio.isPlaying && collision.relativeVelocity.magnitude >= 2) {
//			
//			audio.volume = collision.relativeVelocity.magnitude;
//			audio.Play ();
//		}
//	}
}
