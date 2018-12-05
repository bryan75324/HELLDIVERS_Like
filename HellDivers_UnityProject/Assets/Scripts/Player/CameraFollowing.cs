using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

[RequireComponent(typeof(Camera))]
public class CameraFollowing : MonoBehaviour
{
    [Header("== Border ==")]
    [SerializeField] BoxCollider m_box1;
    [SerializeField] BoxCollider m_box2;
    [SerializeField] BoxCollider m_box3;
    [SerializeField] BoxCollider m_box4;

    #region Define Inputs

    private string m_InputAim = "Fire2";
    private string m_InputHorizontal = "Horizontal";
    private string m_InputVertical = "Vertical";
    private string m_InputRun = "Run";

    #endregion Define Inputs

    #region Porpeties

    public LinkedList<Transform> Targets { get { return m_Targets; } }

    public float Speed
    {
        get { return m_CamLerp; }
        set
        {
            if (value > 1) m_CamLerp = 1.0f;
            else if (value < 0) m_CamLerp = 0.0f;
            m_CamLerp = value;
        }
    }

    #endregion Porpeties

    #region Private Variable

    private LinkedList<Transform> m_Targets = new LinkedList<Transform>();
    private Camera m_Cam;
    [SerializeField] private float m_CamHeight = 10.0f;
    [SerializeField] private float m_CamRotX = 60.0f;
    [SerializeField] private float m_CamLerp = 0.1f;
    [SerializeField] private float m_CamWalkAdd = 3.0f;
    private float m_CurrentLerp;
    private Vector3 m_Destination;
    private Vector2 m_ExtraVec;
    private bool m_bCreate;

    #endregion Private Variable

    #region Public Function

    /// <summary>
    /// Set focus on the target. And clear all processing targets.
    /// </summary>
    public void FocusOnTarget(Transform t)
    {
        m_Targets.Clear();
        m_Targets.AddFirst(t);
        UpdateDestination();
        this.transform.position = m_Destination;
        CheckToEnable();
    }

    /// <summary>
    /// Add a target.
    /// </summary>
    public void AddTarget(Transform t)
    {
        m_Targets.AddLast(t);
        CheckToEnable();
        if(!m_bCreate) StartCoroutine(CreateBorder());
        Borderable();
    }

    /// <summary>
    /// Remove a target.
    /// </summary>
    public bool RemoveTarget(Transform t)
    {
        if (m_Targets.Remove(t))
        {
            CheckToDisable();
            Borderable();
            return true;
        }
        else return false;
    }

    #endregion Public Function

    #region MonoBehaviour

    // Use this for initialization
    private void Start()
    {
        m_Cam = this.GetComponent<Camera>();
        m_Cam.transform.rotation = Quaternion.Euler(m_CamRotX, 0, 0);
        m_Cam.fieldOfView = 60.0f;
        m_CurrentLerp = m_CamLerp;
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_Targets.Count <= 0) return;

        UpdateDestination();

        if (m_Targets.Count == 1)
        {
            if (Input.GetButton(m_InputAim))
            {
                m_ExtraVec.x = m_Targets.First.Value.forward.x * m_CamWalkAdd;
                m_ExtraVec.y = m_Targets.First.Value.forward.z * m_CamWalkAdd * 0.75f;
                m_CurrentLerp = m_CamLerp;
            }
            else
            {
                m_ExtraVec.x = Input.GetAxis(m_InputHorizontal) * m_CamWalkAdd;
                m_ExtraVec.y = Input.GetAxis(m_InputVertical) * m_CamWalkAdd;

                m_CurrentLerp = (Input.GetButton(m_InputRun)) ? m_CamLerp * 2 : m_CamLerp;

                if (m_ExtraVec.y <= 0) m_ExtraVec.y *= 1.25f;
            }
            AddOnDestination(m_ExtraVec);
        }

