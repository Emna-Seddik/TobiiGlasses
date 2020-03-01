using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Donnees : MonoBehaviour
{
  
        [XmlAttribute("Abscisses")]
        public string Abscisse;

        [XmlElement("element1")]
        public float Xboule;

        [XmlElement("element2")]
        public float Xregard;

    public Donnees(string abs, float x, float y)
    {
        Abscisse = abs;
        Xboule = x;
        Xregard = y;
    }
    
}
