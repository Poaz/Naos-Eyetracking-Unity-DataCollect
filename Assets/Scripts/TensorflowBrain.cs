using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TensorFlow;

public class TensorflowBrain : MonoBehaviour
{
    private string modelFile;
    private TFTensor tensor;

    // Use this for initialization
    void Start() {

        modelFile = "my_model.pb";
        StartCoroutine(Predictor());
    }

    // Update is called once per frame
    void Update() {

    }

    public IEnumerator Predictor()
    {        
        using (var graph = new TFGraph())
        {
            tensor = new[] { 140.285470581055f, 41.11237678527829f, 5.836468505859f, 0.0f, 0.0f, 0.0f, 2.463f, 0.0f, 64.28699999999999f, 0.7287418126212261f };
            if (tensor != null)
            {
                
                //try
                //{
                graph.Import(File.ReadAllBytes("Assets/TensorModel/" + modelFile));
                    var session = new TFSession(graph);
                    var runner = session.GetRunner();
                    runner.AddInput(graph["dense_one/kernel"][0], tensor);
                    runner.Fetch(graph["training/Adam/Variable_17"][0]);
                    //runner.Fetch(graph["final/Softmax"][0]);



                    var output = runner.Run();

                    // Fetch the results from output:
                    TFTensor results = output[0];

                    print(results);
                    var re = (float[])results.GetValue(jagged: false);


                    for(int i = 0; i < re.Length; i++)
                    {
                        Debug.Log(re[i]);
                    }
                    

                //}
                //catch
                //{
                 //   Debug.Log("Error in evaluating model");
                //}

            }
        }

        yield return new WaitForSeconds(2f);

    }
}
