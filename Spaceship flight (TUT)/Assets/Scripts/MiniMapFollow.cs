using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{

    //public Transform player; // Assign the player object in the Inspector
    //public float height = 100f; // Height of the minimap camera above the player
    //public float xAngle = 45f; // Height of the minimap camera above the player
    //// Start is called before the first frame update
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (player != null)
    //    {
    //        //Vector3 newPosition = player.position;
    //        //newPosition.y += height; // Keep the camera at a fixed height
    //        transform.position = newPosition;
    //        //transform.rotation = Quaternion.Euler(xAngle, 0f, player.eulerAngles.y); // Match player rotation if needed
    //    }
    //}

    public Transform player; // The player object
    public float height = 100f; // Height of the minimap camera above the player
    public float distance = 50f; // Distance from the player (radius of the circle in XZ plane)
    public float rotationSpeed = 10f; // Speed of rotation around the player
    private float currentAngle = 0f; // The current angle around the player

    void Start()
    {
        // Optionally, you can set the initial camera position and rotation if needed
        currentAngle = transform.eulerAngles.y;
    }

    void Update()
    {
        if (player != null)
        {
            // Calculate the angle around the player based on the player's rotation
            currentAngle = player.eulerAngles.y;

            // Calculate the new X and Z position of the camera based on the current angle
            float radianAngle = Mathf.Deg2Rad * currentAngle; // Convert angle to radians
            float x = player.position.x + Mathf.Cos(radianAngle) * distance;
            float z = player.position.z + Mathf.Sin(radianAngle) * distance;

            // Set the camera position above the player, but at a fixed height
            transform.position = new Vector3(x, player.position.y + height, z);

            // Optionally, make the camera always face downward towards the player
            transform.LookAt(new Vector3(player.position.x, player.position.y + height, player.position.z));
        }
    }
}
