﻿using Source.Code.Utils;
using TMPro;
using UnityEngine;
namespace Source.Code.UI
{
    public class TeamVsTeamScore : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI leftScoreTMP;
        [SerializeField] private TextMeshProUGUI rightScoreTMP;
        [SerializeField] private TextMeshProUGUI whoWinsTitleTMP;

        private SessionSettings sessionSettings;

        private void Start()
        {
            sessionSettings = SessionSettings.Instance;
            sessionSettings.Factions[0].ScoresChanged += OnLeftTeamScoreChanged;
            sessionSettings.Factions[1].ScoresChanged += OnRightTeamScoreChanged;
            leftScoreTMP.text = "0";
            rightScoreTMP.text = "0";
            sessionSettings.GlobalState.GlobalStateChanged += OnGlobalStateChanged;
        }

        private void OnGlobalStateChanged(GlobalState.States state)
        {
            switch (state)
            {
                case GlobalState.States.PreGame:
                    break;
                case GlobalState.States.Game:
                    break;
                case GlobalState.States.GameEnded:
                    {
                        bool isBlueWins = sessionSettings.Factions[0].Scores > sessionSettings.Factions[1].Scores;
                        string winnerSideName = isBlueWins ? "Blue" : "Red";
                        whoWinsTitleTMP.color = isBlueWins ? Color.blue : Color.red;
                        whoWinsTitleTMP.text = winnerSideName + " Team wins!";
                        whoWinsTitleTMP.gameObject.SetActive(true);

                        Debug.Log("Winner is " + sessionSettings.GlobalState.factionsSortedByScore[0].ID + " with score " + sessionSettings.GlobalState.factionsSortedByScore[0].Scores);
                        Debug.Log("Loser is " + sessionSettings.GlobalState.factionsSortedByScore[1].ID + " with score " + sessionSettings.GlobalState.factionsSortedByScore[1].Scores);
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnLeftTeamScoreChanged(int score)
        {
            leftScoreTMP.text = score.ToString();
        }

        private void OnRightTeamScoreChanged(int score)
        {
            rightScoreTMP.text = score.ToString();
        }
    }
}