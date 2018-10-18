using UnityEngine;
using System.Collections;

public class CameraKeyboardController : MonoBehaviour {

    public float MoveSpeed = 1.0f;
    public float CloserSpeed = 1.0f;

    //private CameraToWindowController cam2win;
    public GameObject window;

    // Use this for initialization
    void Start () {
        //cam2win = GetComponent<CameraToWindowController>();
        //window = cam2win.window;
	}
	
	// Update is called once per frame
	void Update () {
        //@TODO
        //the three lines which calculate a position for the camera need to be replaced
        //by input from a kinect-like device and corrected for the difference in location between monitor and kinect.

        float movex = MoveSpeed * Time.deltaTime * Input.GetAxis("Mouse X");
        float movey = MoveSpeed * Time.deltaTime * Input.GetAxis("Mouse Y");
        float movez = CloserSpeed * Time.deltaTime * Input.GetAxis("Mouse ScrollWheel");

        transform.Translate(movex, movey, movez, window.transform);

    }
}
