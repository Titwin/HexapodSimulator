using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceScanner : MonoBehaviour
{
    public GameObject obstacle;

    [Range(1.0f, 25.0f)]
    public float distanceThreshold = 1.0f;
    public Transform[] sensors;


    // Set / get functions
    public void SetDistance(int sensor, float distance)
    {
        if (distance < distanceThreshold)
        {
            Color color = Color.Lerp(Color.red, Color.white, distance / distanceThreshold);
            Debug.DrawLine(sensors[sensor].position, sensors[sensor].position + distance * sensors[sensor].forward, color);

            float s = Mathf.Tan(Mathf.Deg2Rad * 25.0f / 2.0f) * distance;
            //InstanciateObstcle(sensors[sensor].position + (s + distance) * sensors[sensor].forward, new Vector3(s, s, s), Mathf.Lerp(3000.0f, 500.0f, distance / distanceThreshold));
            Collider[] nodes = Physics.OverlapBox(sensors[sensor].position + (s + distance) * sensors[sensor].forward, new Vector3(s, s, s), Quaternion.identity, (1 << 9));
            foreach(Collider node in nodes)
            {
                node.gameObject.GetComponent<OctreeNode>().Increment();
            }
        }
        else if(distance < 1.5f * distanceThreshold)
        {
            Color color = Color.Lerp(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), 2*(distance - distanceThreshold) / distanceThreshold);
            Debug.DrawLine(sensors[sensor].position, sensors[sensor].position + distance * sensors[sensor].forward, color);
        }
    }
    public void InstanciateObstcle(Vector3 position, Vector3 scale, float life)
    {
        GameObject p = Instantiate(obstacle, position, Quaternion.identity);
        p.transform.localScale = scale;

        Particle pp = p.GetComponent<Particle>();
        pp.life = life;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
