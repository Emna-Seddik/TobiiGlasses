using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Coordonnées : MonoBehaviour
{
    [XmlAttribute("Abscisses")]
    public string Abscisses;

    public int xBoule;
    public int xRegard;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
