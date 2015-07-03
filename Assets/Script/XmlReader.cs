using UnityEngine;
using System.Collections;
using System.Xml;

public class XmlReader : MonoBehaviour {
    public static XmlReader Instance;
    public XmlElement root;
    public string version;
	// Use this for initialization
	void Awake () {
        XmlReader.Instance = this;

        string filepath = Application.dataPath + @"/database_12.19.xml";
        //Debug.Log(filepath.ToString());
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(filepath);
        root = xmlDoc.DocumentElement;
        foreach (XmlNode node in XmlReader.Instance.root.ChildNodes)
        {
            if(node.Name=="Version")
            {
                version = ((XmlElement)node).GetAttribute("value");
                return;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
