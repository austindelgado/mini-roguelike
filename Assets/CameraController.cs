using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraController : MonoBehaviour
{
    public Transform player;

    Vector3 target, mousePos, refVel, shakeOffset;
    float cameraDist = 3.5f;
    float smoothTime = 0.2f, zStart;
    float shakeMag, shakeTimeEnd;
    Vector3 shakeVector;
    bool shaking;

    void Start() 
    {
        player = NetworkClient.localPlayer.gameObject.GetComponent<NetworkGamePlayerLobby>().player.transform;
        target = player.position;
        zStart = transform.position.z;
    }

    void LateUpdate()
    {
        mousePos = CaptureMousePos();
        target = UpdateTargetPos();
        UpdateCameraPos();
    }

    Vector3 CaptureMousePos()
    {
        Vector2 ret = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        ret *= 2;
        ret -= Vector2.one;

        float max = 0.9f;
        if (Mathf.Abs(ret.x) > max || Mathf.Abs(ret.y) > max)
            ret = ret.normalized;

        return ret;
    }

    Vector3 UpdateTargetPos()
    {
        Vector3 mouseOffset = mousePos * cameraDist;
        Vector3 ret = player.position + mouseOffset;
        ret.z = zStart;
        return ret;
    }

    void UpdateCameraPos()
    {
        Vector3 tempPos;
        tempPos = Vector3.SmoothDamp(transform.position, target, ref refVel, smoothTime);
        transform.position = tempPos;
    }
}
