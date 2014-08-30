#pragma strict

function Start () {
	
}

function Update () {

}

function OnGUI () {
	if (Event.current.type == EventType.MouseDown) {
		MenuCameraRails();
	}
}

function MenuCameraRails () {
	for(var i = 0; i < 20; ++i) {
		Camera.main.transform.Translate(Vector3(-0.2, 0.2, 0));
		yield WaitForSeconds(0.001);
	}
}