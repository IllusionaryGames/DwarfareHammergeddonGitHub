using UnityEngine;
using System.Collections;

public class CameraRotation : MonoBehaviour {

	bool ObjectRotating = false;
	bool ObjectMoving = true;

	float speed = 0.5f;

	
	// Update is called once per frame
	void Update () {
	
		if(ObjectRotating)
		transform.Rotate(Vector3.up * Time.deltaTime);

		if(ObjectMoving){
			transform.Translate(Vector3.down * speed * Time.deltaTime);
		}

		if (gameObject.transform.rotation.eulerAngles.y >= 359)
		{
			gameObject.transform.Translate(0,25,0);
			ObjectRotating = false;
			ObjectMoving = true;
			gameObject.transform.eulerAngles = new Vector3(0,0,0);
				}

		if (gameObject.transform.position.y <= 1.247632f) {

			ObjectMoving = false;
			ObjectRotating = true;
				}

		if (gameObject.transform.position.y <= 11.16156) {
			GameObject.Find("Spotlight").light.enabled = false;

		}
		if (gameObject.transform.position.y >= 11.16156) {
			GameObject.Find("Spotlight").light.enabled = true;
			
		}

	}

}