using System;
namespace PartituraMagica
{
	public delegate void CancellableEvent(ref bool cancel);

	/**
	 * A user interface element that allows the user to choose from
	 * a (small) set of available choices.
	 * Choices are represented by strings and the current value
	 * is an index
	 */
	public interface IChoiceNavigator
	{
		event Action<int> ChoiceChanged;
		event Action<int> ChoiceClicked;
		/** going to previous choice can be prevented by listeners
		 * by setting the cancel parameter to true
		 * In this case the same choice is "clicked" again
		 */
		event CancellableEvent PreviousChoiceClicked;
		event CancellableEvent NextChoiceClicked;
		string[] Choices { get; set; }
		int Current { get; set; }
	}
}
