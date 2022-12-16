using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeMechanics : MonoBehaviour
{

    public GameObject playerObject;

    float treeAlpha = 1.0f;

    bool allowHit = true;

    public int LogsFromHit = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Tree" && allowHit)
        {
            StartCoroutine(handleHit(collision.gameObject));
            
            // Den här också
            // allowHit = false;
        }
    }

    IEnumerator handleHit(GameObject treeObject)
    {
        yield return new WaitForSeconds(0.4f);
        treeObject.GetComponent<TreeScript>().Health -= 1;
        playerObject.GetComponent<SC_FPSController>().LogsCount += LogsFromHit;
        Debug.Log(treeObject.GetComponent<TreeScript>().Health);
        treeAlpha -= 0.1f;
        treeObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, treeAlpha);
        if(treeObject.GetComponent<TreeScript>().Health < 1){
            Destroy(treeObject);
            treeAlpha = 1f;
        }
        // Använd kod nedanför för att fixa trähuggnings bugg, kommentera för speedrun feature ;) 
        // yield return new WaitForSeconds(0.65f);
        // allowHit = true;
    }

}
