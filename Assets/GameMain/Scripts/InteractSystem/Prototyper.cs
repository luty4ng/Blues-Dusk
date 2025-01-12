using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityGameKit.Runtime;
using GameKit.Element;
using GameKit.Timer;
using GameKit.QuickCode;
using DG.Tweening;

public class Prototyper : MonoBehaviour
{
    public static Vector3 MAGIC_INTERACTIVE_POS = Vector3.zero;
    public static Vector3 MAGIC_TARGET_POS = Vector3.zero;
    public static float MAGIC_ANGLE = 0f;
    public float pressUpdateInterval = 0.2f;
    public float suddenStopDistance = 0.1f;
    public float RoateSpeed = 5;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private Ticker_Auto ticker;
    private Vector3 m_CachedTargetPos;
    private Vector3 m_CachedInteractivePos;
    private float m_CachedAngle;
    private GameElementBase m_CachedInteractive;
    private LayerMask layerMask;
    public bool IsManuallyRoatating = false;

    private void Start()
    {
        m_CachedTargetPos = MAGIC_TARGET_POS;
        m_CachedInteractivePos = MAGIC_INTERACTIVE_POS;
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        layerMask = LayerMask.GetMask("Interactive") | LayerMask.GetMask("Navigation");
        ticker = new Ticker_Auto(pressUpdateInterval);
        ticker.Register(MoveToDestination);
        ticker.Start();
        ticker.Pause();
    }
    void Update()
    {
        navMeshAgent.updateRotation = !IsManuallyRoatating;

        if (!navMeshAgent.updateRotation && m_CachedAngle == MAGIC_ANGLE && m_CachedTargetPos != MAGIC_TARGET_POS)
        {
            Vector2 faceDirection = m_CachedTargetPos.ToVector2() - transform.position.ToVector2();
            m_CachedAngle = Mathf.Abs(Vector3.Angle(transform.forward.ToVector2(), faceDirection));
        }

        if (m_CachedAngle != MAGIC_ANGLE)
        {
            float roateMultipier = Mathf.InverseLerp(0, 180, m_CachedAngle);
            RotateTo(m_CachedTargetPos.IgnoreY(), roateMultipier * 2f);
            Vector2 faceDirection = m_CachedTargetPos.ToVector2() - transform.position.ToVector2();
            float angle = Mathf.Abs(Vector3.Angle(transform.forward.ToVector2(), faceDirection));
            if (angle < 5)
            {
                m_CachedAngle = MAGIC_ANGLE;
            }
        }

        if (navMeshAgent.velocity == Vector3.zero)
        {
            if (m_CachedInteractivePos != MAGIC_INTERACTIVE_POS)
            {
                RotateTo(m_CachedInteractivePos.IgnoreY());
                Vector2 faceDirection = m_CachedInteractivePos.ToVector2() - transform.position.ToVector2();
                float angle = Vector3.Angle(transform.forward.ToVector2(), faceDirection);
                if (angle <= 5f)
                {
                    m_CachedInteractivePos = MAGIC_INTERACTIVE_POS;
                    m_CachedInteractive?.OnInteract();
                }
            }

        }
        animator.SetFloat("VelocityX", Mathf.Abs(navMeshAgent.velocity.x));
        animator.SetFloat("VelocityY", Mathf.Abs(navMeshAgent.velocity.y));

        if (InputManager.instance.GetWorldMouseButtonDown(0) && navMeshAgent != null)
        {
            MoveToDestination();
        }
        // else
        // {

        //     if (InputManager.instance.GetWorldMouseButton(0))
        //     {
        //         Debug.Log($"Keep Press Mouse Button");
        //         ticker.Resume();
        //     }

        //     if (InputManager.instance.GetWorldMouseButtonUp(0))
        //     {
        //         ticker.Pause();
        //     }
        // }
    }

    private void MoveToDestination()
    {
        RaycastHit hitInfo = CursorSystem.current.GetHitInfo(layerMask);
        GameElementBase interactive = CursorSystem.current.GetComponentFromRaycast<GameElementBase>(hitInfo);
        if (interactive == null)
        {
            Vector3 pos = CursorSystem.current.GetPositionFromRaycast(hitInfo);
            if (pos != CursorSystem.MAGIC_POS)
            {
                navMeshAgent.destination = pos;
                m_CachedTargetPos = pos;
            }
        }
        else
        {
            navMeshAgent.destination = interactive.InteractPosition;
            navMeshAgent.velocity = Vector3.one.IgnoreY() * 0.0001f; // 提供基础初速度，避免跳过速度检测
            m_CachedInteractivePos = interactive.transform.position;
            m_CachedInteractive = interactive;
            m_CachedTargetPos = interactive.InteractPosition;
        }
    }

    private void RotateTo(Vector3 position, float roateMultipier = 1)
    {
        Quaternion quaternion = Quaternion.LookRotation(position - transform.position);
        Quaternion clampQuaternion = Quaternion.Euler(quaternion.eulerAngles.IgnoreX().IgnoreZ());
        this.transform.rotation = Quaternion.Lerp(transform.rotation, clampQuaternion, RoateSpeed * roateMultipier * Time.deltaTime);
    }

    public void SetTransform(Transform trans)
    {
        if (trans == null)
        {
            Log.Fatal("Start Tranform for player is null.");
            return;
        }
        this.transform.position = new Vector3(trans.position.x, this.transform.position.y, trans.position.z);
        this.transform.rotation = trans.rotation;
    }

    private void OnDrawGizmos()
    {
        Vector2 faceDirection = m_CachedInteractivePos.ToVector2() - transform.position.ToVector2();
        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position, this.transform.position + transform.forward * 4);
        Gizmos.color = Color.red;
        if (m_CachedInteractivePos != MAGIC_INTERACTIVE_POS)
            Gizmos.DrawLine(this.transform.position, m_CachedInteractivePos.IgnoreY());
        Gizmos.color = Color.blue;
        if (m_CachedTargetPos != MAGIC_TARGET_POS)
            Gizmos.DrawLine(this.transform.position, m_CachedTargetPos.IgnoreY());
    }
}
