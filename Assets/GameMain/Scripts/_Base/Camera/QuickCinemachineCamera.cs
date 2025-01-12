using System.Collections;
using UnityEngine;
using UnityGameKit.Runtime;
using Cinemachine;
using DG.Tweening;

public class QuickCinemachineCamera : MonoSingletonBase<QuickCinemachineCamera>
{
    public CinemachineVirtualCamera m_VirtualCamera;
    public Vector3 DefaultFollowPositionOffset;
    public Vector3 DefaultRotation;
    public Vector3 DialogRotation;
    public Transform FocusTransform;
    private Transform m_CachedPlayerTransform;
    private float DefaultOrthographicSize = 3f;

    protected override void Awake()
    {
        base.Awake();
        DefaultOrthographicSize = m_VirtualCamera.m_Lens.OrthographicSize;
    }

    private void OnDisable()
    {
        m_VirtualCamera.m_Lens.OrthographicSize = DefaultOrthographicSize;
    }

    private bool FollowPlayer(Transform transform)
    {
        if (m_VirtualCamera == null)
        {
            Log.Fail("CinemachineVirtualCamera reference is null.");
            return false;
        }
        m_VirtualCamera.Follow = transform;
        m_CachedPlayerTransform = transform;
        return true;
    }

    public void SetFollowPlayer(Transform transform)
    {
        m_VirtualCamera.ForceCameraPosition(transform.position - DefaultFollowPositionOffset, DefaultRotation.ToQuaternion());
        FollowPlayer(transform);
    }

    public void SetFocus(Vector3 position, bool isShrink = true)
    {
        FocusTransform.position = position;
        m_VirtualCamera.Follow = FocusTransform;
        StopAllCoroutines();
        StartCoroutine(FocusProcess(m_VirtualCamera.m_Lens.OrthographicSize, 2f, isShrink));
    }

    public void SetDialogFocus(Vector3 position, bool isShrink = true)
    {
        FocusTransform.position = position;
        m_VirtualCamera.Follow = FocusTransform;
        
        StopAllCoroutines();
        m_VirtualCamera.transform.DOKill();
        
        StartCoroutine(FocusProcess(m_VirtualCamera.m_Lens.OrthographicSize, 2f, isShrink));
        m_VirtualCamera.transform.DORotate(DialogRotation, 0.5f);
    }

    public void ResetFocus()
    {
        if (m_CachedPlayerTransform == null)
        {
            Log.Fail("m_DefaultTargetTransform is null");
            return;
        }
        Log.Success("Reset Focus to {0}", m_CachedPlayerTransform.gameObject.name);
        FollowPlayer(m_CachedPlayerTransform);
        StopAllCoroutines();
        m_VirtualCamera.transform.DOKill();

        StartCoroutine(ResetFocusProcess(DefaultOrthographicSize, 2f));
        m_VirtualCamera.transform.DORotate(DefaultRotation, 0.5f);
    }

    IEnumerator FocusProcess(float size, float speed, bool isShrink)
    {
        if (isShrink)
        {
            while (m_VirtualCamera.m_Lens.OrthographicSize >= 0.75f * size)
            {
                m_VirtualCamera.m_Lens.OrthographicSize -= speed * Time.deltaTime;
                yield return null;
            }
            
            m_VirtualCamera.m_Lens.OrthographicSize = 0.75f * size;
        }
        else
        {
            while (m_VirtualCamera.m_Lens.OrthographicSize <= 1.25f * size)
            {
                m_VirtualCamera.m_Lens.OrthographicSize += speed * Time.deltaTime;
                yield return null;
            }
            
            m_VirtualCamera.m_Lens.OrthographicSize = 1.25f * size;
        }
        
        //Debug.Log("Focus End.");
    }

    IEnumerator ResetFocusProcess(float size, float speed)
    {
        if (m_VirtualCamera.m_Lens.OrthographicSize >= size)
        {
            while (m_VirtualCamera.m_Lens.OrthographicSize >= size)
            {
                m_VirtualCamera.m_Lens.OrthographicSize -= speed * Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (m_VirtualCamera.m_Lens.OrthographicSize < size)
            {
                m_VirtualCamera.m_Lens.OrthographicSize += speed * Time.deltaTime;
                yield return null;
            }
        }

        m_VirtualCamera.m_Lens.OrthographicSize = size;

        //Debug.Log("Refocus End.");
    }
}