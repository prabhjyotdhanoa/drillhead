using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float forwardSpeed = 25f, strafeSpeed = 7.5f, hoverSpeed = 5f;
    private float activeForwardSpeed, activeStrafeSpeed, activeHoverSpeed;
    private float forwardAcceleration = 2.5f, strafeAcceleration = 2f, hoverAcceleration = 2f;

    public float lookRateSpeed = 90f;
    private Vector2 lookInput, screenCenter, mouseDistance;

    private float rollInput;
    public float rollSpeed = 90f, rollAcceleration = 3.5f;

    public float dashSpeed = 50f;
    public float dashDuration = 1f;
    public float dashCooldown = 5f;
    public float spinSpeed = 720f; // Degrees per second
    private bool isDashing = false;
    private float dashTimeRemaining = 0f;
    private float dashCooldownRemaining = 0f;

    public List<GameObject> enemyObjects; // Add enemy objects to this list in the inspector.

    //for fixing camera in dash
    private Transform mainCamera;
    private Quaternion originalCameraRotation;
    private float originalFOV;

    private Vector3 dashDirection;

    //for speedlines during dash
    public ParticleSystem speedLines;
    private ParticleSystem.ColorOverLifetimeModule speedLinesCol;
    private Gradient originalGradient;

    //for explosion when enemy is hit
    public ParticleSystem explosion;

    void Start()
    {
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;
        originalFOV = Camera.main.fieldOfView;

        speedLines.Stop();
        speedLinesCol = speedLines.colorOverLifetime;
        originalGradient = speedLinesCol.color.gradient;

        explosion.Stop();

        Cursor.lockState = CursorLockMode.Confined;
        Debug.Log("Player Start");
    }

    void Update()
    {
        speedLines.transform.position = transform.position;
        if (isDashing)
        {
            mainCamera = Camera.main.transform;
            HandleDash();
            return;
        }

        if (!isDashing)
        {
            // Smoothly transition the FOV back to the original value when not dashing
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 65f, Time.deltaTime * 5f);
            speedLines.transform.rotation = (transform.rotation);
        }

        // Cooldown management
        if (dashCooldownRemaining > 0)
        {
            dashCooldownRemaining -= Time.deltaTime;
        }

        HandleMovement();
        HandleDashInput();
        HandleDashCooldownReduction();
    }

    void OnTriggerEnter(Collider other)
    {

        Debug.Log("Player ship entered a trigger zone!");
        if (other.gameObject.tag == "enemy")
        {
            explosion.transform.position = other.transform.position;
            Debug.Log("Player ship entered enemy trigger zone!");

            if (isDashing)
            {
                // Perform additional logic if dashing
                dashTimeRemaining = dashDuration;
                explosion.Play();

                var col = speedLines.colorOverLifetime;
                col.enabled = true;

                Gradient grad = new Gradient();
                grad.SetKeys(new GradientColorKey[] { 
                    new GradientColorKey(Color.red, 0.0f), 
                    new GradientColorKey(Color.yellow, 1.0f) }, 
                    new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.5f, 1.0f) 
                    });
                col.color = grad;
                Debug.Log("Dash continued through the enemy!");
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        explosion.Stop();
        var col = speedLines.colorOverLifetime;
        col.enabled = true; // Restore the enabled state
        col.color = originalGradient; // Restore the original color

    }

    private void HandleDashCooldownReduction() 
    { 
        //spining the plane reduces ur dash cooldown
        //todo - add a UI bar to show recharge
        if (dashCooldownRemaining >= 0 && Mathf.Abs(rollInput) > 0.05)
        {
            dashCooldownRemaining -= (Time.deltaTime);
        
        }
    }
    private void HandleMovement()
    {
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);

        rollInput = Mathf.Lerp(rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime);

        transform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime, mouseDistance.x * lookRateSpeed * Time.deltaTime, rollInput * rollSpeed * Time.deltaTime, Space.Self);

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, forwardAcceleration * Time.deltaTime);
        activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, Input.GetAxisRaw("Horizontal") * strafeSpeed, strafeAcceleration * Time.deltaTime);
        activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed, hoverAcceleration * Time.deltaTime);

        transform.position += transform.forward * activeForwardSpeed * Time.deltaTime;
        transform.position += (transform.right * activeStrafeSpeed * Time.deltaTime) + (transform.up * activeHoverSpeed * Time.deltaTime);
    }

    private void HandleDashInput()
    {
        if (Input.GetMouseButtonDown(1) && dashCooldownRemaining <= 0)
        {
            isDashing = true;
            dashTimeRemaining = dashDuration;
            //dashDirection = transform.forward;
            dashCooldownRemaining = dashCooldown;
        }
    }

    private void HandleDash()
    {
        // Spin the ship during dash;
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime, Space.Self);
        speedLines.transform.Rotate(0, 0, spinSpeed * Time.deltaTime, Space.Self);

        //keep camera in rotation that it had prior to dashing
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 85f, Time.deltaTime * 10f);
        // Move forward quickly
        transform.position += transform.forward * dashSpeed * Time.deltaTime;
        transform.position += (transform.right * (activeStrafeSpeed*dashSpeed/3) * Time.deltaTime) + (transform.up * (activeHoverSpeed*dashSpeed/3) * Time.deltaTime);


        // Start the particle system if not already playing
        if (!speedLines.isPlaying)
        {
            speedLines.Play();
        }


        // Reduce dash time remaining
        dashTimeRemaining -= Time.deltaTime;

        if (dashTimeRemaining <= 0)
        {
            isDashing = false;
            mainCamera.rotation = transform.rotation;

            if (speedLines.isPlaying)
            {
                speedLines.Stop();
            }
        }
    }
}
