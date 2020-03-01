using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QualisysRealTime.Unity;

public class GazeVector : MonoBehaviour
{
    public bool oneTarget, twoTargets, filtering, useMean;
    public int floatingFrames;
    private GameObject selectedObject, hitObject, objTarget, objTargetLeft, objTargetRight;
    ///private GameObject objTargetRight;
    private int increment;
    private int currentFrame;
    private List<Vector3> direction = new List<Vector3>();
    private Vector3 leftHitPoint;
    private Vector3 rightHitPoint;

    void Start() {
        floatingFrames = 5;
        increment = 0;
        currentFrame = 1;
        filtering = true;
        useMean = false;
        oneTarget = true;
        twoTargets = false;
    }
   
    List<GameObject> selectedObjects = new List<GameObject>();

    void Update() {
        objTarget = GameObject.Find("Target");
        objTargetLeft = GameObject.Find("Target Left");
        objTargetRight = GameObject.Find("Target Right");
        var gazeVectorStream = GetComponent<RTGazeStream>();

        foreach (var selectedObject in selectedObjects) {
            selectedObject.GetComponent<Renderer>().material.color = Color.white;
        }

        selectedObjects.Clear();

        foreach (var vectorData in gazeVectorStream.gazeVectorData) {
            // filtering ----------------------------
            while (direction.Count > floatingFrames) { //save last n frames to an array
                direction.RemoveAt(0);
            }

            if (direction.Count<= floatingFrames) {
                direction.Add(vectorData.Direction);
            }
            
            if (currentFrame >= floatingFrames) { //start filtering after collecting enough data
                Vector3 medianVector = GetMedianVector(direction);
                Vector3 meanVector = new Vector3(direction.Average(x=>x.x), direction.Average(x=>x.y), direction.Average(x=>x.z));

                if (filtering) {
                    if (useMean) {
                        vectorData.Direction = meanVector;
                    }
                    else {
                        vectorData.Direction = medianVector;
                    }
                }
            }

            RaycastHit hit;
            if (Physics.Raycast(vectorData.Position, vectorData.Direction, out hit, 20.0f, Physics.DefaultRaycastLayers)) {
                hitObject = GameObject.Find(hit.collider.gameObject.name);
                hitObject.GetComponent<Renderer>().material.color = Color.blue;
                selectedObjects.Add(hit.collider.gameObject);

                // Find the line from the eye to the projection point
                Vector3 incomingVec = hit.point - vectorData.Position;
                Debug.DrawLine(vectorData.Position, hit.point, Color.red);

                if (oneTarget) {
                    objTarget.GetComponent<Renderer>().enabled = true;
                    objTarget.GetComponent<TrailRenderer>().enabled = true;
                }
                if (twoTargets) {
                    objTargetLeft.GetComponent<Renderer>().enabled = true;
                    objTargetRight.GetComponent<Renderer>().enabled = true;
                    objTargetLeft.GetComponent<TrailRenderer>().enabled = true;
                    objTargetRight.GetComponent<TrailRenderer>().enabled = true;
                }

                if (vectorData.Name == "Gaze vector 1 (L)") {
                    leftHitPoint = hit.point;
                    objTargetLeft.transform.position = leftHitPoint;
                }

                if (vectorData.Name == "Gaze vector 1 (R)") {
                    rightHitPoint = hit.point;
                    objTargetRight.transform.position = rightHitPoint;
                }
            }
            else {
                if (hitObject) {
                    hitObject.GetComponent<Renderer>().material.color = Color.white;
                }
                objTarget.GetComponent<Renderer>().enabled = false;
                objTarget.GetComponent<TrailRenderer>().enabled = false;
                objTargetLeft.GetComponent<Renderer>().enabled = false;
                objTargetRight.GetComponent<Renderer>().enabled = false;
                objTargetLeft.GetComponent<TrailRenderer>().enabled = false;
                objTargetRight.GetComponent<TrailRenderer>().enabled = false;
            }

            Vector3 meanHitPoint = Vector3.Lerp(leftHitPoint, rightHitPoint, 0.5f);
            objTarget.transform.position = meanHitPoint;
       }

        if (currentFrame % 5 == 0) {
            increment = 0;
        }
        else {
            increment++;
        }
        currentFrame++;
    }
    public Vector3 GetMedianVector(List<Vector3> vector) {
        Vector3 median;
        int count = vector.Count - 1;
        int i = count / 2;

        Vector3[] myVectorX = vector.OrderBy(v=>v.x).ToArray<Vector3>();
        Vector3[] myVectorY = vector.OrderBy(v=>v.y).ToArray<Vector3>();
        Vector3[] myVectorZ = vector.OrderBy(v=>v.z).ToArray<Vector3>();
        median.x = myVectorX[i].x;
        median.y = myVectorY[i].y;
        median.z = myVectorZ[i].z;

        return median;
    }
}
