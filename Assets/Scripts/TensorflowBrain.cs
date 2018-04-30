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
    public DataGathering naos;
    public ReceiveLiveStream eyeData;

    void Start()
        {  
            modelFile = "my_model.pb";
            naos = GetComponent<DataGathering>();
            eyeData = GetComponent<ReceiveLiveStream>();
            StartCoroutine(Predictor());

        }

        void Update()
        {
        }

        TFTensor GetTensor()
        {
            TFTensor tmpTensor = new[]
            {
                (float) eyeData.GetGDLX(), (float) eyeData.GetGDLY(), (float) eyeData.GetGDLZ(),
                (float) eyeData.GetGDRX(), (float) eyeData.GetGDRY(), (float) eyeData.GetGDRZ(),
                (float) eyeData.GetPDL(), (float) eyeData.GetPDR(), (float) naos.GetHeartRate(),
                (float) naos.GetGsr()
            };
            return tmpTensor;
        }

        public IEnumerator Predictor()
        {
            using (var graph = new TFGraph())
            {
                TFOutput TFinput = graph.Placeholder(TFDataType.Float, new TFShape(-1, 10));
                TFOutput outputLayer = graph.Placeholder(TFDataType.Float, new TFShape(-1, 7));
                tensor = new[,]
                    {
                        {140.285470581055f, 41.11237678527829f, 5.836468505859f, 0.0f, 0.0f, 0.0f, 2.463f, 0.0f,
                        64.28699999999999f, 0.7287418126212261f},
                        {140.285470581055f, 41.11237678527829f, 5.836468505859f, 0.0f, 0.0f, 0.0f, 2.463f, 0.0f,
                        64.28699999999999f, 0.7287418126212261f}
                    };
            //tensor = GetTensor();
            if (tensor != null)
                {
                    graph.Import(File.ReadAllBytes("Assets/TensorModel/" + modelFile));
                    var session = new TFSession(graph);
                    var runner = session.GetRunner();

                    TFinput = graph["dense_one_input"][0];
                    outputLayer = graph["dense_3_out"][0];
                    runner.Fetch(outputLayer);
                    runner.AddInput(TFinput, tensor);
                    

                    var output = runner.Run();

                    TFTensor results = output[0];
                    Debug.Log(results);
                    //var re = (float[]) results.GetValue(jagged: false);

                    //foreach (var t in re) Debug.Log(t);
                }
            }
            return null;
        }
    }
