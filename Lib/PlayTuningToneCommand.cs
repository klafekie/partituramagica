#region usings

using System.Media;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib
{
	public class PlayTuningToneCommand : ICommand
	{
		public bool CanExecute()
		{
			return true;
		}

		public void Execute()
		{
			new SoundPlayer("440Hz_44100Hz_16bit_05sec_lower.wav").Play();
		}
	}
}