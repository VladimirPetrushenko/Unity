using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum MOVETYPE { IDLE = 0, WALK = 1, BOXPUSH = 2, CHEER = 3 };
    public enum SPACESTATE { EMPTY = 0, CRATE = 1, COLLIDER = 2 }
    // Player states
    private MOVETYPE State = MOVETYPE.IDLE;
    // время в секундах для передвижения
    public float MoveTime = 1.0f;
    // время в секундах для поворота
    public float RoteTime = 1.0f;
    // скорость передвижения
    public float MoveDistance = 2.0f;
    // скорость передвижения
    public float RoteDistance = 90.0f;
    // cached transform
    private Transform ThisTransform = null;
    private Transform LastBox = null;
    // Reference to animator
    private Animator AnimComp = null;
    private Collider[] Colliders = null;

    public Transform LeftHandDest = null;
    public Transform RightHandDest = null;
    public MOVETYPE PlayerState
    {
        get { return State; }
        set 
        { 
            State = value;
            AnimComp.SetInteger("iState", (int) State);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        ThisTransform = GetComponent<Transform>();
        AnimComp = GetComponent<Animator>();
        StartCoroutine(HandleInput());
        Colliders = Object.FindObjectsOfType<Collider>();
    }

    public IEnumerator HandleInput()
    {
        GameMenagger GM = GameMenagger.Instance;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                GM.RestartGame();
                yield break;
            }

            if (Mathf.CeilToInt(Input.GetAxis("Vertical")) > 0)
            {
                PlayerState = ValidateWalk();
                if(PlayerState != MOVETYPE.IDLE)
                    yield return StartCoroutine(Move(MoveDistance));
            }
            else
                PlayerState = MOVETYPE.IDLE;

            if (Input.GetAxis("Vertical") < 0)
            {
                float RotMove = RoteTime;
                RoteTime = 0.2f;
                yield return StartCoroutine(Rotate(RoteDistance));
                yield return StartCoroutine(Rotate(RoteDistance));
                RoteTime = RotMove;
            }


            if (Input.GetAxis("Horizontal") > 0)
                yield return StartCoroutine(Rotate(RoteDistance));
            if (Input.GetAxis("Horizontal")<0)
                yield return StartCoroutine(Rotate(-RoteDistance));
            
            yield return null;
        }
    }
    public IEnumerator Move(float Increment = 0)
    {
        Vector3 startPosition = ThisTransform.position;
        Vector3 destPosition = ThisTransform.position + ThisTransform.forward * Increment;
        float ElapsedTime = 0.0f;
        while (ElapsedTime < MoveTime)
        {
            Vector3 finalPosition = Vector3.Lerp(startPosition, destPosition, ElapsedTime / MoveTime);
            ThisTransform.position = finalPosition;
            if (PlayerState == MOVETYPE.BOXPUSH)
                LastBox.position = new Vector3(ThisTransform.position.x, LastBox.position.y, ThisTransform.position.z) + ThisTransform.forward * Increment;
            yield return null;
            ElapsedTime += Time.deltaTime;
        }
        ThisTransform.position = destPosition;
        if (PlayerState == MOVETYPE.BOXPUSH)
            LastBox.position = new Vector3(ThisTransform.position.x, LastBox.position.y, ThisTransform.position.z) + ThisTransform.forward * Increment;

        yield break;
    }
    public IEnumerator Rotate(float Increment = 0)
    {
        float startRotation = ThisTransform.rotation.eulerAngles.y;
        float destRotation = startRotation + Increment;
        float ElapsedTime = 0.0f;
        while (ElapsedTime < RoteTime)
        {
            float Angle = Mathf.LerpAngle(startRotation, destRotation, ElapsedTime / RoteTime);
            ThisTransform.eulerAngles = new Vector3(0, Angle, 0);
            yield return null;
            ElapsedTime += Time.deltaTime;
        }
        ThisTransform.eulerAngles = new Vector3(0, Mathf.FloorToInt(destRotation), 0);
        yield break;
    }
    public SPACESTATE Point(Vector3 point)
    {
        foreach(Collider c in Colliders)
        {
            if (c.bounds.Contains(point) && !c.gameObject.CompareTag("End"))
            {
                if (c.gameObject.CompareTag("Crate"))
                {
                    LastBox = c.gameObject.transform;
                    return SPACESTATE.CRATE;
                }
                else
                {
                    return SPACESTATE.COLLIDER;
                }
            }
        }
        return SPACESTATE.EMPTY;
    }
    public MOVETYPE ValidateWalk()
    {
        LastBox = null;
        Vector3 destPosition = ThisTransform.position + ThisTransform.forward * MoveDistance;
        Vector3 dbldestPosition = ThisTransform.position + ThisTransform.forward * MoveDistance * 2.0f;
        SPACESTATE nextStatus = Point(destPosition);
        Transform nextBox = LastBox;
        SPACESTATE dblnextStatus = Point(dbldestPosition);
        LastBox = nextBox;
        if(nextStatus == SPACESTATE.EMPTY)
        {
            return MOVETYPE.WALK;
        }
        if(nextStatus == SPACESTATE.CRATE && dblnextStatus == SPACESTATE.EMPTY)
        {
            return MOVETYPE.BOXPUSH;
        }
        return MOVETYPE.IDLE;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (PlayerState == MOVETYPE.BOXPUSH)
        {
            AnimComp.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.5f);
            AnimComp.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.5f);
            AnimComp.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.5f);
            AnimComp.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0.5f);
            AnimComp.SetIKPosition(AvatarIKGoal.RightHand, RightHandDest.position);
            AnimComp.SetIKRotation(AvatarIKGoal.RightHand, RightHandDest.rotation);
            AnimComp.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandDest.position);
            AnimComp.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandDest.rotation);
        }
        else
        {
            AnimComp.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
            AnimComp.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
            AnimComp.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            AnimComp.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
        }
    }
}
