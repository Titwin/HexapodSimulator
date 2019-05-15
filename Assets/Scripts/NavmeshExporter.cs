using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class NavmeshExporter : MonoBehaviour {
    public string fileName;
    public NavmeshNode[] navmesh;

    // Use this for initialization
    void Start () {
        Export();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Export()
    {
        StreamWriter writer = new StreamWriter(fileName, false);

        writer.Write("Number of node : : " + navmesh.Length + "\n");

        foreach (NavmeshNode n in navmesh)
        {
            writer.Write("node: " + n.id + " : ");

            writer.Write(0.1f * n.transform.position.x + " ");
            writer.Write(0.1f * n.transform.position.y + " ");
            writer.Write(0.1f * n.transform.position.z + " ");

            writer.Write("   [");
            for (int i = 0; i< n.neighbours.Length; i++)
            {
                writer.Write(n.neighbours[i].id);
                if(i != n.neighbours.Length - 1)
                    writer.Write(", ");
            }
            writer.Write("]\n");
        }

        writer.Close();
    }
}
