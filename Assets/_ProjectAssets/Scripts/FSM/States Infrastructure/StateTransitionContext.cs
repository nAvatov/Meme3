using System.Collections.Generic;
using _ProjectAssets.Scripts.Structures;

namespace _ProjectAssets.Scripts.FSM.States_Infrastructure
{
    public class StateTransitionContext
    {
        private List<List<ArrayPositionData>> _verticalMatchesList = new List<List<ArrayPositionData>>();
        private List<List<ArrayPositionData>> _horizontalMatchesList = new List<List<ArrayPositionData>>();

        public List<List<ArrayPositionData>> VerticalMatches => _verticalMatchesList;
        public List<List<ArrayPositionData>> HorizontalMatches => _horizontalMatchesList;

        public int ComboAmount { get; set; }

        public void SetMatches(List<List<ArrayPositionData>> vertical, List<List<ArrayPositionData>> horizontal)
        {
            _verticalMatchesList = vertical;
            _horizontalMatchesList = horizontal;
        }
    }
}