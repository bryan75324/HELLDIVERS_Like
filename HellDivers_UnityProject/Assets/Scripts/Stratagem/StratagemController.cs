using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundManager))]
[RequireComponent(typeof(CheckCodesMechine))]
public class StratagemController : MonoBehaviour
{
    #region Properties

    /// <summary>
    /// Was any stratagem has been ready ?
    /// </summary>
    public bool IsReady { get { return m_CurrentStratagem != false; } }

    /// <summary>
    /// Was the checking codes process has been actived ?
    /// </summary>
    public bool IsCheckingCode { get { return m_CheckCodesMechine.IsChecking; } }

    /// <summary>
    /// Represent of the stratagem is ready.
    /// </summary>
    public Stratagem CurrentStratagem { get { return m_CurrentStratagem; } }

    /// <summary>
    /// Represent of stratagems in the controller.
    /// </summary>
    public List<Stratagem> Stratagems { get { return m_Stratagems; } }

    /// <summary>
    /// Represent of stratagems check codes behavior mechine.
    /// </summary>
    public CheckCodesMechine CheckCodesMechine { get { return m_CheckCodesMechine; } }

    /// <summary>
    /// Represent of stratagems sound manager.
    /// </summary>
    public SoundManager SoundManager { get { return m_SoundManager; } }

    /// <summary>
    /// Represent of current checking code input step.
    /// </summary>
    public int InputCodeStep { get { return m_CheckCodesMechine.Step; } }

    /// <summary>
    /// Represent of the throw out force scale.
    /// [Range( 0 , MaxScaleForce )]
    /// </summary>
    public float ScaleThrowForce
    {
        get { return m_ScaleForce; }
        private set
        {
            if (value > StratagemSystem.MaxScaleThrowForce) m_ScaleForce = StratagemSystem.MaxScaleThrowForce;
            else m_ScaleForce = value;
        }
    }

    #endregion Properties

    #region Event

    public delegate void EventHolder();

    public event EventHolder OnGetReady;

    public event EventHolder OnThrow;

    #endregion Event

    #region Private Variable

    [SerializeField] private List<Stratagem> m_Stratagems = new List<Stratagem>();
    [SerializeField] private Vector3 m_ThrowForce = new Vector3(0.0f, 300.0f, 500.0f);
    private float m_ScaleForce = 1;
    private Stratagem m_CurrentStratagem;
    private CheckCodesMechine m_CheckCodesMechine;
    private SoundManager m_SoundManager;

    #endregion Private Variable

    #region MonoBehaviour

    private void Awake()
    {
        m_SoundManager = this.GetComponent<SoundManager>();
        SoundDataSetting data = ResourceManager.m_Instance.LoadData(typeof(SoundDataSetting), "Sounds/StratagemController", "SoundDataSetting") as SoundDataSetting;
        m_SoundManager.SetAudioClips(data.SoundDatas);

        m_CheckCodesMechine = this.GetComponent<CheckCodesMechine>();
        m_CheckCodesMechine.OnGetResult += GetReady;
        m_CheckCodesMechine.OnChecking += () => m_SoundManager.PlayInWorld(2001, this.transform.position);
    }

    private void OnDestroy()
    {
        m_CheckCodesMechine.OnGetResult -= GetReady;
    }

    #endregion MonoBehaviour

    #region Public Function

    #region Management

    /// <summary>
    /// Add a stratagem by id key.
    /// If the id already in the controller will be pass.
    /// </summary>
    /// <param name="id">The id key which in the gamedata.stratagem table.</param>
    /// <param name="launchPos">The spawn transform root.</param>
    public bool AddStratagem(int id, Transform readyPos, Transform launchPos)
    {
        for (int i = 0; i < m_Stratagems.Count; i++)
        {
            if (m_Stratagems[i].Info.ID == id) return false;
        }

        Stratagem stratagem;
        GameObject stratagemGo;

        string name = string.Format("Stratagem{0}", id);
        stratagemGo = new GameObject(name);
        stratagem = stratagemGo.AddComponent<Stratagem>();

        stratagemGo.SetActive(true);
        stratagem.SetStratagemInfo(id, readyPos, launchPos);
        m_Stratagems.Add(stratagem);
        return true;
    }

    /// <summary>
    /// Add a multi stratagems by id key.
    /// If the id already in the controller will be pass.
    /// </summary>
    /// <param name="ids">The id key which in the gamedata.stratagem table.</param>
    /// <param name="launchPos">The spawn transform root.</param>
    public void AddStratagems(List<int> ids, Transform readyPos, Transform launchPos)
    {
        foreach (int id in ids)
        {
            AddStratagem(id, readyPos, launchPos);
        }
    }

