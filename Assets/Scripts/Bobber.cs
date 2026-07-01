using UnityEngine;
using System.Collections;

public class Bobber : MonoBehaviour
{
    [SerializeField] private float minBiteWait = 3f;
    [SerializeField] private float maxBiteWait = 10f;
    [SerializeField] private float biteWindow = 1f;

    public bool IsBiting { get; private set; }

    private Rigidbody rb;
    private bool isFloating = false;
    private Coroutine biteRoutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water") && !isFloating)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;

            transform.position = new Vector3(transform.position.x, other.bounds.max.y, transform.position.z);

            isFloating = true;
            biteRoutine = StartCoroutine(BiteCycle());
        }
    }

    // Loops indefinitely while the bobber floats: wait, bite window opens, repeat if missed
    private IEnumerator BiteCycle()
    {
        while (true)
        {
            float waitTime = Random.Range(minBiteWait, maxBiteWait);
            yield return new WaitForSeconds(waitTime);

            IsBiting = true;
            Debug.Log("Fish is biting!");

            yield return new WaitForSeconds(biteWindow);

            // Window closed without a hook -> fish gives up, timer restarts
            if (IsBiting)
            {
                IsBiting = false;
                Debug.Log("Missed it — waiting for another bite...");
            }
        }
    }
    
    // Called by FishingRodController on a successful right-click during the bite window
    public void Hook()
    {
        IsBiting = false;
        if (biteRoutine != null)
        {
            StopCoroutine(biteRoutine);
        }
        Debug.Log("Hooked! (reel-in minigame comes next)");
    }
}
