using UnityEngine;


//Moves Camera with Keys WASD
public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f; 

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); 
        float moveZ = Input.GetAxis("Vertical");  

        
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ);

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }
}