    /// <summary>
    /// Add a multi stratagems by id key.
    /// If the id already in the controller will be pass.
    /// </summary>
    /// <param name="ids">The id key which in the gamedata.stratagem table.</param>
    /// <param name="launchPos">The spawn transform root.</param>
    public void AddStratagems(int[] ids, Transform readyPos, Transform launchPos)
    {
        foreach (int id in ids)
        {
            AddStratagem(id, readyPos, launchPos);
        }
    }

    /// <summary>
    /// Remove the stratagem by index.
    /// </summary>
    /// <param name="index">Index of the stratagems</param>
    /// <returns></returns>
    public bool RemoveStratagemAt(int index)
    {
        if (index < 0 || index >= m_Stratagems.Count) return false;

        Stratagem target = m_Stratagems[index];
        m_Stratagems.RemoveAt(index);

        DestroyImmediate(target.gameObject);
        return true;
    }

    /// <summary>
    /// Remove the stratagem by index.
    /// </summary>
    /// <param name="id">The id key which in the gamedata.stratagem table.</param>
    /// <returns>If remove succeeful return true</returns>
    public bool RemoveStratagem(int id)
    {
        Stratagem target = null;
        for (int i = 0; i < m_Stratagems.Count; i++)
        {
            if (m_Stratagems[i].Info.ID == id)
            {
                target = m_Stratagems[i];
                break;
            }
        }

        if (target == null) return false;

        m_Stratagems.Remove(target);
        DestroyImmediate(target.gameObject);

        return false;
    }

    /// <summary>
    /// Clean up all stratagems in the controller.
    /// </summary>
    public void Clear()
    {
        if (m_Stratagems.Count <= 0) return;

        for (int i = 0; i < m_Stratagems.Count; i++)
        {
            DestroyImmediate(m_Stratagems[i].gameObject);
        }

        m_Stratagems.Clear();
        m_CurrentStratagem = null;
    }

    /// <summary>
    /// Reset all stratagem uses = 0.
    /// </summary>
    public void Reset()
    {
        foreach (Stratagem stratagem in m_Stratagems)
        {
            stratagem.Reset();
        }
    }

    #endregion Management

    /// <summary>
    /// Start checking stratagem codes.
    /// </summary>
    /// <returns>Was there are any stratagems in the contorller ?</returns>
    public bool StartCheckCodes()
    {
        m_SoundManager.PlayOnce(2000);
        m_CheckCodesMechine.Clear();
        foreach (Stratagem s in m_Stratagems)
        {
            if (s.State == Stratagem.eState.Idle && !s.IsCooling && !s.IsOutOfUses)
            {
                m_CheckCodesMechine.AddCodes(s, s.Info.Codes);
            }
        }
        m_CheckCodesMechine.StartCheckCodes();
        return true;
    }

    /// <summary>
    /// Stop checking stratagem codes.
    /// </summary>
    public void StopCheckCodes()
    {
        m_SoundManager.Stop();
        m_CheckCodesMechine.StopCheckCodes();
    }

    /// <summary>
    /// On check codes mechine get success.
    /// </summary>
    private void GetReady()
    {
        if (m_CheckCodesMechine.Result == null) return;
        m_SoundManager.Stop();
        m_SoundManager.PlayOnce(2002);
        m_CurrentStratagem = m_CheckCodesMechine.Result as Stratagem;
        m_CurrentStratagem.GetReady();

        if (OnGetReady != null) OnGetReady();
    }

    /// <summary>
    /// Throw out the current stratagem.
    /// </summary>
    public void Throw()
    {
        if (IsReady == false) return;

        StopAllCoroutines();
        Vector3 force = m_ThrowForce * ScaleThrowForce;
        m_CurrentStratagem.Throw(force);
        m_CurrentStratagem = null;

        if (OnThrow != null) OnThrow();
    }

    /// <summary>
    /// Start add on throw force.
    /// </summary>
    /// <returns>Was there is stratagem which is ready ?</returns>
    public bool PrepareThrow()
    {
        if (IsReady == false) return false;

        StopAllCoroutines();
        StartCoroutine(ThorwForceAddOn());
        return true;
    }

    #endregion Public Function

    #region Private Function

    /*----------------------------
     * Scale throw force by time *
     ----------------------------*/

    private IEnumerator ThorwForceAddOn()
    {
        ScaleThrowForce = 0;

        while (ScaleThrowForce < StratagemSystem.MaxScaleThrowForce)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            ScaleThrowForce += Time.deltaTime * StratagemSystem.ScaleThorwForceRate;
        }

        yield break;
    }

    #endregion Private Function
}