using UnityEngine;
using System.Collections.Generic;

using MarchingCubesProject;
using VolumeViewer;

public enum MARCHING_MODE { CUBES, TETRAHEDRON };

[RequireComponent(typeof(VolumeComponent))]
public class VolumeViewerMarcher : MonoBehaviour
{
    public Material m_material;

    public MARCHING_MODE mode = MARCHING_MODE.TETRAHEDRON;

    public int seed = 0;

    List<GameObject> meshes = new List<GameObject>();

    void Update()
    {
        if(Input.GetKeyDown(name: "f8"))
        {
            ConvertToMesh(GetComponent<VolumeComponent>().cutValueRangeMin);
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void ConvertToMesh(float cutoff = 0.5f)
    {
        //Set the mode used to create the mesh.
        //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface.
        Marching marching = null;
        if (mode == MARCHING_MODE.TETRAHEDRON)
            marching = new MarchingTertrahedron();
        else
            marching = new MarchingCubes();

        //Surface is the value that represents the surface of mesh
        //For example the perlin noise has a range of -1 to 1 so the mid point is where we want the surface to cut through.
        //The target value does not have to be the mid point it can be any value with in the range.
        marching.Surface = cutoff;
        
        VolumeFileLoader vfl = GetComponent<VolumeFileLoader>();

        //The size of voxel array.
        int width = vfl.dataVolume.nx;
        int height = vfl.dataVolume.ny;
        int length = vfl.dataVolume.nz;

        float[] voxels = new float[width * height * length];
        Color[] pixels = vfl.dataVolume.texture.GetPixels();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    float fx = x / (width - 1.0f);
                    float fy = y / (height - 1.0f);
                    float fz = z / (length - 1.0f);

                    int idx = x + y * width + z * width * height;

                    Color pixel = pixels[idx];
                    if (pixel.a > 0f)
                    {
                        voxels[idx] = pixel.a;
                    }
                }
            }
        }

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        Debug.Log("Starting marching cubes...");
        //The mesh produced is not optimal. There is one vert for each index.
        //Would need to weld vertices for better quality mesh.
        marching.Generate(voxels, width, height, length, verts, indices);

        //A mesh in unity can only be made up of 65000 verts.
        //Need to split the verts between multiple meshes.

        int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle
        int numMeshes = verts.Count / maxVertsPerMesh + 1;

        Debug.Log(verts.Count + " vertices scanned.");
        GameObject marchingObject = new GameObject("MarchingObject");
        marchingObject.transform.parent = transform;

        for (int i = 0; i < numMeshes; i++)
        {

            List<Vector3> splitVerts = new List<Vector3>();
            List<int> splitIndices = new List<int>();

            for (int j = 0; j < maxVertsPerMesh; j++)
            {
                int idx = i * maxVertsPerMesh + j;

                if (idx < verts.Count)
                {
                    splitVerts.Add(verts[idx]);
                    splitIndices.Add(j);
                }
            }

            if (splitVerts.Count == 0) continue;

            Mesh mesh = new Mesh();
            mesh.SetVertices(splitVerts);
            mesh.SetTriangles(splitIndices, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            GameObject go = new GameObject("Mesh");
            go.transform.parent = marchingObject.transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = m_material;
            go.GetComponent<MeshFilter>().mesh = mesh;
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);
            go.transform.localPosition = new Vector3((float)-width / 2, (float)-height / 2, (float)-length / 2);

            meshes.Add(go);
        }

        marchingObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        marchingObject.transform.localScale = new Vector3(1f / width, 1f / height, 1f / length);
        Debug.Log("Finishing marching cubes...");

    }
}
