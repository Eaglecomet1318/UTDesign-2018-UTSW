using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;   //Used to read files in the directory path
using Parabox.STL; //Used to use the pb_Stl_Importer
using VRTK;
using VolumeViewer;

/* STLImport : UTD Fall 2017
 * Description: Handles importing of STL files into the scene.
 * 
 */ 
public class STLImport : MonoBehaviour  {

    [Header("Coordinates and Scaling", order = 1)]

    [Tooltip("Scale that the models will be scaled to.")]
	public float scaleFactor; //Public scale factor for scaling the models down to desired size.
    [Tooltip("X Coordinate to spawn the STL models at.")]
    public float spawnX;      //Initial spawn x coordinate of object.
    [Tooltip("Y Coordinate to spawn the STL models at.")]
    public float spawnY;      //Initial spawn y coordinate of object.
    [Tooltip("Z Coordinate to spawn the STL models at.")]
    public float spawnZ;      //Initial spawn z coordinate of object.

    private List<GameObject> objectsToGroup = new List<GameObject>();

    private string loadFolder;

    //The following two objects are for the default DICOM volume look. May be null if desired. UTD2018
    public Texture2D noise;
    public Material loading;


    // Use this for initialization
    void Start () {
        loadFolder = Application.dataPath + "/STL Load Folder";
        if(!Directory.Exists(loadFolder))
        {
            Directory.CreateDirectory(loadFolder);
        }
	}

    /* waitForFileInput : UTD Fall 2017
     * Input: None
     * Output: Waits for the user to select a file and excutes an import on the selected file.
     */ 
    IEnumerator waitForFileInput()
    {
        yield return new WaitUntil(() => GameObject.Find("Scene_Models").GetComponent<Scene_Mode>().currentSceneMode != 3);
        importFolder(GameObject.Find("File Menu").GetComponent<FileMenuManager>().selectedPath);
    }

    /* importSTLFiles : UTD Fall 2017
     * Input: None
     * Output: Clears the objects from the last import and fetches new files.
     * 
     */ 
    public void importSTLFiles()
    {
        objectsToGroup.Clear();
		fetchNewModels ();          //Search resourcePath for new files that haven't been imported yet.
    }
		
    /* fetchNewModels : UTD Fall 2017
     * Input: None
     * Output: Starts the file browser and waits for a response.
     */ 
	public void fetchNewModels() //Search resoucePath for new STL and Prefab files.
	{
        GameObject.Find("Scene_Models").GetComponent<Scene_Mode>().startFileMenu(loadFolder);
        StartCoroutine(waitForFileInput());
    }

    /* importFolder : UTD Fall 2017
     * Input: string directory
     * Output: Grabs the STL files from the selected folder and imports them into the scene. After importing the objects it groups them together under a single object.
     * 
     */ 
    private void importFolder(string directory)
    {
        if (directory != "")
        {
            DirectoryInfo dir = new DirectoryInfo(directory); //Obtain directory info of the file at resourcePath.
            FileInfo[] fileInfo = dir.GetFiles("*.stl*", SearchOption.AllDirectories); //Get the file Info of every file in the directory.
            if(fileInfo.Length < 1) //UTD2018
            {
                fileInfo = dir.GetFiles("*.dcm*", SearchOption.AllDirectories);
            }
            foreach (FileInfo f in fileInfo) //Iterate over all files.
            {
                if (f.Extension == ".stl") //If file is a STL file.
                {
                        createSTLObject(f);
                }
                
                if (f.Extension == ".dcm") //If file is a DCM file. UTD2018
                {
                    createDCMObject(f);
                    break;
                }
            }

            if (objectsToGroup.Count > 0)
            {
                GameObject Scene_Models = GameObject.Find("Scene_Models");
                GameObject modelGroup = new GameObject();
                modelGroup.transform.position = new Vector3(spawnX, spawnY, spawnZ);
                modelGroup.transform.SetParent(Scene_Models.transform, false);
                modelGroup.name = "Group";
                modelGroup.AddComponent<Rigidbody>().isKinematic = true;
                modelGroup.AddComponent<InteractiveMedicalObject>();
                modelGroup.GetComponent<InteractiveMedicalObject>().isGrabbable = true;
                modelGroup.GetComponent<InteractiveMedicalObject>().validDrop = InteractiveMedicalObject.ValidDropTypes.DropAnywhere;
                modelGroup.GetComponent<InteractiveMedicalObject>().holdButtonToGrab = true;
                modelGroup.GetComponent<InteractiveMedicalObject>().touchHighlightColor = Color.cyan;

                foreach (GameObject g in objectsToGroup)
                {
                    g.transform.SetParent(modelGroup.transform, false);
                }

            }
        }
    }

