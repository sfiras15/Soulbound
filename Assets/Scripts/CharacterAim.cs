using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAim : MonoBehaviour
{

    public Camera cam;

    public Vector3 mousePosition;

    public LayerMask whatIsGround;

    public Vector3 direction;

    public ThirdPersonController thirdPersonController;

    public Transform debugSphere;

    public float rotateSpeed;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;


    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        Ray rayPosition = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(rayPosition, out var hitInfo, Mathf.Infinity, whatIsGround))
        {
            mousePosition = hitInfo.point;
            direction = mousePosition - transform.position;
            direction.y = 0f;

            starterAssetsInputs.SetCursorState(false);
            debugSphere.position = mousePosition;
            thirdPersonController.RotateOnAim(true);

            transform.forward = Vector3.Lerp(transform.forward,direction,rotateSpeed * Time.deltaTime);
        }
    }
}
