using UnityEngine;

public class TestPlayerMove : MonoBehaviour
{
    public float speed = 5f;
    private CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v) * speed;
        cc.SimpleMove(move);
    }
}