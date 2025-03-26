using System.Collections.Generic;
using _ProjectAssets.Scripts.Instances;
using _ProjectAssets.Scripts.Structures;

namespace _ProjectAssets.Scripts.FSM.States_Infrastructure
{
    public class StateTransitionContext
    {
        private List<List<ArrayPositionData>> _verticalMatchesList = new List<List<ArrayPositionData>>();
        private List<List<ArrayPositionData>> _horizontalMatchesList = new List<List<ArrayPositionData>>();

        private MatchElement _movedElement;
        private MatchElement _targetedElement;

        public List<List<ArrayPositionData>> VerticalMatches => _verticalMatchesList;
        public List<List<ArrayPositionData>> HorizontalMatches => _horizontalMatchesList;
        
        public MatchElement TargetedElement => _targetedElement;
        public MatchElement MovedElement => _movedElement;

        public int ComboAmount { get; set; }

        public void CacheStackedMatches(List<List<ArrayPositionData>> vertical, List<List<ArrayPositionData>> horizontal)
        {
            _verticalMatchesList = vertical;
            _horizontalMatchesList = horizontal;
        }

        public void CacheSwappedMatches(MatchElement movedMatch, MatchElement targetedMatch)
        {
            _movedElement = movedMatch;
            _targetedElement = targetedMatch;
        }

        public void ClearCachedSwapMatches()
        {
            _movedElement = null;
            _targetedElement = null;
        }
    }
}