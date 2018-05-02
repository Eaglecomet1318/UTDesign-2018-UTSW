using UnityEngine;
using UnityEngine.UI;

/* Semester: Spring 2018
|------------------ public class AngleView------------------
| This class allows the camera to track the 
|
|
|
*/
public class AngleView : MonoBehaviour {
    
    public Camera viewer;
    public Text target;
    public float label_height = 1f;
    public float label_scale = .025f;

    private void Start()
    {
        if (!viewer) viewer = Camera.main;
        if (!target)
        {
            GameObject g = new GameObject();
            Canvas canvas = g.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            CanvasScaler cs = g.AddComponent<CanvasScaler>();
            cs.scaleFactor = 10.0f;
            cs.dynamicPixelsPerUnit = 10f;
            GraphicRaycaster gr = g.AddComponent<GraphicRaycaster>();
            g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3.0f);
            g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 3.0f);
            
            /*
            | 
            |
            */
            GameObject g2 = new GameObject();
            g2.name = "Text";
            g2.transform.Rotate(0, 180, 0);
            g2.transform.parent = g.transform;
            Text t = g2.AddComponent<Text>();
            target = t;
            g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3.0f);
            g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 3.0f);
            
            /*
            |---------------------
            | Text font, 
            | color, and activation
            |---------------------
            */
            t.alignment = TextAnchor.MiddleCenter;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            t.font = ArialFont;
            t.fontSize = 7;
            t.text = "Test";
            t.enabled = true;
            t.color = Color.yellow;

            g.name = "Text Label";
            bool bWorldPosition = false;

            g.GetComponent<RectTransform>().SetParent(transform, bWorldPosition);
            g.transform.localPosition = new Vector3(0f, label_height, 0f);
            g.transform.localScale = new Vector3(
                                                 1.0f / transform.localScale.x * label_scale,
                                                 1.0f / transform.localScale.y * label_scale,
                                                 1.0f / transform.localScale.z * label_scale);
        }
    }

    /* Semester: Spring 2018
    |-------------void OnGUI()----------------------------------------------
    |           Camera Angle Tracker
    |-----------------------------------------------------------------------
    | This function helps track the position of a camera object and displays 
    | the angle of the DICOM image in the VR environment that is being looked 
    | at from.
    */
    void OnGUI () {
        Vector3 distance = (viewer.transform.position - transform.position).normalized;
        float ax = Mathf.Rad2Deg * Mathf.Acos(distance.x * transform.right.x); // angle x coordinate of position on plane
        float ay = Mathf.Rad2Deg * Mathf.Acos(distance.y * transform.up.y);    // angle y coordinate of position on plane

		if (ax > 90) ax = ax - 90; // if angle is greater than 90, helps viewpoint change
        else ax = 90 - ax;         // else, position of camera is lower than 90 degrees
        if (ay > 90) ay = ay - 90; // if angle is greater than 90, helps viewpoint change
        else ay = 90 - ay;

        bool left = Direction(transform.forward, distance, transform.up) == -1;
        bool anterior = Direction(transform.right, distance, transform.up) == -1;
        bool caudal = Direction(transform.forward, distance, transform.right) == -1;
        target.text = string.Format("{3}\u00B0 {0}{1}O {4}\u00B0 {2}", 
            left?"L":"R",  //position is either left (L) or right (R) based on relation to the angle image is viewed at
            anterior?"A":"P", // position is either anterior (A) or posterior (A) based on ....
            caudal?"caudal":"cranial", // position is either anterior (A) or posterior (A) based on ....
            (int)ax,
            (int)ay);
        target.GetComponentInParent<Canvas>().transform.LookAt(viewer.transform);
    }

    //returns -1 when to the left, 1 to the right, -1 if neither.
    public static float Direction(Vector3 forward, Vector3 target, Vector3 up) {
        Vector3 cross = Vector3.Cross(forward, target);
        float direction = Vector3.Dot(cross, up);
        if (direction > 0.0) {
            return 1;
        } else {
            return -1;
        }
    }
}
