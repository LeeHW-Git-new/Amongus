using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameUIManager : MonoBehaviour
{
    public static IngameUIManager Instance;

    [SerializeField]
    private IngameIntroUI ingameIntroUI;
    public IngameIntroUI IngameIntroUI { get { return ingameIntroUI; }}

    [SerializeField]
    private KillButtonUI killButonUI;
    public KillButtonUI KillButtonUI { get { return killButonUI; } }

    [SerializeField]
    private KillUI killUI;
    public KillUI KillUI { get { return killUI; } }

    [SerializeField]
    private ReportButtonUI reportButtonUI;
    public ReportButtonUI ReportButtonUI { get { return reportButtonUI; } }

    [SerializeField]
    private ReportUI reportUI;
    public ReportUI ReportUI { get { return reportUI; } }

    [SerializeField]
    private MeetingUI meetingUI;
    public MeetingUI MeetingUI { get { return meetingUI; } }

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
