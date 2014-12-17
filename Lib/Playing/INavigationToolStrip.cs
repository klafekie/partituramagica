using System;
namespace PartituraMagica
{
	public delegate void CancellableEvent(ref bool cancel);

	public interface IChoiceNavigator
	{
		event Action<int> ChoiceChanged;
		event Action<int> ChoiceClicked;
		event CancellableEvent PreviousChoiceClicked;
		event CancellableEvent NextChoiceClicked;
		string[] Choices { get; set; }
		int Current { get; set; }
	}
}
