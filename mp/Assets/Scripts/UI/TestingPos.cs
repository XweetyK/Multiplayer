using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Net;

public class TestingPos : MonoBehaviourSingleton<TestingPos>
{
    public GameObject _cube;
    byte[] _pos;

    protected override void Initialize() {

        NetworkManager.Instance.OnReceiveEvent += OnReceiveDataEvent;
    }

    void OnReceiveDataEvent(byte[] data, IPEndPoint ep) {
        Vector3 pos = Vector3.zero;
        pos.x = BitConverter.ToSingle(data, 0);
        pos.y = BitConverter.ToSingle(data, 1*sizeof(float));
        pos.z = BitConverter.ToSingle(data, 2*sizeof(float));

        _cube.transform.position = pos;

        if (NetworkManager.Instance.isServer) {
            NetworkManager.Instance.Broadcast(data);
        }
    }

    private void Update() {
        Debug.Log(_cube.transform.position);
        if (Input.GetButtonDown("Submit")) {
            _pos = new byte[(sizeof(float)) * 3]; // 4 bytes per float

            Buffer.BlockCopy(_pos, 0, BitConverter.GetBytes(_cube.transform.position.x), 0, 4);
            Buffer.BlockCopy(_pos, 4, BitConverter.GetBytes(_cube.transform.position.y), 0, 4);
            Buffer.BlockCopy(_pos, 8, BitConverter.GetBytes(_cube.transform.position.z), 0, 4);

            if (NetworkManager.Instance.isServer) {
                NetworkManager.Instance.Broadcast(_pos);
            } else {
                NetworkManager.Instance.SendToServer(_pos);
            }
        }
    }
}
