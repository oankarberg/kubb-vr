#pragma strict

var sun : GameObject;
var rayMaterial : Material;
var rayThickness : float = 0.5;
var rayLength : float = 5.0;
var rayCount : int = 1;
var rayRadius : float = 0.5;

private var rayRef : GameObject;

function Start () {
	GenerateSunRays ();
}

function Update () {
	UpdateSunRays ();
}

function GenerateSunRays () {
	rayRef = GameObject.CreatePrimitive(PrimitiveType.Quad);
	rayRef.GetComponent.<Renderer>().material = rayMaterial;
	rayRef.transform.localScale.x = rayThickness;
	rayRef.transform.localScale.y = rayLength;
	rayRef.SetActive(false);
	
	for(var i = 1; i <= rayCount; i++) {
		var ray = Instantiate(rayRef, rayRef.transform.position, rayRef.transform.rotation);
		ray.transform.parent = transform;
		ray.transform.localEulerAngles.x = 90;
		ray.transform.localPosition = Vector3(Random.Range(-rayRadius,rayRadius),Random.Range(-rayRadius,rayRadius),rayLength/2);
		ray.transform.localScale.x *= Random.Range(0.2,1.0);
		ray.name = "(cloned) ray" + i;
		ray.SetActive(true);
	}
}

function UpdateSunRays () {
	transform.rotation.eulerAngles = sun.transform.rotation.eulerAngles;
}