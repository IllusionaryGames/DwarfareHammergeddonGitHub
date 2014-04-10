using UnityEngine;
using System.Collections;

public class Blockmaterial : MonoBehaviour {

	public Material[] Materials;

	void Awake () {
	
		int RandomTexture = Random.Range(0, 3);

		this.gameObject.renderer.material = Materials[RandomTexture];
	}
}
