using System;
using UnityEngine;

public class AnimalMovementComponent : MonoBehaviour
{
    public Action OnMoveDone;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float jumpTime = 0.5f; // Constant move+jump time

    private AnimalSO animalSO;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float elapsedTime;
    private bool isMoving;
    private bool isWaiting;
    private float waitTimeRemaining;

    private Quaternion startRotation;
    private Quaternion targetRotation;

    void Awake() {
        animalSO = GetComponent<AnimalBehaviour>().GetAnimalSO();
        moveDuration += UnityEngine.Random.Range(0, 0.05f) - 0.025f;
    }

    public void MoveTo(Vector3 target)
    {
        startPosition = transform.position;
        endPosition = target;
        elapsedTime = 0f;
        isMoving = true;
        isWaiting = false;
        waitTimeRemaining = (moveDuration / animalSO.Speed) - jumpTime;
        waitTimeRemaining = Mathf.Max(0f, waitTimeRemaining);

        // Setup smooth rotation
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.0001f)
        {
            startRotation = transform.rotation;
            targetRotation = Quaternion.LookRotation(direction);
        }
        else
        {
            startRotation = targetRotation = transform.rotation;
        }
    }

    void Update()
    {
        if (isMoving) {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / jumpTime);

            // Movement
            Vector3 flatPos = Vector3.Lerp(startPosition, endPosition, t);
            float height = jumpHeight * 4 * t * (1 - t);
            flatPos.y += height;
            transform.position = flatPos;

            // Smooth rotation
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            if (t >= 1f)
            {
                isMoving = false;
                isWaiting = true;
                elapsedTime = 0f;
            }
        }
        else if (isWaiting)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= waitTimeRemaining)
            {
                isWaiting = false;
                OnMoveDone?.Invoke();
            }
        }
    }

    public bool IsDoingMove() =>
        isWaiting || isMoving;
}
