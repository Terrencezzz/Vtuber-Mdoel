using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class UDP : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;
    private bool isRunning = false;

    public List<Landmark> landmarks = new List<Landmark>();
    public List<HandLandmark> leftHandLandmarks = new List<HandLandmark>();
    public List<HandLandmark> rightHandLandmarks = new List<HandLandmark>();

    // Start is called before the first frame update
    void Start()
    {
        StartReceiver();
    }

    void StartReceiver()
    {
        udpClient = new UdpClient(5052);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        isRunning = true;
        receiveThread.Start();
        Debug.Log("UDP Receiver started.");
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (isRunning)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string json = Encoding.UTF8.GetString(data);

                Debug.Log("Received JSON: " + json);

                JObject jsonData = JObject.Parse(json);
                string type = (string)jsonData["type"];

                if (type == "pose")
                {
                    // Deserialize pose data
                    PoseData poseData = jsonData.ToObject<PoseData>();

                    lock (landmarks)
                    {
                        landmarks = poseData.landmarks;
                    }

                    Debug.Log("Received pose data.");
                }
                else if (type == "hand")
                {
                    // Deserialize hand data
                    HandData handData = jsonData.ToObject<HandData>();

                    if (handData.handedness == "Right")
                    {
                        lock (leftHandLandmarks)
                        {
                            leftHandLandmarks = handData.hand_landmarks;
                        }

                        Debug.Log("Received Right hand data.");
                    }
                    else if (handData.handedness == "Left")
                    {
                        lock (rightHandLandmarks)
                        {
                            rightHandLandmarks = handData.hand_landmarks;
                        }

                        Debug.Log("Received Left hand data.");
                    }
                    else
                    {
                        Debug.LogWarning("Unknown handedness: " + handData.handedness);
                    }
                }
                else
                {
                    Debug.LogWarning("Unknown data type received: " + type);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error receiving data: " + ex.Message);
            }
        }
    }

    [System.Serializable]
    public class ReceivedData
    {
        public string type;
    }

    [System.Serializable]
    public class PoseData
    {
        public string type;
        public List<Landmark> landmarks;
    }

    [System.Serializable]
    public class HandData
    {
        public string type;
        public string handedness;
        public List<HandLandmark> hand_landmarks;
    }

    [System.Serializable]
    public class Landmark
    {
        public float x;
        public float y;
        public float z;
        public float visibility;
    }

    [System.Serializable]
    public class HandLandmark
    {
        public float x;
        public float y;
        public float z;
    }
}
