using System;

namespace PartituraMagica
{
    public delegate void CancellableEvent(ref bool cancel);

    /**
     * A user interface element that allows the user to choose from
     * a (small) set of available parts.
     * Parts are represented by strings and the current value
     * is an index
     */

    public interface IPartNavigator
    {
        string[] Parts { get; set; }
        int Current { get; set; }
        event Action<int> PartChanged;
        event Action<int> PartClicked;
        /** going to previous part can be prevented by listeners
         * by setting the cancel parameter to true
         * In this case the same part is "clicked" again
         */
        event CancellableEvent PreviousPartClicked;
        event CancellableEvent NextPartClicked;
    }
}