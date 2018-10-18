using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class StereoScopicCamera : MonoBehaviour {


    public Camera leftCamera;
    public Camera rightCamera;

    public float interAxialDistance = 0.05f;

    public GameObject window;
    public float TvWidth = 1.25f;
    public float TvHeight = 0.72f;
    public float farClipPlane = 150f;
    public float nearClipPlane = 1f;

    // Use this for initialization
    void Start () {
        Vector3 offset = new Vector3(interAxialDistance/2f, 0f, 0f);
        leftCamera.transform.localPosition = - offset;
        rightCamera.transform.localPosition = offset;
	
	}

    void LateUpdate()
    {

        Vector3 thispos = transform.position - window.transform.position;
        float left = -(TvWidth / 2f) - thispos.x;
        float right = (TvWidth / 2f) - thispos.x;
        float top = (TvHeight / 2f) - thispos.y;
        float bottom = -(TvHeight / 2f) - thispos.y;

        //Debug.Log("left=" + left + ", right=" + right + ", bottom=" + bottom + ", top=" + top + ", nearClipPlane=" + nearClipPlane + ", farClipPlane=" + farClipPlane);
        Matrix4x4 m = PerspectiveOffCenter(left, right, bottom, top, nearClipPlane, farClipPlane);
        leftCamera.projectionMatrix = m;
        rightCamera.projectionMatrix = m;
    }

    static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        float x = 2.0F * near / (right - left);
        float y = 2.0F * near / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0F * far * near) / (far - near);
        float e = -1.0F;
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;
        return m;
    }

}
