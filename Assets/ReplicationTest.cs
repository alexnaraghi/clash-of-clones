using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplicationTest : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
        SL.Get<NetReplicationManager>().Spawn(gameObject);
    }
}
