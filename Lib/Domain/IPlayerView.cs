using System;
using System.Collections.Generic;

namespace PartituraMagica
{
    /** 
     * <param name="cancel">implementors can prevent the view from scrolling to the next line by setting this to true"</param>
     * <param name="line" summary="index of the line that has reached its end"/>
     */

    public delegate void LineEndEvent(int line, ref bool cancel);


    /** 
     * represents a view that can display (and scroll) a continuous
     * "stream" of musical lines (clippings)
     */

    public interface IPlayerView : IDisposable
    {
        double XPos { get; }

        /**
         * returns an instance that knows the currently played part.
         */
        IPartNavigator PartNavigator { get; }
        /** 
         * Initialize the views graphical objects using the given clippings
         */
        void Init(string partPattern, IEnumerable<Clipping> clippings);

        void GotoLine(int lineIndex);

        void StartScrolling();

        /**
         * Event triggered when the view has scrolled until the end of a line.
         */
        event LineEndEvent LineEndReached;

        PlayerMemento ToMemento();
    }
}