using _ProjectAssets.Scripts.FSM;
using Zenject;

namespace _ProjectAssets.Scripts.Game_States
{
    public class CheckMatchesState : GameState
    {
        
        private GridView _gridView;
        private FSMachine _fsm;
        
        [Inject]
        public void Construct(GridView gridView, FSMachine fsm)
        {
            _gridView = gridView;
            _fsm = fsm;
        }
        public override void Enter()
        {
            StartCheckProcedure();
        }

        public override void Exit()
        {
            
        }

        public override void SetNextState()
        {
            
        }
        
        private void StartCheckProcedure()
        {
            //for
        }
    }
}