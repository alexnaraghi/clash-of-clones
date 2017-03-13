using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NetReplicationManager))]
public class NetReplicationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        NetReplicationManager myScript = (NetReplicationManager)target;
        if(GUILayout.Button("Take Snapshot"))
        {
            myScript.TakeSnapshot();
        }
        if(GUILayout.Button("Apply Snapshot"))
        {
            myScript.ApplySnapshot();
        }
    }
}
