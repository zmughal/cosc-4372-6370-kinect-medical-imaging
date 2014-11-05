using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WindowsKinect = Windows.Kinect;

public class PlayerController : MonoBehaviour
{
    public GameObject Kinect;
    public GameObject HandWave;
    public GameObject Jump;
    public GUIText Instruction;

    public float Speed { get; private set; }
    public Vector3 Destination { get; set; }

    private ulong? trackingId;
    private bool isMoving = true;
    private Vector3 stopPosition;
    private HandWaveDetector handWaveController;
    private JumpDetector jumpController;

    private string noInstructionText = "Don't do anything yet!";

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(transform.position, 1);
    //    Gizmos.DrawRay
    //}

    private void UpdateInstruction(string newInstruction)
    {
        Instruction.text = newInstruction;
    }

    void Start()
    {
        Speed = 120f;
        Destination = gameObject.transform.position;
        UpdateInstruction(noInstructionText);
    }

    void Update()
    {
        KinectController kinect = Kinect.GetComponent<KinectController>();
        handWaveController = HandWave.GetComponent<HandWaveDetector>();
        jumpController = Jump.GetComponent<JumpDetector>();

        if (kinect.PlayerSkeleton == null)
        {
            rigidbody.velocity = Vector3.zero;
            return;
        }

        UpdateHandWave(kinect);
        UpdateJump(kinect);
    }

    void LateUpdate()
    {
        if (isMoving)
        {
            rigidbody.AddForce((Destination - transform.position).normalized * Speed * Time.deltaTime);
            //transform.rotation = Quaternion.Slerp(transform.rotation,
            //    Quaternion.LookRotation(destination), 1f * Time.deltaTime);
            //transform.position += transform.forward * 2f * Time.deltaTime;
        }
        else
        {
            transform.position = stopPosition;
        }
    }

    private void UpdateHandWave(KinectController kinect)
    {
        if (handWaveController.IsActivated)
        {
            if (handWaveController.IsPassing)
            {
                handWaveController.Deactivate();
                isMoving = true;
                UpdateInstruction(noInstructionText);
            }
            else
            {
                WindowsKinect.Body body = kinect.PlayerSkeleton;

                handWaveController.Set(
                    body.GetPosition(WindowsKinect.JointType.HandLeft),
                    body.GetPosition(WindowsKinect.JointType.HandRight),
                    body.GetPosition(WindowsKinect.JointType.ElbowLeft),
                    body.GetPosition(WindowsKinect.JointType.ElbowRight));
            }
        }
    }

    private void UpdateJump(KinectController kinect)
    {
        if (jumpController.IsActivated)
        {
            if (jumpController.IsPassing)
            {
                jumpController.Deactivate();
                isMoving = true;
                UpdateInstruction(noInstructionText);
            }
            else
            {
                WindowsKinect.Body body = kinect.PlayerSkeleton;

                jumpController.Set(
                    body.GetPosition(WindowsKinect.JointType.FootLeft),
                    body.GetPosition(WindowsKinect.JointType.FootRight));
            }
        }
    }

    public bool IsAtDestination
    {
        get { return (gameObject.transform.position - Destination).magnitude < 0.5f; }
    }

    void OnCollisionEnter(Collision otherObject)
    {
        string tag = otherObject.gameObject.tag;

        if (tag == "Wave")
        {
            InstructPlayer("WAVE YOUR HAND");
            handWaveController.Activate();
            otherObject.gameObject.SetActive(false);
        }
        else if (tag == "Jump")
        {
            InstructPlayer("JUMP");
            jumpController.Activate();
            otherObject.gameObject.SetActive(false);
        }
        else if (tag == "Fly")
        {
            InstructPlayer("FLY WITH YOUR HANDS");
            renderer.enabled = false;
            otherObject.gameObject.SetActive(false);
        }
    }

    private void InstructPlayer(string instruction)
    {
        rigidbody.velocity = Vector3.zero;
        stopPosition = transform.position;
        isMoving = false;
        UpdateInstruction(instruction);
    }
}
