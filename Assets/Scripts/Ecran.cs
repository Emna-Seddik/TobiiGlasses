﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Ecran : MonoBehaviour
{

    StreamReader stream = new StreamReader("E:\\emna\\datas\\29-02-2020\\ecran.txt");

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        string line = null;
        // Open the text file using a stream reader.

        // Read the stream to a string, and write the string to the con--sole.
        line = stream.ReadLine();
        Debug.Log(line);
        string[] strings = line.Split(';');
        this.transform.position = new Vector3(float.Parse(strings[0]), float.Parse(strings[1]), float.Parse(strings[2]));
        this.transform.rotation = new Quaternion(float.Parse(strings[3]), float.Parse(strings[4]), float.Parse(strings[5]), float.Parse(strings[6]));


    }
}
