using System.Collections.Generic;
using UnityEngine;
using VolumeViewer;

public class test : MonoBehaviour {
    public Texture2D noise;
    
	void Start () {
        MakeVolume(
            "Assets/VolumeViewerPro/examples/volumes/UTDesign DICOM/Atrial Switch/Atrial Switch CMR 3D SSFP/img0001-2721.13.dcm",
            noise);
    }

    public static GameObject MakeVolume(string path, Texture2D noise)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        VolumeComponent vc = g.AddComponent<VolumeComponent>();
        vc.rayOffset = noise;
        VolumeRenderer vr = Camera.main.GetComponent<VolumeRenderer>();
        if (!vr)
        {
            Debug.Log("Volume Renderer not detected, adding one to the main camera...");
            vr = Camera.main.gameObject.AddComponent<VolumeRenderer>();
            vr.volumeObjects = new List<VolumeComponent>();
        }
        vr.volumeObjects.Add(vc);
        VolumeFileLoader vfl = g.AddComponent<VolumeFileLoader>();
        vfl.forceDataFormat = VolumeTextureFormat.Alpha8;
        vfl.dataPath = path;
        return g;
    }
}
