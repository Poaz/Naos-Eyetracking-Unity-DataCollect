using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartData : MonoBehaviour {
    public GameObject dataObject;

    public void DestroyData()
    {
        GameObject dataObject;
        dataObject = GameObject.FindGameObjectWithTag("Data");
        Destroy(dataObject);
        StartData();
    }

    public void StartData()
    {
        Instantiate(dataObject, transform.position, Quaternion.identity);
    }
}
