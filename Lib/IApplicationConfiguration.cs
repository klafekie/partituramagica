#region usings

#endregion

namespace PartituraMagica.Lib
{
	public interface IApplicationConfiguration
	{
		string LastPartituraPath { get; set; }
		int Color { get; }
		bool InvertColors { get; }
		void Save();
	}
}