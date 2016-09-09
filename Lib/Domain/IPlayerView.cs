using System;
using System.Collections.Generic;

namespace PartituraMagica
{
    /** 
     * represents a view that can display (and scroll) a continuous
     * "stream" of musical lines (clippings)
     */

    public interface IPlayerView : IDisposable
    {
        double XPos { get; set; }

        /** 
         * Initialize the views graphical objects using the given clippings
         */
        void Init(string partPattern, IEnumerable<Clipping> clippings, double lineDistance);

        void StartScrolling();

        /**
         * Event triggered when the view has scrolled forward. 
         * It is NOT fired when the position is changed by navigation.
         * 
         * Take care that event handlers to nothing that takes much time!
         */
        event Action ViewHasScrolled;

        PlayerMemento ToMemento();
        
        void RegisterEvents(IPartNavigator navigator);
    }
}