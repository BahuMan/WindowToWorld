using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;
using System.Text;


class CameraKinectController : MonoBehaviour
{
    public int debug_bodiestracked = 0;
    public Vector3 kinectOffset;

    private Text debug_panel;

    private KinectSensor _Sensor;
    private BodyFrameReader _Reader;
    private Body[] _Data = null;

    //private CameraToWindowController cam2win;
    public GameObject window;
    private bool calibrating = false;

    void Start()
    {
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor == null)
        {
            Debug.LogError("Kinect not found. Falling back to keyboard control");
            CameraKeyboardController ctrl = GetComponent<CameraKeyboardController>();
            ctrl.enabled = true;
            this.enabled = false;
            return;
        }

        _Reader = _Sensor.BodyFrameSource.OpenReader();

        if (!_Sensor.IsOpen)
        {
            _Sensor.Open();
        }

        //cam2win = GetComponent<CameraToWindowController>();
        //window = cam2win.window;

        debug_bodiestracked = 0;
        debug_panel = GameObject.FindGameObjectWithTag("DebugPanel").GetComponent<Text>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) calibrating = !calibrating;

        if (calibrating)
        {
            calibrateScreen();
        }
        else
        {
            trackMotion();
        }
    }

    private void trackMotion()
    {
        Body closest = getClosestTrackedBody();
        if (closest == null) return;

        CameraSpacePoint headpos = closest.Joints[JointType.Head].Position;
        if (headpos == null)
        {
            debug_panel.text = "headless horseman?";
            return;
        }
        StringBuilder sb = new StringBuilder("tracked: ");
        sb.Append(debug_bodiestracked).AppendLine();
        sb.Append(-headpos.X).AppendLine();
        sb.Append(-headpos.Y).AppendLine();
        sb.Append(-headpos.Z).AppendLine();
        debug_panel.text = sb.ToString();

        transform.localPosition = kinectOffset;
        transform.Translate(headpos.X, headpos.Y, -headpos.Z, window.transform);

    }

    private void calibrateScreen()
    {
        debug_panel.text = "Calibrating...\n";
        Body closest = getClosestTrackedBody();
        if (closest == null) return;

        Vector3 righthand = toVector(closest.Joints[JointType.HandTipRight]);
        Vector3 rightelbow = toVector(closest.Joints[JointType.ElbowRight]);
        Vector3 rightshoulder = toVector(closest.Joints[JointType.ShoulderRight]);

        Vector3 elbowhand = rightelbow - righthand;
        Vector3 shoulderhand = rightshoulder - righthand;
        float armAngle = Vector3.Angle(elbowhand, shoulderhand);
        if ( armAngle > 20f)
        {
            debug_panel.text = debug_panel.text + "Your right arm doesn't seem to be stretched (" + armAngle + ")\n";
        }
        else
        {
            
        }
    }

    //Get the intersection between a line and a plane. 
    //If the line and plane are not parallel, the function outputs true, otherwise false.
    public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
    {

        float length;
        float dotNumerator;
        float dotDenominator;
        Vector3 vector;
        intersection = Vector3.zero;

        //calculate the distance between the linePoint and the line-plane intersection point
        dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
        dotDenominator = Vector3.Dot(lineVec, planeNormal);

        //line and plane are not parallel
        if (dotDenominator != 0.0f)
        {
            length = dotNumerator / dotDenominator;

            //create a vector from the linePoint to the intersection point
            vector = lineVec.normalized * length;

            //get the coordinates of the line-plane intersection point
            intersection = linePoint + vector;

            return true;
        }

        //output not valid
        else
        {
            return false;
        }
    }

    private Vector3 toVector(Windows.Kinect.Joint j)
    {
        return new Vector3(j.Position.X, j.Position.Y, j.Position.Z);
    }

    private Body getClosestTrackedBody()
    {
        var frame = _Reader.AcquireLatestFrame();
        if (frame == null)
        {
            debug_panel.text = "no frame data";
            return null;
        }

        if (_Data == null)
        {
            _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
        }

        frame.GetAndRefreshBodyData(_Data);
        Body result = null;
        debug_bodiestracked = 0;
        foreach (var body in _Data)
        {
            if (body == null)
            {
                continue;
            }

            if (!body.IsTracked)
            {
                continue;
            }

            debug_bodiestracked++;

            if (result == null || body.Joints[JointType.Head].Position.Z < result.Joints[JointType.Head].Position.Z)
            {
                result = body;
            }
        }

        if (result == null)
        {
            debug_panel.text = "nobody found";
        }

        frame.Dispose();
        frame = null;
        return result;
    }

    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}

