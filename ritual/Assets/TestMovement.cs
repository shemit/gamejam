using UnityEngine;
using System.Collections;

public class TestMovement : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public float speed = 6.0F;
    private Vector3 moveDirection = Vector3.zero;

    // Update is called once per frame
    void Update () {
        CharacterController controller = GetComponent<CharacterController>();

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;

        controller.Move(moveDirection * Time.deltaTime);
    }
}