    /* createSTLObject : UTD Fall 2017
     * Input: FileInfo f
     * Output: Creates a new GameObject using the meshes from the specified file.
     */ 
    private void createSTLObject(FileInfo f)
    {
        Mesh[] STLMeshes = pb_Stl_Importer.Import(f.FullName); //Import the STL file as an array of meshes.
        GameObject STLGameObject = new GameObject(); //Create new gameObject that will be the model.
        STLGameObject.name = Path.GetFileNameWithoutExtension(f.Name); //Set the GameObjects name as the file name.

        for (int i = 0; i < STLMeshes.Length; i++) //Iterate over the meshes
        {
            GameObject child = new GameObject(); //Create a child GameObject.
            child.name = Path.GetFileNameWithoutExtension(f.Name) + " Part " + (i + 1); //Set the name of the child GameObject.
            child.transform.parent = STLGameObject.transform; //Set the child GameObjects parent as STLGameObject.

            MeshFilter mf = child.AddComponent<MeshFilter>(); //Add a MeshFilter to the child GameObject.
            mf.sharedMesh = STLMeshes[i]; //Set the MeshFilters mesh to the STLMesh.

            MeshRenderer mr = child.AddComponent<MeshRenderer>(); //Add MeshRenderer to the child GameObject.
            mr.material = new Material(Shader.Find("Transparent/Diffuse")); //Set the material of the MeshRenderer to diffuse.
            mr.material.color = new Color(1f, 1f, 1f, 0.4f);

            child.AddComponent<MeshCollider>();
            child.AddComponent<Rigidbody>().isKinematic = true;

            child.AddComponent<CutShader>().enabled = false; //Used for cut planes.
        }

        STLGameObject.AddComponent<Rigidbody>();
        STLGameObject.GetComponent<Rigidbody>().isKinematic = true;
        STLGameObject.GetComponent<Rigidbody>().useGravity = false;

        STLGameObject.AddComponent<InteractiveMedicalObject>();
        STLGameObject.GetComponent<InteractiveMedicalObject>().isGrabbable = true;
        STLGameObject.GetComponent<InteractiveMedicalObject>().validDrop = InteractiveMedicalObject.ValidDropTypes.DropAnywhere;
        STLGameObject.GetComponent<InteractiveMedicalObject>().holdButtonToGrab = true;
        STLGameObject.GetComponent<InteractiveMedicalObject>().touchHighlightColor = Color.cyan;

        objectsToGroup.Add(STLGameObject);
    }

    /* createDCMObject : UTD Spring 2018 UTD2018
     * Input: FileInfo f
     * Output: Creates a new GameObject using the volume from the specified file.
     */
    private void createDCMObject(FileInfo f)
    {
        GameObject g = MakeVolume(f.FullName, noise, loading);

        g.AddComponent<Rigidbody>();
        g.GetComponent<Rigidbody>().isKinematic = true;
        g.GetComponent<Rigidbody>().useGravity = false;

        g.AddComponent<InteractiveMedicalObject>();
        g.GetComponent<InteractiveMedicalObject>().isGrabbable = true;
        g.GetComponent<InteractiveMedicalObject>().validDrop = InteractiveMedicalObject.ValidDropTypes.DropAnywhere;
        g.GetComponent<InteractiveMedicalObject>().holdButtonToGrab = true;
        g.GetComponent<InteractiveMedicalObject>().touchHighlightColor = Color.cyan;

        //objectsToGroup.Add(g);
    }

    //UTD2018
    public static GameObject MakeVolume(string path, Texture2D noise = null, Material loading = null)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.transform.position = Vector3.up;
        g.GetComponent<MeshRenderer>().material = loading;
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
