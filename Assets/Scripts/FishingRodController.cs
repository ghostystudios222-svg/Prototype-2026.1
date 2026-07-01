using UnityEngine;
using UnityEngine.InputSystem;

public class FishingRodController : MonoBehaviour
{
    [SerializeField] private GameObject fishingRod;
    [SerializeField] private GameObject bobberPrefab;
    [SerializeField] private Transform castPoint;
    [SerializeField] private LineRenderer fishingLine;
    [SerializeField] private float minCastForce = 3f;
    [SerializeField] private float maxCastForce = 15f;
    [SerializeField] private float maxChargeTime = 3f;

    private bool isEquipped = false;
    private GameObject activeBobber;
    private bool isCharging = false;
    private float chargeStartTime;

    // Rod starts unequipped and hidden, line starts off until a bobber exists
    void Start()
    {
        fishingRod.SetActive(false);
        fishingLine.positionCount = 2;
        fishingLine.enabled = false;
    }

    // Handles: equip toggle, charge-cast input, and keeping the line synced to the bobber
    void Update()
    {
        // Toggle fishing rod equip state
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            isEquipped = !isEquipped;
            fishingRod.SetActive(isEquipped);

            if (!isEquipped && activeBobber != null)
            {
                Destroy(activeBobber);
                activeBobber = null;
            }
        }
        // Skip all fishing input while the rod isn't equipped
        if (!isEquipped)
        {
            fishingLine.enabled = false;
            return;
        }

        // Right-click press: start charging a new cast, OR reset/cancel an existing one
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (activeBobber == null && !isCharging)
            {
                // No bobber yet -> start charging a new cast
                isCharging = true;
                chargeStartTime = Time.time;
            }
            else if (activeBobber != null)
            {
                Bobber bobber = activeBobber.GetComponent<Bobber>();

                if (bobber.IsBiting)
                {
                    // Right click landed inside the bite window -> successfull hook
                    bobber.Hook();
                }
                else
                {
                    // Too early, or no bite -> reset the line
                    Destroy(activeBobber);
                    activeBobber = null; 
                }
                
            }
        }
        // Right-click release: fire the cast, force scaled by how long the button was held
        if (Mouse.current.rightButton.wasReleasedThisFrame && isCharging)
        {
            isCharging = false;
            float chargeTime = Mathf.Clamp(Time.time - chargeStartTime, 0f, maxChargeTime);
            float force = Mathf.Lerp(minCastForce, maxCastForce, chargeTime / maxChargeTime);

            activeBobber = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
            Rigidbody rb = activeBobber.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.linearVelocity = castPoint.forward * force + Vector3.up * (force * 0.5f);
        }

        // If bobber is casted then update the fishing line positions
        if (activeBobber != null)
        {
            fishingLine.enabled = true;
            fishingLine.SetPosition(0, castPoint.position);
            fishingLine.SetPosition(1, activeBobber.transform.position);
        }
        else
        {
            fishingLine.enabled = false;
        }
    }
}
