using System.Media;

namespace PartituraMagica
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