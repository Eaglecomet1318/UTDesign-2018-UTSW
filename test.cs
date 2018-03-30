using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumeViewer;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MakeVolume("Assets/VolumeViewerPro/examples/volumes/UTDesign DICOM/Atrial Switch/Atrial Switch CMR 3D SSFP/img0001-2721.13.dcm");
    }

    public static GameObject MakeVolume(string path)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        VolumeComponent vc = g.AddComponent<VolumeComponent>();
        VolumeRenderer vr = Camera.main.GetComponent<VolumeRenderer>();
        if (!vr)
        {
            Debug.Log("Volume Renderer not detected, adding one to the main camera...");
            vr = Camera.main.gameObject.AddComponent<VolumeRenderer>();
            vr.volumeObjects = new List<VolumeComponent>();
        }
        vr.volumeObjects.Add(vc);
        VolumeFileLoader vfl = g.AddComponent<VolumeFileLoader>();
        vfl.forceDataFormat = vfl.forceOverlayFormat = VolumeTextureFormat.Alpha8;
        vfl.dataPath = path;
        /*     Note: The devs of VolumeViewer are morons and assume _overlayPath can't be null 
         * when they try to compare the internal field to the public property's set value.
         * In the code of VolumeFileLoader, make it so that _overlayPath is initialized to 
         * an empty string. Otherwise, you'll end up with null pointer exceptions.
         */
        vfl.overlayPath = "Assets/VolumeViewerPro/examples/volumes/MRI_Overlay.nii";
        vfl.StartCoroutine(vfl.loadFiles());
        return g;
    }
}
