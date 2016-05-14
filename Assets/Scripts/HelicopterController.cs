using System;
using UnityEngine;
using UnityEngine.UI;

public class HelicopterController : MonoBehaviour
{
    public AudioSource HelicopterSound;
   // public ControlPanel ControlPanel;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;

    public float TurnForce = 3f;
    public float ForwardForce = 10f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
    public float EffectiveHeight = 200f;

    public float turnTiltForcePercent = 1.5f;
    public float turnForcePercent = 1.3f;

    private float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            MainRotorController.RotarSpeed = value * 80;
            SubRotorController.RotarSpeed = value * 40;
            HelicopterSound.pitch = Mathf.Clamp(value / 40, 0, 1.2f);
            _engineForce = value;
        }
    }

    private Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    private float hTurn = 0f;
    public bool IsOnGround = true;
    
	void Update () {
	}
  
    void FixedUpdate()
    {
        var threshold = 5f;
        float tempY = 0;
        float tempX = 0;

#if UNITY_EDITOR
        EngineForce += Input.GetKey(KeyCode.LeftShift) ? 0.2f : Input.GetKey(KeyCode.Space)? -1f : 0f;
        var rotate = Input.GetKey(KeyCode.LeftArrow) ? -100f : Input.GetKey(KeyCode.RightArrow) ? 100f : 0f;
        var forward = Input.GetKey(KeyCode.Z) ? 100f : Input.GetKey(KeyCode.S) ? -100f : 0f;
        var side = Input.GetKey(KeyCode.Q) ? 100f : Input.GetKey(KeyCode.D) ? -100f : 0f;
#else         
        EngineForce += Input.GetAxis("Vertical") * 0.2f;
        var rotate = Input.GetAxis("RotateRight") * 100f - Input.GetAxis("RotateLeft") * 100f;
        var forward = Input.GetAxis("Forward") * 100f;
        var side = -Input.GetAxis("Side") * 100f;
#endif

        // stable forward
        if (hMove.y > threshold)
            tempY = -Time.fixedDeltaTime;
        else if (hMove.y < -threshold)
            tempY = Time.fixedDeltaTime;

        // stable lurn
        if (hMove.x > threshold)
            tempX = -Time.fixedDeltaTime;
        else if (hMove.x < -threshold)
            tempX = Time.fixedDeltaTime;

        if (Math.Abs(forward) > threshold && !IsOnGround)
        {
            if (forward > threshold)
                tempY = Time.fixedDeltaTime;
            else
                tempY = -Time.fixedDeltaTime;

            hMove.y += tempY;
            hMove.y = Mathf.Clamp(hMove.y, -1, 1);
        }

        if (Math.Abs(side) > threshold && !IsOnGround)
        {
            if (side > threshold)
                tempX = -Time.fixedDeltaTime;
            else
                tempX = Time.fixedDeltaTime;

            hMove.x += tempX;
            hMove.x = Mathf.Clamp(hMove.x, -1, 1);
        }

        if (Math.Abs(rotate) > threshold && !IsOnGround) { 
            var force = 0f;
            if (rotate > threshold && !IsOnGround)
                force = (turnForcePercent - Mathf.Abs(hMove.y)) * HelicopterModel.mass;
            else
                force = -(turnForcePercent - Mathf.Abs(hMove.y)) * HelicopterModel.mass;
            HelicopterModel.AddRelativeTorque(0f, force, 0);
        }
        
        LiftProcess();
        MoveProcess();
        TiltProcess();
    }

    private void MoveProcess()
    {
        var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
        HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass, 0f);
        HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass));
    }

    private void LiftProcess()
    {
        var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
        upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
        HelicopterModel.AddRelativeForce(Vector3.up * upForce);
    }

    private void TiltProcess()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
        HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

    private void OnCollisionEnter()
    {
        IsOnGround = true;
    }

    private void OnCollisionExit()
    {
        IsOnGround = false;
    }
}