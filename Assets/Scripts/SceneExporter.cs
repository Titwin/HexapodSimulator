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
            writer.Write("Totem: "+ t.TotemID + " : ");

            writer.Write(0.1f * t.transform.position.x + " ");
            writer.Write(0.1f * t.transform.position.y + " ");
            writer.Write(0.1f * t.transform.position.z + " ");

            writer.Write(t.transform.rotation.x + " ");
            writer.Write(t.transform.rotation.y + " ");
            writer.Write(t.transform.rotation.z + " ");
            writer.Write(t.transform.rotation.w + " ");

            writer.Write(t.transform.localScale.x + " ");
            writer.Write(t.transform.localScale.y + " ");
            writer.Write(t.transform.localScale.z + "\n");


            foreach (Marker m in t.markers)
            {
                writer.Write(m.markerID+" : ");
                writer.Write(0.1f * m.transform.localPosition.x+" ");
                writer.Write(0.1f * m.transform.localPosition.y + " ");
                writer.Write(0.1f * m.transform.localPosition.z + " ");

                writer.Write(m.transform.localRotation.x + " ");
                writer.Write(m.transform.localRotation.y + " ");
                writer.Write(m.transform.localRotation.z + " ");
                writer.Write(m.transform.localRotation.w + " ");

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