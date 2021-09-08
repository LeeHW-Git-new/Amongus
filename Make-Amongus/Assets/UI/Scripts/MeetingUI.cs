using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeetingUI : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPanelPrefab;

    [SerializeField]
    private Transform playerPanelsParent;

    private List<MeetingPlayerPanle> meetingPlayerPanles = new List<MeetingPlayerPanle>();
    public void Open()
    {
        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as IngameCharacterMover;
        var myPanel = Instantiate(playerPanelPrefab, playerPanelsParent).GetComponent<MeetingPlayerPanle>();
        myPanel.SetPlayer(myCharacter);
        meetingPlayerPanles.Add(myPanel);

        gameObject.SetActive(true);

        var players = FindObjectsOfType<IngameCharacterMover>();
        foreach(var player in players)
        {
            if(player != myCharacter)
            {
                var panel = Instantiate(playerPanelPrefab, playerPanelsParent).GetComponent<MeetingPlayerPanle>();
                panel.SetPlayer(player);
                meetingPlayerPanles.Add(panel);
            }
        }
    }
    public void SelectPlayerPanel()
    {
        foreach(var panel in meetingPlayerPanles)
        {
            panel.Unselect();
        }
    }
}
