using UnityEngine;

public class ArrowBob : MonoBehaviour
{
    [Header("Bob Settings")]
    [SerializeField] private float amplitude = 0.25f;
    [SerializeField] private float frequency = 2f;      

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition; 
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}
