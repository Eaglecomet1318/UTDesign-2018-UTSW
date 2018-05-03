/* UTD Spring 2018 
*/

using UnityEngine;
using VolumeViewer;
using UnityEngine.UI;
using VRTK;

/* UTD Semester: Spring 2018
|------------------ public class DICOMMenu-------------------
| This class displays a side panel of different modifications
| that can be used on the DICOM image.
|
*/
public class DICOMMenu : MonoBehaviour
{
    private static DICOMMenu _mInstance;
    public static DICOMMenu mInstance {
        get
        {
            if (!_mInstance)
            {
                _mInstance = Resources.FindObjectsOfTypeAll<DICOMMenu>()[0];

            }
            return _mInstance;
        }
        private set
        {
            _mInstance = value;
        }
    }
    
    /* Sp18
    | cut away extraneous data of image or add back to it slider
    | increase or decrease contrast (shading) slider
    | bring up or bring down image slider
    */  
    public Slider cutValue;  
    public Slider contrast;
    public Slider height;
    
    /* Sp18
    | convert to STL (creates mesh for now) function button
    | button to disable any transfer functions applied
    */  
    public Button convertToSTL;
    public Button disableTF;
    
    /* Sp18
    | Transfer function buttons, each of which
    | apllies 
    */
    public Button redTF;
    public Button darkRedTF;
    public Button orangeTF;
    public Button yellowTF;
    public Button greenTF;
    public Button blueTF;
    public Button purpleTF;
    public VolumeComponent mainVolume;

    public void ToggleMenu()
    {
        Debug.Log("test");
        mInstance.gameObject.SetActive(!mInstance.gameObject.activeSelf);
    }
    
    /* UTD Semester: Spring 2018
    |------------------ void Awake() -------------------
    | This void function activates the various buttons and sliders on the
    | DICOM menu panel.
    || 
     */
    void Awake()
    {
        mInstance = this;
        cutValue.onValueChanged.AddListener(delegate { CutValueChanged(); });
        contrast.onValueChanged.AddListener(delegate { ContrastChanged(); });
        
        convertToSTL.onClick.AddListener(delegate { Debug.Log("Convert to STL button pressed."); ConvertToSTL(); });
        disableTF.onClick.AddListener(delegate { Debug.Log("Disable TF button pressed."); disableTransferFunction(); });
        
        redTF.onClick.AddListener(delegate { Debug.Log("Red TF button pressed."); redTFChanged(); });
        darkRedTF.onClick.AddListener(delegate { Debug.Log("Dark Red TF button pressed."); darkRedTFChanged(); });
        orangeTF.onClick.AddListener(delegate { Debug.Log("Orange TF button pressed."); orangeTFChanged(); });
        yellowTF.onClick.AddListener(delegate { Debug.Log("Yellow TF button pressed."); yellowTFChanged(); });
        greenTF.onClick.AddListener(delegate { Debug.Log("Green TF button pressed."); greenTFChanged(); });
        blueTF.onClick.AddListener(delegate { Debug.Log("Blue TF button pressed."); blueTFChanged(); });
        purpleTF.onClick.AddListener(delegate { Debug.Log("Purple TF button pressed."); purpleTFChanged(); });
        
        mainVolume.tfData = GetComponent<tfDataMode>().list[0]; // Default TF
    }
    
    /* UTD Semester: Spring 2018
    |------------------ void Update() -----------------------
    | This void function allows the sliders and buttons to be 
    | modified (pushed, dragged, selected, etc.) by the VRTK
    | device (in our case, HTC Vive controllers, though Oculus
    | will do just as well).
    | 
     */
    public void Update()
    {
        GameObject left, right;
        bool grabbing = false;
        if((left = VRTK_DeviceFinder.GetControllerLeftHand()) && left.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() != null)
        {
            grabbing = true;
        }
        if ((right = VRTK_DeviceFinder.GetControllerRightHand()) && right.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() != null)
        {
            grabbing = true;
        }
        if(grabbing)
        {
            setAlpha(.5f);
            GetComponent<VRTK_UICanvas>().enabled = false;
        }
        else
        {
            setAlpha(1f);
            GetComponent<VRTK_UICanvas>().enabled = true;
        }
    }

