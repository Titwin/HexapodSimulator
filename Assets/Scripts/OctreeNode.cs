using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OctreeNode : MonoBehaviour
{
    public GameObject nodePrefab;
    public int[] divX;
    public int[] divY;
    public int[] divZ;
    public bool isRoot = false;
    private OctreeNode[] children = null;
    
    static public ulong visibitityRange = 1000;

    public ulong samplesCount = 0;

    // Use this for initialization
    void Start ()
    {
        if(isRoot)
        {
            int level = min(divX.Length , divY.Length, divZ.Length);

            System.Array.Resize(ref divX, level);
            System.Array.Resize(ref divY, level);
            System.Array.Resize(ref divZ, level);
        }
    }
    void Awake()
    {
        if (isRoot)
        {
            for (int i = 0; i < divX.Length; i++)
                Split(0);
        }
    }

    // subdivide and merge
    public void Merge(int level)
    {
        if (children != null && children.Length != 0)
        {
            for (int i = 0; i < children.Length; i++)
                children[i].Merge(level + 1);
            System.Array.Clear(children, 0, children.Length);
        }
        foreach (Transform child in transform)
            GameObject.Destroy(child.gameObject);
    }
    public void Split(int level)
    {
        if (level >= divX.Length) return;

        if (children == null || children.Length == 0)
        {
            children = new OctreeNode[divX[level] * divY[level] * divZ[level]];
            for (int i = 0; i < divX[level]; i++)
                for (int j = 0; j < divY[level]; j++)
                    for (int k = 0; k < divZ[level]; k++)
                    {
                        float sx = 1.0f / divX[level];
                        float sy = 1.0f / divY[level];
                        float sz = 1.0f / divZ[level];

                        float x = sx / 2.0f - 0.5f + i * sx;
                        float y = sy / 2.0f - 0.5f + j * sy;
                        float z = sz / 2.0f - 0.5f + k * sz;

                        int index = i * divY[level] * divX[level] + j * divZ[level] + k;

                        GameObject child = Instantiate(nodePrefab, transform, true);
                        child.transform.localPosition = new Vector3(x, y, z);
                        child.transform.localScale = new Vector3(sx, sy, sz);

                        children[index] = child.GetComponent<OctreeNode>();
                        children[index].isRoot = false;
                        children[index].divX = divX;
                        children[index].divY = divY;
                        children[index].divZ = divZ;
                        children[index].nodePrefab = nodePrefab;
                    }
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            for (int i=0; i<children.Length; i++)
                children[i].Split(level + 1);
        }
    }
    public void Increment()
    {
        if (children == null || children.Length == 0)
        {
            if (samplesCount < 100000000) samplesCount += 2;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (samplesCount > 0) samplesCount--;
        if (samplesCount > visibitityRange)
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        else if(samplesCount < 100)
            gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    // usefull
    int min(int a, int b, int c)
    {
        if (a <= b && a <= c) return a;
        else if (b <= a && b <= c) return b;
        else return c;
    }
}
