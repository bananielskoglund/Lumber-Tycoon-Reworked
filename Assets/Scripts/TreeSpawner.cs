using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject treeObject;

    public GameObject treesInWorldObject;

    public int treeAmount;
    private float rotationSpaceBound = 0.1f;
    private int getPositionRayStartHeight = 1000;

    List<GameObject> treesList = new List<GameObject>();

    GameObject[] treesArray;

    private Vector3 getPosition()
    {

        RaycastHit hit;
        Vector3 pos = new Vector3(Random.Range(50, 230), getPositionRayStartHeight, Random.Range(50, 230));

        Ray ray = new Ray(pos, -transform.up);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.point != null)
            {
                pos.y = hit.point.y - 0.25f;
                return pos;
            }
        }
        return pos;

    }

    private bool isCollidingOrSomething(GameObject obj)
    {
        return false;
    }

    private void fixCollision(GameObject obj)
    {
        while (isCollidingOrSomething(obj)) {
            obj.transform.position = getPosition();
        }
    }
    


    void Start()
    {
        for (int i = 0; i <= treeAmount; i++)
        {
            treesList.Add(Instantiate<GameObject>(treeObject));
            treesArray = treesList.ToArray();

            treesArray[i].transform.position = getPosition();
            fixCollision(treesArray[i]);

            // treesArray[i].transform.rotation = new Quaternion(Random.Range(0, 360), 0, Random.Range(0, 360), 1);
            treesArray[i].transform.rotation = new Quaternion(0, Random.Range(0, 180), 0, 1);
            treesArray[i].transform.parent = treesInWorldObject.transform;
        }
    }
}