    public void setAlpha(float alpha)
    {
        CanvasRenderer[] children = GetComponentsInChildren<CanvasRenderer>();
        Color newColor;
        foreach (CanvasRenderer child in children)
        {
            newColor = child.GetColor();
            newColor.a = alpha;
            child.SetColor(newColor);
        }
    }

    public void CutValueChanged()
    {
        mainVolume.cutValueRangeMin = cutValue.normalizedValue;
    }

    public void ContrastChanged()
    {
        mainVolume.contrast = contrast.normalizedValue;
    }

    public void HeightChanged()
    {
        mainVolume.transform.position = new Vector3(0, height.normalizedValue + 1, 0);
    }

    public void ConvertToSTL()
    {
        if (!mainVolume.GetComponent<VolumeViewerMarcher>())
        {
            Debug.LogError("Target Volume does not have a VolumeViewerMarcher component.");
            return;
        }
        mainVolume.GetComponent<VolumeViewerMarcher>().ConvertToMesh(mainVolume.cutValueRangeMin);
    }

    /* 
    | All functions to do with transfer functions are accessed in the member function tfDataBlendMode which is set
    | equal to the Disabled constant (not active) of the VolumeBlendMode enumerated list in VolumeUtilities.cs .
    */
    public void disableTransferFunction()
    {
        mainVolume.tfDataBlendMode = VolumeBlendMode.Disabled;
    }



    /*
    | Various transfer functions (rainbow scale)
    */
    public void redTFChanged()
    {
        mainVolume.tfData = GetComponent<tfDataMode>().list[0]; // Appears first in list.

        if (mainVolume.tfDataBlendMode == VolumeBlendMode.Disabled) // if transfer function is not applied,
        {
            mainVolume.tfDataBlendMode = VolumeBlendMode.Multiply;  // activate it.
        }
    }

    public void darkRedTFChanged()
    {
        mainVolume.tfData = GetComponent<tfDataMode>().list[1];  

        if (mainVolume.tfDataBlendMode == VolumeBlendMode.Disabled)
        {
            mainVolume.tfDataBlendMode = VolumeBlendMode.Multiply;
        }
    }

    public void orangeTFChanged()
    {
        mainVolume.tfData = GetComponent<tfDataMode>().list[2];

        if (mainVolume.tfDataBlendMode == VolumeBlendMode.Disabled)
        {
            mainVolume.tfDataBlendMode = VolumeBlendMode.Multiply;
        }
    }
    public void yellowTFChanged()
    {
        mainVolume.tfData = GetComponent<tfDataMode>().list[3];

        if (mainVolume.tfDataBlendMode == VolumeBlendMode.Disabled)
        {
            mainVolume.tfDataBlendMode = VolumeBlendMode.Multiply;
        }
    }

    public void greenTFChanged()
    {
        mainVolume.tfData = GetComponent<tfDataMode>().list[4];

        if (mainVolume.tfDataBlendMode == VolumeBlendMode.Disabled)
        {
            mainVolume.tfDataBlendMode = VolumeBlendMode.Multiply;
        }
    }

    public void blueTFChanged()
    {
        mainVolume.tfData = GetComponent<tfDataMode>().list[5];

        if (mainVolume.tfDataBlendMode == VolumeBlendMode.Disabled)
        {
            mainVolume.tfDataBlendMode = VolumeBlendMode.Multiply;
        }
    }

    public void purpleTFChanged()
    {
        mainVolume.tfData = GetComponent<tfDataMode>().list[6];

        if (mainVolume.tfDataBlendMode == VolumeBlendMode.Disabled)
        {
            mainVolume.tfDataBlendMode = VolumeBlendMode.Multiply;
        }
    }
}
