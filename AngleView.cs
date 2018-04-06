using UnityEngine;
using UnityEngine.UI;

public class AngleView : MonoBehaviour {
    
    public Camera viewer;
    public Text target;
	
	void OnGUI () {
        if (!viewer) viewer = Camera.main;
        if (!target) return;
        Vector3 distance = (viewer.transform.position - transform.position).normalized;
        float ax = Mathf.Rad2Deg * Mathf.Acos(distance.x * transform.right.x);
        float ay = Mathf.Rad2Deg * Mathf.Acos(distance.y * transform.up.y);
        float az = Mathf.Rad2Deg * Mathf.Acos(distance.z * transform.forward.z);
        target.text = string.Format("{0} {1} {2}", (int)ax, (int)ay, (int)az);
    }
}
