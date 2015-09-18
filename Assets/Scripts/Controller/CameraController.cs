using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float verticalSpeed;
	public float horizontalSpeed;


	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W)) {
			this.transform.Translate(new Vector3(0, 1,0)*verticalSpeed*Time.deltaTime);
		}
		else if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.S)) {
			this.transform.Translate(new Vector3(0, 1,0)*-verticalSpeed*Time.deltaTime);
		}
		else if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
			this.transform.Translate(new Vector3(1, 0,0)*-horizontalSpeed*Time.deltaTime);
		}
		else if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {
			this.transform.Translate(new Vector3(1, 0,0)*horizontalSpeed*Time.deltaTime);
		}
	}
}
