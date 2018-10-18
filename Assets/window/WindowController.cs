using UnityEngine;
using System.Collections;

public class WindowController : MonoBehaviour {

    public float MovementSpeed = 5.0f;
    public float TurnSpeed = 180.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {

        float move = MovementSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        transform.Translate(0f, 0f, move);

        float turn = TurnSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        transform.Rotate(new Vector3(0f, turn, 0f));
	}
}
