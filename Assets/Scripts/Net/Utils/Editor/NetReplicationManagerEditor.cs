using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NetReplicationStream))]
public class NetReplicationStreamEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        NetReplicationStream myScript = (NetReplicationStream)target;
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
