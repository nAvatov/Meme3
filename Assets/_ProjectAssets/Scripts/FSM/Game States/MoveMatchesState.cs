using UnityEngine;

namespace _ProjectAssets.Scripts.FSM.Game_States
{
    public class MoveMatchesState : GameState
    {
        public override void Enter()
        {
            Debug.Log("Time to move elements!");
        }
    }
}