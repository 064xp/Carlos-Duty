using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindRandomPoint {

    List<Vector3> VerticeList = new List<Vector3>(); //List of local vertices on the plane
    List<Vector3> Corners = new List<Vector3>();
    //public int sphereSize = 1;
    List<Vector3> EdgeVectors = new List<Vector3>();
    GameObject plane;

    public FindRandomPoint(GameObject plane) {
        this.plane = plane;
        VerticeList = new List<Vector3>(plane.GetComponent<MeshFilter>().sharedMesh.vertices); //get vertice points from the mesh of the object
        CalculateCornerPoints();
    }


    public void RecalculateTransform() {
        VerticeList = new List<Vector3>(plane.GetComponent<MeshFilter>().sharedMesh.vertices); //get vertice points from the mesh of the object
        CalculateCornerPoints();
    }

    void CalculateEdgeVectors(int VectorCorner) {
        EdgeVectors.Clear();

        EdgeVectors.Add(Corners[3] - Corners[VectorCorner]);
        EdgeVectors.Add(Corners[1] - Corners[VectorCorner]);
    }

    public Vector3 CalculateRandomPoint() {
        int randomCornerIdx = Random.Range(0, 2) == 0 ? 0 : 2; //there is two triangles in a plane, which tirangle contains the random point is chosen
                                                               //corner point is chosen for triangles as the variable

        CalculateEdgeVectors(randomCornerIdx); //in case of transform changes edge vectors change tddoo
        float u = Random.Range(0.0f, 1.0f);
        float v = Random.Range(0.0f, 1.0f);

        if (v + u > 1) //sum of coordinates should be smaller than 1 for the point be inside the triangle
        {
            v = 1 - v;
            u = 1 - u;
        }

        return Corners[randomCornerIdx] + u * EdgeVectors[0] + v * EdgeVectors[1];
    }
    public void CalculateCornerPoints() {
        Corners.Clear(); //in case of transform changes corner points are reset

        Corners.Add(plane.transform.TransformPoint(VerticeList[0])); //corner points are added to show  on the editor
        Corners.Add(plane.transform.TransformPoint(VerticeList[10]));
        Corners.Add(plane.transform.TransformPoint(VerticeList[110]));
        Corners.Add(plane.transform.TransformPoint(VerticeList[120]));
    }
}
