using UnityEngine;
//using UnityEditor;
using System.IO;

public class SceneExporter : MonoBehaviour {

    [SerializeField] string fileName;
    [SerializeField] Totem[] totems;


    void Start()
    {
        ExportScene();
    }
    //[MenuItem("Tools/Write file")]
    void ExportScene()
    {
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(fileName, false);

        int tidx = 0;
        foreach (Totem t in totems)
        {
            writer.WriteLine("Totem: "+ tidx);
            foreach (Marker m in t.markers)
            {
                writer.Write(m.markerID+" : ");
                writer.Write(0.1f * m.transform.position.x+" ");
                writer.Write(0.1f * m.transform.position.y + " ");
                writer.Write(0.1f * m.transform.position.z + " ");

                writer.Write(m.transform.rotation.x + " ");
                writer.Write(m.transform.rotation.y + " ");
                writer.Write(m.transform.rotation.z + " ");
                writer.Write(m.transform.rotation.w + " ");

                writer.Write(m.transform.localScale.x + " ");
                writer.Write(m.transform.localScale.y + " ");
                writer.Write(m.transform.localScale.z + "\n");

            }
            writer.WriteLine("");
            ++tidx;
        }
        writer.Close();

        //Re-import the file to update the reference in the editor
        //AssetDatabase.ImportAsset(path);
        //TextAsset asset = Resources.Load("test");

        //Print the text from the file
        //Debug.Log(asset.text);
    }

    //[MenuItem("Tools/Read file")]
    /*static void ReadString()
    {
        string path = "Assets/Resources/test.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }*/
}