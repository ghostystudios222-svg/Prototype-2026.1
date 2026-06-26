using UnityEngine;
using UnityEngine.InputSystem;

public class FishingRodController : MonoBehaviour
{
    [SerializeField] private GameObject fishingRod;
    [SerializeField] private GameObject bobberPrefab;
    [SerializeField] private Transform castPoint;
    [SerializeField] private float castForce = 10f;
    private bool isEquipped = false;
    private GameObject activeBobber;

    void Start()
    {
        fishingRod.SetActive(false);
    }

    void Update()
    {
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
        if (Mouse.current.rightButton.wasPressedThisFrame && isEquipped)
        {
            if (activeBobber != null)
            {
                Destroy(activeBobber);
                activeBobber = null;
            }

            activeBobber = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
            Rigidbody rb = activeBobber.AddComponent<Rigidbody>();
            rb.linearVelocity = castPoint.forward * castForce + Vector3.up * (castForce * 0.5f);
        }
    }
}
