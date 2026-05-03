using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //you spin me right round baby right round like a record baby right round round round
        transform.Rotate(new Vector3(0, 100, 0) * Time.deltaTime);
    }
}