        MoveToDestination();

    }

    #endregion MonoBehaviour

    #region Private Function
    private IEnumerator CreateBorder()
    {
        yield return new WaitForSeconds(2);
        DefineBorder( ref m_box1, 1, 0, 2);
        DefineBorder( ref m_box2, 3, 4, 2);
        DefineBorder( ref m_box3, 5, 6, 4);
        DefineBorder( ref m_box4, 7, 6, 0);
        m_bCreate = true;
    }

    private void DefineBorder( ref BoxCollider b, int i, int small, int big)
    {
        Vector3 pos = Vector3.zero;
        Vector3 Leftpos = Vector3.zero;
        Vector3 Rightpos = Vector3.zero;
        if (RayCast(transform, ref pos, i) && RayCast(transform, ref Leftpos, small) && RayCast(transform, ref Rightpos, big))
        {
            if (i == 1)
            {
                pos += Vector3.back * 2;
            }
            Vector3 vec = Rightpos - Leftpos;
            float wide = vec.magnitude;
            float thick = .5f;
            float height = 20;
            b.transform.position = pos;
            b.transform.right = vec;
            pos.Set(wide, height, thick);
            Debug.Log(pos);
            b.size = pos;
        }
    }

    private void Borderable()
    {
        if (m_Targets.Count < 2) { m_box1.enabled = m_box2.enabled = m_box3.enabled = m_box4.enabled = false; }
        else { m_box1.enabled = m_box2.enabled = m_box3.enabled = m_box4.enabled = true; }
    }

    private bool RayCast(Transform t, ref Vector3 pos, int i)
    {
        RaycastHit rh;
        if (Physics.Raycast(t.position, VectorRotate(t, i), out rh, 500f, 1 << LayerMask.NameToLayer("Terrain")))
        {
            pos = rh.point;
            return true;
        }
        return false;
    }

    private Vector3 VectorRotate(Transform t, int i)
    {
        Vector3 target = Vector3.zero;
        switch (i)
        {
            case 0:
                target = t.forward + t.up / Mathf.Sqrt(3) - t.right;
                break;
            case 1:
                target = t.forward + t.up / Mathf.Sqrt(3);
                break;
            case 2:
                target = t.forward + t.up / Mathf.Sqrt(3) + t.right;
                break;
            case 3:
                target = t.forward + t.right;
                break;
            case 4:
                target = t.forward - t.up / Mathf.Sqrt(3) + t.right;
                break;
            case 5:
                target = t.forward - t.up / Mathf.Sqrt(3);
                break;
            case 6:
                target = t.forward - t.up / Mathf.Sqrt(3) - t.right;
                break;
            case 7:
                target = t.forward - t.right;
                break;
        }
        return target;
    }

    private void UpdateDestination()
    {
        if (m_Targets.Count > 1)
        {
            m_Destination.Set(0, 0, 0);
            float maxZ = float.MinValue;
            float minZ = float.MaxValue;
            foreach (Transform target in m_Targets)
            {
                m_Destination += target.position;
                if (target.position.z > maxZ) maxZ = target.position.z;
                if (target.position.z < minZ) minZ = target.position.z;
            }
            m_Destination /= m_Targets.Count;
            m_Destination.z -= (maxZ - minZ) * 0.17f;
        }
        else
        {
            m_Destination = m_Targets.First.Value.position;
        }

        m_Destination.y += m_CamHeight;
        m_Destination.z += -Mathf.Tan((90 - m_CamRotX) * Mathf.Deg2Rad) * (m_CamHeight - 1f);
    }

    private void AddOnDestination(Vector2 vec)
    {
        m_Destination.x += vec.x;
        m_Destination.z += vec.y;
    }

    private void MoveToDestination()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, m_Destination, m_CurrentLerp);
    }

    private void CheckToEnable()
    {
        if (this.enabled) return;
        if (m_Targets.Count > 0) this.enabled = true;
    }

    private void CheckToDisable()
    {
        if (this.enabled == false) return;
        if (m_Targets == null || m_Targets.Count == 0) this.enabled = false;
    }

    #endregion Private Function

#if UNITY_EDITOR

    #region Debug Draw

    public bool m_ShowDebugDraw;

    private void OnDrawGizmos()
    {
        if (m_ShowDebugDraw == false) return;
        if (m_Targets.First == null) return;

        Vector3 pos = m_Targets.First.Value.position;
        pos.y += 1.0f;
        targetOnScreen = m_Cam.WorldToScreenPoint(pos);

        screenBottom.Set(targetOnScreen.x, 0, targetOnScreen.z);
        DrawTestLine(screenBottom, pos);

        screenTop.Set(targetOnScreen.x, m_Cam.pixelHeight, targetOnScreen.z);
        DrawTestLine(screenTop, pos);

        screenLeft.Set(0.0f, targetOnScreen.y, targetOnScreen.z);
        DrawTestLine(screenLeft, pos);

        screenRight.Set(m_Cam.pixelWidth, targetOnScreen.y, targetOnScreen.z);
        DrawTestLine(screenRight, pos);
    }

    private void DrawTestLine(Vector3 border, Vector3 agent)
    {
        border = m_Cam.ScreenToWorldPoint(border);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(border, agent);

        float dist = Vector3.Distance(border, agent);
        Vector3 center = (border + agent) * 0.5f;
        Handles.Label(center, dist.ToString("0.00"));
    }

    private Vector3 targetOnScreen;
    private Vector3 screenBottom;
    private Vector3 screenTop;
    private Vector3 screenLeft;
    private Vector3 screenRight;

    #endregion Debug Draw

#endif
}