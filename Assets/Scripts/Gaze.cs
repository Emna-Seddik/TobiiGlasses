using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TobiiGlasses;

public class Gaze : MonoBehaviour
{
    [SerializeField] private LayerMask screenLayer;
    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private Material normalMaterial;
    private Transform _selection;


    StreamReader stream = new StreamReader("E:\\emna\\datas\\29-02-2020\\gaze.txt");


    // Start is called before the first frame update
    void Start()
    {
        //this.transform.parent.Rotate(new Vector3(0.0f, 90.0f, 0.0f), Space.World);
    }

    // Update is called once per frame
    void Update()
    {
        string line = null;
        // Open the text file using a stream reader.

        // Read the stream to a string, and write the string to the con--sole.
        line = stream.ReadLine();
        Debug.Log(line);
        Vector3 gazeV = ConvertGP3Data.getValidGP3(line);
        Debug.Log("gazeV magnitude =" + gazeV.magnitude);
        //left handed sys coor
        Vector3 gazeVlh = new Vector3(-gazeV.x, gazeV.y, gazeV.z);
        string[] strings = line.Split(';');
        this.transform.localPosition = gazeVlh;
       
        // make the gaze bject look forrward
        //TODO : verify ths line
        this.transform.LookAt(2 * this.transform.position - this.transform.parent.position);

        //this.transform.parent.LookAt(new Vector3(1.3017f, 1.0175f, 0.9738f));
        //this.transform.parent.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);


        //Debug.DrawRay(this.transform.parent.position, new Vector3(1.3017f, 1.0175f, 0.9738f), Color.cyan);
        /*Debug.DrawRay(transform.parent.position, transform.forward * 10, Color.magenta);
        Debug.DrawRay(this.transform.position, transform.forward * 10, Color.red);
        Debug.DrawRay(this.transform.parent.position, transform.position, Color.blue);*/
        //Debug.DrawLine(this.transform.parent.position, this.transform.position, Color.yellow);
        //Debug.DrawRay(this.transform.parent.position, this.transform.forward*1000, Color.yellow);

        if (_selection != null) {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            if (selectionRenderer != null)
            {
                selectionRenderer.material = normalMaterial;
                _selection = null;
            }
        }
        RaycastHit hit;
        // Does the ray intersect any objects 
        if (Physics.Raycast(transform.parent.position, this.transform.forward, out hit, Mathf.Infinity, screenLayer))
        {
            var selection = hit.transform;
            var selectionRenderer = selection.GetComponent<Renderer>();
            if (selectionRenderer != null) {
                selectionRenderer.material = highlightedMaterial;
                _selection = selection;
            }

            if (hit.collider != null) { 
               // hit.collider.
            }
            Debug.DrawRay(transform.parent.position, this.transform.forward * hit.distance*10, Color.green);
            //Debug.DrawRay(transform.parent.position, this.transform.forward * , Color.green);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.parent.position, this.transform.forward * 1000, Color.white);
            Debug.Log("Did not Hit");
        }

    }
}
