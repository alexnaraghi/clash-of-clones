using UnityEngine;

public class NetworkTest : MonoBehaviour 
{
    private NetTransportManager _network;

    // Use this for initialization
    void Start () 
    {
        _network = GetComponent<NetTransportManager>();
    }

    public void Connect()
    {
        _network.Connect("192.168.0.8");
    }

    public void Disconnect()
    {
        if(_network.Connections.Count > 0)
        {
            _network.Disconnect(_network.Connections[0]);
        }
        else
        {
            Debug.Log("No connections to disconnect from.");
        }
    }

    public void Send()
    {
        if(_network.Connections.Count > 0)
        {
            _network.Send(_network.Connections[0], new byte[] { 0x20, 0x20 }, isReliable : true );
        }
        else
        {
            Debug.Log("No connections to send to.");
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Connect"))
        {
            Connect();
        }

        if (GUILayout.Button("Disconnect"))
        {
            Disconnect();
        }

        if (GUILayout.Button("Send"))
        {
            Send();
        }
    }
}
