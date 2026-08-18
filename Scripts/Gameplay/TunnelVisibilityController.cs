using UnityEngine;
using System.Collections.Generic;

public class TunnelVisibilityController : MonoBehaviour
{
    [Header("Target Objects (Fish / Cubes)")]
    public GameObject[] targetObjects; // Drag fish/cubes here in prefab or after instantiating

    [Header("Trigger Distance")]
    public float triggerDistance = 1.5f; // Distance from camera to tunnel

    private Transform cameraTransform;
    private MeshRenderer[] meshRenderers;

    void Start()
    {
        // Find the Main Camera automatically
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("Main Camera not found! Make sure your scene has a camera tagged 'MainCamera'.");
        }

        // Collect all MeshRenderers from target objects
        List<MeshRenderer> allRenderers = new List<MeshRenderer>();

        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
            {
                MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();
                allRenderers.AddRange(renderers);
            }
        }

        meshRenderers = allRenderers.ToArray();
    }

    void Update()
    {
        if (cameraTransform == null || meshRenderers == null)
            return;

        float distance = Vector3.Distance(cameraTransform.position, transform.position);

        if (distance < triggerDistance)
        {
            SetObjectsVisible(false); // Camera inside tunnel
        }
        else
        {
            SetObjectsVisible(true); // Camera outside tunnel
        }
    }

    void SetObjectsVisible(bool isVisible)
    {
        foreach (MeshRenderer mr in meshRenderers)
        {
            if (mr != null)
                mr.enabled = isVisible;
        }
    }
}
