using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Topology
{
  
    public class Node: MonoBehaviour
    {
        public string id,name;
        public TextMesh nodeText;

        

        private void Awake()
        {
            if(nodeText == null)
            {
                print("make text object");
                var _gameObject = new GameObject("NodeName");

               _gameObject.AddComponent<TextMesh>();

                _gameObject.transform.SetParent(gameObject.transform);

                nodeText = GetComponentInChildren<TextMesh>();

                nodeText.text = id;

            }
        }

        

        private void Update()
        {
            if (nodeText != null)
            {
                
                nodeText.transform.LookAt(transform.position-Camera.main.transform.position);
            }
        }
    }


}


