
using UnityEngine;
using System.Collections;

public class SteamVR_FirstPersonController : MonoBehaviour
{
	public enum AxisType
	{
		XAxis,
		ZAxis
	}
    //length is how long the vibration should go for
    //strength is vibration strength from 0-1
    IEnumerator LongVibration(float length, float strength, SteamVR_Controller.Device device)
    {
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            device.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
             yield return null;
        }
    }

   
    public Color pointerColor;
	public float pointerThickness = 0.002f;
	public AxisType pointerFacingAxis = AxisType.ZAxis;
	public float pointerLength = 100f;
	public bool showPointerTip = true;
	public bool teleportWithPointer = true;
	public float blinkTransitionSpeed = 0.6f;
    public bool visibleController = true;

    public GameObject mainMenu;
    public Material grabOutlineMaterial;

    public bool highlightGrabbableObject = true;
	public Color grabObjectHightlightColor;

    public GameObject armObject;

    private bool controllerIsHidden = false;

	private SteamVR_TrackedObject trackedController;
	private SteamVR_Controller.Device device;


	private GameObject pointerHolder;
	private GameObject pointer;
	private GameObject pointerTip;

	private Vector3 pointerTipScale = new Vector3(0.05f, 0.05f, 0.05f);

    public GameObject respawnSelectedObjects;
	private float pointerContactDistance = 0f;
	private Transform pointerContactTarget = null;

	private Rigidbody controllerAttachPoint;
	private FixedJoint controllerAttachJoint;
	private GameObject canGrabObject;
	private Color[] canGrabObjectOriginalColors;
    private Material[] canGrabObjectOriginalMaterials;
    private GameObject previousGrabbedObject;



	private Transform HeadsetCameraRig;
	private float HeadsetCameraRigInitialYPosition;
	private Vector3 TeleportLocation;

	void Awake()
	{
		trackedController = GetComponent<SteamVR_TrackedObject>();
        
    }

	void Start()
	{
		InitController();
		InitPointer();
        InitHeadsetReferencePoint();
        
    }

	void InitController()
	{
		controllerAttachPoint = transform.GetChild(0).Find("tip").GetChild(0).GetComponent<Rigidbody>();

		BoxCollider collider = this.gameObject.AddComponent<BoxCollider>();
		collider.size = new Vector3(0.1f, 0.1f, 0.2f);
		collider.isTrigger = true;

        
        
	}

	void InitPointer()
	{
		Material newMaterial = new Material(Shader.Find("Unlit/Color"));
		newMaterial.SetColor("_Color", pointerColor);

		pointerHolder = new GameObject();
		pointerHolder.transform.parent = this.transform;
		pointerHolder.transform.localPosition = Vector3.zero;

		pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
		pointer.transform.parent = pointerHolder.transform;
		pointer.GetComponent<MeshRenderer>().material = newMaterial;

		pointer.GetComponent<BoxCollider>().isTrigger = true;
		pointer.AddComponent<Rigidbody>().isKinematic = true;
		pointer.layer = 2;

		pointerTip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		pointerTip.transform.parent = pointerHolder.transform;
		pointerTip.GetComponent<MeshRenderer>().material = newMaterial;
		pointerTip.transform.localScale = pointerTipScale;

		pointerTip.GetComponent<SphereCollider>().isTrigger = true;
		pointerTip.AddComponent<Rigidbody>().isKinematic = true;
		pointerTip.layer = 2;

		SetPointerTransform(pointerLength, pointerThickness);
        //Set pointer from beginning if menu is present
        if (mainMenu != null)
        {
            TogglePointer(mainMenu.activeSelf);
        }
     
		
	}

 
	void InitHeadsetReferencePoint()
	{
		Transform eyeCamera = GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
		// The referece point for the camera is two levels up from the SteamVR_Camera
		HeadsetCameraRig = eyeCamera.parent;
		HeadsetCameraRigInitialYPosition = HeadsetCameraRig.transform.position.y;
	}

	void SetPointerTransform(float setLength, float setThicknes)
	{
		//if the additional decimal isn't added then the beam position glitches
		float beamPosition = setLength / (2 + 0.00001f);

		if (pointerFacingAxis == AxisType.XAxis)
		{
			pointer.transform.localScale = new Vector3(setLength, setThicknes, setThicknes);
			pointer.transform.localPosition = new Vector3(beamPosition, 0f, 0f);
			pointerTip.transform.localPosition = new Vector3(setLength - (pointerTip.transform.localScale.x / 2), 0f, 0f);
		}
		else
		{
			pointer.transform.localScale = new Vector3(setThicknes, setThicknes, setLength);
			pointer.transform.localPosition = new Vector3(0f, 0f, beamPosition);
			pointerTip.transform.localPosition = new Vector3(0f, 0f, setLength - (pointerTip.transform.localScale.z / 2));
		}

		TeleportLocation = pointerTip.transform.position;
	}

	float GetPointerBeamLength(bool hasRayHit, RaycastHit collidedWith)
	{
		float actualLength = pointerLength;

		//reset if beam not hitting or hitting new target
		if (!hasRayHit || (pointerContactTarget && pointerContactTarget != collidedWith.transform))
		{
			pointerContactDistance = 0f;
			pointerContactTarget = null;
		}

		//check if beam has hit a new target
		if (hasRayHit)
		{
			if (collidedWith.distance <= 0)
			{

			}
			pointerContactDistance = collidedWith.distance;
			pointerContactTarget = collidedWith.transform;

		}

		//adjust beam length if something is blocking it
		if (hasRayHit && pointerContactDistance < pointerLength)
		{
			actualLength = pointerContactDistance;
		}

		return actualLength; 
	}
    void HideMenu()
    {
        mainMenu.SetActive(false);
    }
    float SetUIPointerHit(bool hasRayHit, RaycastHit collidedWith)
    {

        float actualLength = pointerLength;

        //reset if beam not hitting or hitting new target
        if (!hasRayHit || (pointerContactTarget && pointerContactTarget != collidedWith.transform))
        {
            pointerContactDistance = 0f;
            pointerContactTarget = null;
        }

        //check if beam has hit a new target
        if (hasRayHit)
        {
            if (collidedWith.distance <= 0)
            {

            }
            pointerContactDistance = collidedWith.distance;
            pointerContactTarget = collidedWith.transform;
            if (collidedWith.transform.name.Equals("GotItButton"))
            {
                var colors = collidedWith.transform.GetComponent<UnityEngine.UI.Button>().colors;
                colors.normalColor = new Vector4(0.0f /255.0f, 152.0f / 255.0f, 242.0f / 255.0f, 1.0f);
                collidedWith.transform.GetComponent<UnityEngine.UI.Button>().colors = colors;
                if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                    HideMenu();
                }
            }
            else
            {
                var colors = GameObject.Find("GotItButton").GetComponent<UnityEngine.UI.Button>().colors;
                colors.normalColor = new Vector4(79.0f / 255.0f, 135.0f / 255.0f, 169.0f / 255.0f, 1.0f);
                GameObject.Find("GotItButton").GetComponent<UnityEngine.UI.Button>().colors = colors;
            }
        }

        //adjust beam length if something is blocking it
        if (hasRayHit && pointerContactDistance < pointerLength)
        {
            actualLength = pointerContactDistance;
        }

        
    
        return actualLength;
    }

	public void TogglePointer(bool state)
	{
		pointer.gameObject.SetActive(state);
		bool tipState = (showPointerTip ? state : false);
		pointerTip.gameObject.SetActive(tipState);
	}

	void Teleport()
	{
		SteamVR_Fade.Start(Color.black, 0);
		SteamVR_Fade.Start(Color.clear, blinkTransitionSpeed);
        float posY = Terrain.activeTerrain.SampleHeight(transform.position);
        HeadsetCameraRig.position = new Vector3(TeleportLocation.x, posY, TeleportLocation.z);
        
    }

	void UpdatePointer()
	{
		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
		{
			TogglePointer(true);
		}

		if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
		{
			if (pointerContactTarget != null && teleportWithPointer)
			{
				Teleport();
			}
			TogglePointer(false);
		}

		if (pointer.gameObject.activeSelf)
		{
			Ray pointerRaycast = new Ray(transform.position, transform.forward);
			RaycastHit pointerCollidedWith;
			bool rayHit = Physics.Raycast(pointerRaycast, out pointerCollidedWith);
			float pointerBeamLength = GetPointerBeamLength(rayHit, pointerCollidedWith);
			SetPointerTransform(pointerBeamLength, pointerThickness);
		}

       
    }

    void UpdatePointerUIMenu()
    {
        if (pointer.gameObject.activeSelf)
        {
            Ray pointerRaycast = new Ray(transform.position, transform.forward);
            RaycastHit pointerCollidedWith;
            bool rayHit = Physics.Raycast(pointerRaycast, out pointerCollidedWith);
            float pointerBeamLength = SetUIPointerHit(rayHit, pointerCollidedWith);
            SetPointerTransform(pointerBeamLength, pointerThickness);
        }
    }

	void SnapCanGrabObjectToController(GameObject obj)
	{
        //Disable boxcollider on arm if exist
        if(armObject != null)
        {
            armObject.GetComponent<Collider>().enabled = false;
        }

        if(obj.tag == "tower")
        {
            //obj.transform.Rotate(controllerAttachPoint.transform.right);
            obj.transform.position = controllerAttachPoint.transform.position;
            obj.transform.right = controllerAttachPoint.transform.right;
            obj.transform.up = controllerAttachPoint.transform.forward;
        }
        
        controllerAttachJoint = obj.AddComponent<FixedJoint>();
		controllerAttachJoint.connectedBody = controllerAttachPoint;

       
        controllerAttachJoint.transform.up = controllerAttachPoint.transform.forward;
        


        ToggleGrabbableObjectHighlight(false);
	}

	Rigidbody ReleaseGrabbedObjectFromController()
	{
		var jointGameObject = controllerAttachJoint.gameObject;
		var rigidbody = jointGameObject.GetComponent<Rigidbody>();
		Object.DestroyImmediate(controllerAttachJoint);
		controllerAttachJoint = null;

        //Disable boxcollider on arm if exist
        if (armObject != null)
        {
            armObject.GetComponent<Collider>().enabled = true;
        }

        return rigidbody;
	}

	void ThrowReleasedObject(Rigidbody rb)
	{
		var origin = trackedController.origin ? trackedController.origin : trackedController.transform.parent;
		if (origin != null)
		{
			rb.velocity = origin.TransformVector(device.velocity * 1.2f);
			rb.angularVelocity = origin.TransformVector(device.angularVelocity * 1.2f);
		}
		else
		{
			rb.velocity = device.velocity * 1.2f;
			rb.angularVelocity = device.angularVelocity * 1.2f;
		}

		rb.maxAngularVelocity = rb.angularVelocity.magnitude;
	}

	void RecallPreviousGrabbedObject()
	{
		if (previousGrabbedObject != null && device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
		{
			previousGrabbedObject.transform.position = controllerAttachPoint.transform.position;
           
            var rb = previousGrabbedObject.GetComponent<Rigidbody>();
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.maxAngularVelocity = 0f;
		}
	}
    void RecallAllSelectedObjects()
    {
        if (respawnSelectedObjects !=null && device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {

            for (var i = 0; i < respawnSelectedObjects.transform.childCount; i++)
            {
                Vector3 controllerPos = controllerAttachPoint.transform.position;
                GameObject child = respawnSelectedObjects.transform.GetChild(i).gameObject;
                child.transform.position =  new Vector3(controllerPos.x + i*0.1f, controllerPos.y, controllerPos.z);
                var rb = child.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.maxAngularVelocity = 0f;
            }
       

           
        }
    }
    void HideViveModelRender()
    {
        Debug.Log("Hiding Controller");
        foreach (SteamVR_RenderModel model in trackedController.GetComponentsInChildren<SteamVR_RenderModel>())
        {
        
            foreach (var child in model.GetComponentsInChildren<MeshRenderer>())
            {
                child.enabled = false;
                controllerIsHidden = true;
               
            }
                
        }
    }
    void ToggleViveModelRender(bool toggle)
    {
        if (visibleController)
        {
            foreach (SteamVR_RenderModel model in trackedController.GetComponentsInChildren<SteamVR_RenderModel>())
            {
                foreach (var child in model.GetComponentsInChildren<MeshRenderer>())
                {
                    child.enabled = toggle;
                    controllerIsHidden = toggle;
                }
                    
            }
        }
        else
        {
            Debug.LogWarning("The controller model can't toggle because it is set to visibleController = false");
        }
        
    }

	void UpdateGrabbableObjects()
	{

       
        if (canGrabObject != null)
		{
            
            if (controllerAttachJoint == null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
			{
                //Dont render vive model
                ToggleViveModelRender(false);
                StartCoroutine(LongVibration(0.05f, 1, device));
                //Debug.Log("Holding");
                previousGrabbedObject = canGrabObject;
				SnapCanGrabObjectToController(canGrabObject);
			}
			else if (controllerAttachJoint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
			{
                    //start render vivemodel
                ToggleViveModelRender(true);
                //Debug.Log("RELEASING");
                StartCoroutine(LongVibration(0.05f, 1, device));
                Rigidbody releasedObjectRigidBody = ReleaseGrabbedObjectFromController();
				ThrowReleasedObject(releasedObjectRigidBody);
			}
		}
	}

	Renderer[] GetObjectRendererArray(GameObject obj)
	{
		return (obj.GetComponents<Renderer>().Length > 0 ? obj.GetComponents<Renderer>() : obj.GetComponentsInChildren<Renderer>());
	}

	Color[] BuildObjectColorArray(GameObject obj, Color defaultColor)
	{
		Renderer[] rendererArray = GetObjectRendererArray(obj);

		int length = rendererArray.Length;

		Color[] colors = new Color[length];
		for (int i = 0; i < length; i++)
		{
			colors[i] = defaultColor;
		}
		return colors;
	}
    Material[] BuildMaterialArray(GameObject obj, Material defaultMaterial)
    {
        Renderer renderer;

        renderer = obj.GetComponent<Renderer>();
        int length = canGrabObjectOriginalMaterials.Length;
        int i = 0;
        Material[] materials = new Material[length + 1];
        
        foreach (Material mat in canGrabObjectOriginalMaterials)
        {
            //If we already have the default material on the object
           // Debug.Log("grabOutlineMaterial.name " + mat.name); 
            if(grabOutlineMaterial.name != mat.name)
            {
                materials[i] = mat;
            }
            else
            {

                return canGrabObjectOriginalMaterials;
            }
            
        }
        materials[length] = defaultMaterial;

        return materials;
    }


    Color[] StoreObjectOriginalColors(GameObject obj)
	{
		Renderer[] rendererArray = GetObjectRendererArray(obj);

		int length = rendererArray.Length;
		Color[] colors = new Color[length];

		for (int i = 0; i < length; i++)
		{
			var renderer = rendererArray[i];
            if(renderer.material.HasProperty("_Color"))
            {
                colors[i] = renderer.material.color;
            }
			    
		}

		return colors;
	}
    Material[] StoreObjectOriginalMaterials(GameObject obj)
    {
        Renderer renderer;

        renderer = obj.GetComponent<Renderer>();
        int length = renderer.materials.Length;
        int i = 0;
        Material[] materials = new Material[length];
        
        foreach (Material mat in renderer.materials)
        {
            //Initialize without the outline material attached to an object
            if(mat.name != grabOutlineMaterial.name)
            {
                materials[i] = mat;
                i++;
            }

            
        }
        
        
        return materials;
    }

    void ChangeObjectColor(GameObject obj, Color[] colors)
	{
		Renderer[] rendererArray = GetObjectRendererArray(obj);
		int i = 0;
		foreach (Renderer renderer in rendererArray)
		{
			renderer.material.color = colors[i];
			i++;
		}
	}
    void ChangeObjectMaterial(GameObject obj, Material[] newMat)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if(renderer != null)
        {
            

            //Material[] newMatArray;
            renderer.materials = newMat;
        }
       

    }
    void RemoveHighlightObjectMaterial(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            int newSize = canGrabObjectOriginalMaterials.Length - 1;
            Material[] newMat = new Material[newSize];
            int i = 0;
            bool containsOld = false;
            foreach (Material mat in canGrabObjectOriginalMaterials)
            {
                
                if (mat == grabOutlineMaterial)
                {
                    containsOld = true;
                }else if(newSize > i)
                {
                    newMat[i] = mat;
                    i++;
                }
                
            }

            if (containsOld)
            {
                renderer.materials = newMat;
            }
            else
            {
                renderer.materials = canGrabObjectOriginalMaterials;
            }
            //Material[] newMatArray;
            
        }


    }

    void ToggleGrabbableObjectHighlight(bool highlightObject)
	{
      
        //GrabbableWithoutHighlight for objects with complex material properties
		if (highlightGrabbableObject && canGrabObject != null && canGrabObject.tag != "GrabbableWithoutHighlight")
		{            
			if (highlightObject)
			{
				//var colorArray = BuildObjectColorArray(canGrabObject, grabObjectHightlightColor);
                var materialArray = BuildMaterialArray(canGrabObject, grabOutlineMaterial);
                //ChangeObjectColor(canGrabObject, colorArray);
                ChangeObjectMaterial(canGrabObject, materialArray);

            }
			else
			{
                RemoveHighlightObjectMaterial(canGrabObject);
                //ChangeObjectColor(canGrabObject, canGrabObjectOriginalColors);
            }
		}
	}

	void FixedUpdate()
	{
		device = SteamVR_Controller.Input((int)trackedController.index);

        //RecallPreviousGrabbedObject();
        if (!mainMenu.activeSelf)
        {
            UpdateGrabbableObjects();
            RecallAllSelectedObjects();
        }
		
	}

	void Update()
	{

        if (!mainMenu.activeSelf)
        {
            UpdatePointer();
        }
        else
        {
            UpdatePointerUIMenu();
        }
		

       
       
        //Hide controller models
        if (!visibleController && !controllerIsHidden)
        {
            HideViveModelRender();
        }

    }

	void OnTriggerEnter(Collider collider)
	{
        if (controllerAttachJoint == null)
        {
            if (collider.tag == "Grabbable" || collider.tag == "tower")
            {
               
                if (canGrabObject == null)
                {
                    canGrabObjectOriginalColors = StoreObjectOriginalColors(collider.gameObject);
                    canGrabObjectOriginalMaterials = StoreObjectOriginalMaterials(collider.gameObject);
                    canGrabObject = collider.gameObject;
                    ToggleGrabbableObjectHighlight(true);
                }

                
            }else if(collider.tag == "GrabbableWithoutHighlight")
            {
                canGrabObject = collider.gameObject;
            }
            
        }
		
    }

    void OnTriggerExit(Collider collider)
    {
        //this is so we don't assign cangrabobject to null when we are grabbing an object
        if (controllerAttachJoint == null)
        {

            if (collider.tag == "Grabbable" || collider.tag == "tower")
            {

                ToggleGrabbableObjectHighlight(false);
                canGrabObject = null;
            } else if (collider.tag == "GrabbableWithoutHighlight")
            {
                canGrabObject = null;
            }
        }
           
      
    }
}