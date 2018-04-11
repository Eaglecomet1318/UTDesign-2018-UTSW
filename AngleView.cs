using UnityEngine;
using UnityEngine.UI;

public class AngleView : MonoBehaviour {
    
    public Camera viewer;
    public Text target;
	
	void OnGUI () {
        if (!viewer) viewer = Camera.main;
        if (!target) return;
        Vector3 distance = (viewer.transform.position - transform.position).normalized;
        /*float ax = Mathf.Rad2Deg * Mathf.Acos(distance.x * transform.right.x);
        float ay = Mathf.Rad2Deg * Mathf.Acos(distance.y * transform.up.y);
        float az = Mathf.Rad2Deg * Mathf.Acos(distance.z * transform.forward.z);
        target.text = string.Format("{0} {1} {2}", (int)ax, (int)ay, (int)az);*/
        bool left = Direction(transform.forward, distance, transform.up) == -1;
        bool anterior = Direction(transform.right, distance, transform.up) == -1;
        bool caudal = Direction(transform.forward, distance, transform.right) == -1;
        target.text = string.Format("{0}{1}O {2}", 
            left?"L":"R",
            anterior?"A":"P",
            caudal?"caudal":"cranial");
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
