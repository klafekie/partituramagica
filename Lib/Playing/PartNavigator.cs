using System;
using System.Collections.Generic;

namespace PartituraMagica.Playing
{
    /** 
     * Decides when to change to a new part / lineIndex depending on the users commands.
     */

    public class PartNavigator : IPartNavigator
    {
        private static readonly double MinPartScroll = 150D;
        private readonly PartPositionProvider _calculator;
        private readonly IPlayerView _view;
        private int _currentPartIndex;

        public PartNavigator(string partPattern, IList<Part> parts, IPlayerView view)
        {
            _view = view;
            _view.ViewHasScrolled += ViewOnViewHasScrolled;
            _calculator = new PartPositionProvider(partPattern, parts);
        }


        public int CurrentPartIndex
        {
            get { return _currentPartIndex; }
            private set
            {
                bool changed = _currentPartIndex != value;
                _currentPartIndex = value;
                if (changed)
                {
                    CurrentPartIndexChanged?.Invoke();
                }
            }
        }

        public int PartCount => _calculator.GetLastPatternIndex() + 1;

        public event Action CurrentPartIndexChanged;

        public void FirstPartClicked()
        {
            CurrentPartIndex = 0;
            _view.XPos = _calculator.GetPatternXPosition(0);
        }

        public void PreviousPartClicked()
        {
            if (CurrentPartIndex > 0)
            {
                double currentPartStartPos = _calculator.GetPatternXPosition(CurrentPartIndex);

                if (_view.XPos < currentPartStartPos + MinPartScroll)
                {
                    CurrentPartIndex--;
                }
                _view.XPos = _calculator.GetPatternXPosition(CurrentPartIndex);
            }
        }

        public void PartClicked(int partIndex)
        {
            CurrentPartIndex = partIndex;
            _view.XPos = _calculator.GetPatternXPosition(partIndex);
        }

        public void NextPartClicked()
        {
            if (_calculator.PatternHasPartsAfter(CurrentPartIndex))
            {
                CurrentPartIndex++;
                _view.XPos = _calculator.GetPatternXPosition(CurrentPartIndex);
            }
        }

        public void LastPartClicked()
        {
            CurrentPartIndex = _calculator.GetLastPatternIndex();
            _view.XPos = _calculator.GetPatternXPosition(CurrentPartIndex);
        }

        public void JumpBackwardClicked()
        {
            //TODO thread safety??
            double viewXPos = Math.Max(_view.XPos - 250d, 0d);
            CurrentPartIndex = _calculator.GetPatternIndexFromXPosition(viewXPos);
            _view.XPos = viewXPos;
        }

        public bool IsPartituraFinished()
        {
            var patternEndXPos = _calculator.GetPatternEndXPos();
            return _view.XPos >= patternEndXPos;
        }

        public void JumpForwardClicked()
        {
            double viewXPos = Math.Min(_view.XPos + 250d, _calculator.GetPatternEndXPos());
            CurrentPartIndex = _calculator.GetPatternIndexFromXPosition(_view.XPos);
            _view.XPos = viewXPos;
        }

        private void ViewOnViewHasScrolled()
        {
            //fires part changed event if really changed
            CurrentPartIndex = _calculator.GetPatternIndexFromXPosition(_view.XPos);
        }

        public void PlayerOpened()
        {
            _view.Init(_calculator.PartPattern, _calculator.GetClippings(), PartPositionProvider.LineDistance);
            _view.StartScrolling();
        }
    }
}