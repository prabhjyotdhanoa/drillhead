using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapGlowBuildings : MonoBehaviour
{
    public LayerMask minimapLayer; // Assign the "MinimapGlowProxy" layer
    public Material glowMaterial; // Assign the glowing material
    public float sphereRadius = 10.0f; // Radius of the sphere
    private GameObject proxy; // Proxy object reference

    void Start()
    {
        // Create a proxy object for the minimap
        proxy = GameObject.CreatePrimitive(PrimitiveType.Cube); // Create a sphere
        proxy.transform.position = transform.position;
        proxy.transform.rotation = transform.rotation;

        proxy.transform.localScale = new Vector3 (sphereRadius, 5*sphereRadius, sphereRadius); // Set sphere radius (diameter)
        proxy.layer = LayerMask.NameToLayer("MiniMap Glow");

        // Apply the glowing material
        MeshRenderer proxyRenderer = proxy.GetComponent<MeshRenderer>();
        proxyRenderer.sharedMaterial = glowMaterial;

    }

    void Update()
    {
        // Synchronize the proxy object's position, rotation, and scale with the real object
        if (proxy != null)
        {
            proxy.transform.position = transform.position;
            //proxy.transform.rotation = transform.rotation;
        }
    }

    void OnDestroy()
    {
        // Destroy the proxy object when the real object is destroyed
        if (proxy != null)
        {
            Destroy(proxy);
        }
    }
}