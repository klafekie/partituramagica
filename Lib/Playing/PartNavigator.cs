using System;
using System.Collections.Generic;

namespace PartituraMagica.Playing
{
    /** 
     * Decides when to change to a new part / lineIndex depending on the users commands.
     */

    public class PartNavigator : IPartNavigator
    {
        private static readonly double MinPartScroll = 50D;
        private readonly PartPositionProvider _calculator;
        private readonly IPlayerView _view;


        public PartNavigator(string partPattern, IList<Part> parts, IPlayerView view)
        {
            _view = view;
            _calculator = new PartPositionProvider(partPattern, parts);
        }

        public int CurrentPartIndex { get; private set; }

        public void FirstPartClicked()
        {
            PartClicked(0);
        }

        public void PreviousPartClicked()
        {
            if (CurrentPartIndex > 0)
            {
                double currentPartStartPos = _calculator.GetPartXPosition(CurrentPartIndex);

                if (_view.XPos < currentPartStartPos + MinPartScroll)
                {
                    CurrentPartIndex--;
                }
                _view.XPos = _calculator.GetPartXPosition(CurrentPartIndex);
            }
        }

        public void PartClicked(int partIndex)
        {
            _view.XPos = _calculator.GetPartXPosition(partIndex);
            CurrentPartIndex = partIndex;
        }

        public void NextPartClicked()
        {
            if (_calculator.HasPartsAfter(CurrentPartIndex))
            {
                CurrentPartIndex++;
                _view.XPos = _calculator.GetPartXPosition(CurrentPartIndex);
            }
        }

        public void LastPartClicked()
        {
            PartClicked(_calculator.GetLastPartIndex());
        }

        public void JumpBackwardClicked()
        {
            //TODO thread safety??
            _view.XPos = Math.Max(_view.XPos - 250d, 0d);
            CurrentPartIndex = _calculator.GetPatternIndexFromXPosition(_view.XPos);
        }

        public void JumpForwardClicked()
        {
            _view.XPos = Math.Min(_view.XPos + 250d, _calculator.GetPartituraEndXPos());
            CurrentPartIndex = _calculator.GetPatternIndexFromXPosition(_view.XPos);
        }

        public void PlayerOpened()
        {
            //TPDO why does view need the partPattern?
            _view.Init(_calculator.PartPattern, _calculator.GetClippings());
            _view.StartScrolling();
        }


        //TODO store lineIndex inside navigator?
        public void LineEndReached(int lineIndex)
        {
            if (_calculator.HasLinesAfter(lineIndex))
            {
                CurrentPartIndex = _calculator.GetPatternIndex(lineIndex + 1);
                _view.XPos = _calculator.GetLineXPosition(lineIndex + 1);
            }
        }
    }
}