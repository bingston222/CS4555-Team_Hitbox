using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitchZone : MonoBehaviour
{
    public CinemachineCamera myCam;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            myCam.Priority = 20; // activate this camera
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            myCam.Priority = 1; // deactivate
    }
}
