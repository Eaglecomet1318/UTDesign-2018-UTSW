using UnityEngine;
using VolumeViewer;
using UnityEngine.UI;

public class DICOMMenu : MonoBehaviour {

    public static DICOMMenu mInstance { get; private set; }
    public Slider cutValue;
    public Button convertToSTL;
    public VolumeComponent mainVolume;
    
    void Start () {
        mInstance = this;
        cutValue.onValueChanged.AddListener(delegate { CutValueChanged(); });
        convertToSTL.onClick.AddListener(delegate { ConvertToSTL(); });
    }
	
    public void CutValueChanged()
    {
        mainVolume.cutValueRangeMin = cutValue.normalizedValue;
    }

    public void ConvertToSTL()
    {
        if (!mainVolume.GetComponent<VolumeViewerMarcher>())
        {
            Debug.LogWarning("Target Volume does not have a VolumeViewerMarcher component.");
            return;
        }
        mainVolume.GetComponent<VolumeViewerMarcher>().ConvertToMesh(mainVolume.cutValueRangeMin);
    }
}
