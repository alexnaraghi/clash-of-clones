using System;
using UnityEngine;

public class NetworkTest : MonoBehaviour 
{
    public bool IsConnected;

    private NetAgentManager _network;
    private NetConnection _connection;

    // Use this for initialization
    void Start () 
    {
        _network = SL.Get<NetAgentManager>();
    }

    private void onDisconnected(NetConnection arg0)
    {
        IsConnected = false;
    }

    private void onConnectionFailed(string arg0)
    {
        // Do something?
    }

    private void onConnected(NetConnection arg0)
    {
        Log.Debug(this, "connected: " + arg0.ConnectionId);
        _connection = arg0;
        IsConnected = true;
    }

    public void Host()
    {
        _network.StartHost();
    }

    public void Client()
    {
        _network.StartClient();
    }

    public void Disconnect()
    {
        _network.Disconnect();
    }

    private void OnGUI()
    {

        GUILayout.BeginArea(new Rect(5, 5, 200, 300));
        int y = 10;
        const int yIncrement = 25;
        const int yLabelIncrement = 20;

        GUI.Label(new Rect(0, y, 200, 20), "My port");
        y += yLabelIncrement;
        _network.MyPort = GUI.TextField(new Rect(0, y, 200, 20), _network.MyPort, 25);
        y += yIncrement;
        GUI.Label(new Rect(0, y, 200, 20), "Connect ip");
        y += yLabelIncrement;
        _network.ConnectIp = GUI.TextField(new Rect(0, y, 200, 20), _network.ConnectIp, 25);
        y += yIncrement;
        GUI.Label(new Rect(0, y, 200, 20), "Connect port");
        y += yLabelIncrement;
        _network.ConnectPort = GUI.TextField(new Rect(0, y, 200, 20), _network.ConnectPort, 25);
        y += yIncrement;
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(5, 200, 200, 300));
        if (GUILayout.Button("Host"))
        {
            Host();
        }

        if (GUILayout.Button("Client"))
        {
            Client();
        }

        if (GUILayout.Button("Disconnect"))
        {
            Disconnect();
        }

        GUILayout.EndArea();
    }
}
