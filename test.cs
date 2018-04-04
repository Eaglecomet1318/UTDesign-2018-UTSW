using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumeViewer;
using System.IO;

public class test : MonoBehaviour {
    
    private static Texture2D _noise;
    public static Texture2D noise { get {
            if (_noise) return _noise;
            _noise = new Texture2D(512, 512);
            _noise.LoadImage(File.ReadAllBytes("Assets/VolumeViewerPro/textures/noise.png"));
            return _noise;
    } }

	// Use this for initialization
	void Start () {
        MakeVolume("Assets/VolumeViewerPro/examples/volumes/UTDesign DICOM/Atrial Switch/Atrial Switch CMR 3D SSFP/img0001-2721.13.dcm");
    }

    public static GameObject MakeVolume(string path)
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
