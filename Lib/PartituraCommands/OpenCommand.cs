#region usings

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica.PartituraCommands
{
	public class OpenCommand : PartituraCommandBase, ICommand
	{
		private readonly string _path;

		public OpenCommand(ICommandParameters p)
			: base(p)
		{
			_path = null;
		}

		//for testing purposes
		public OpenCommand(string path, ICommandParameters p)
			: base(p)
		{
			_path = path;
		}

		public void Execute()
		{
			string chosenFile = _path ?? P.FileSystem.ShowOpenPartituraDialog();
			if (chosenFile != null)
			{
				new ClosePartituraCommand(P).TryExecute();
				if (!P.Active.IsOpen)
				{
					using (Stream str = P.FileSystem.OpenRead(chosenFile))
					{
						BinaryFormatter sf = new BinaryFormatter();
						PartituraMemento mem = (PartituraMemento) sf.Deserialize(str);
						Partitura partitura = new Partitura(mem);
						P.Active.Open(chosenFile, partitura);
					}

					new ShowPageCommand(0, P).TryExecute();
					P.MainWindow.PartPattern = P.Active.Partitura.PartPattern;
					P.MainWindow.ShowPartituraInfo(P.Active);
				}
			}
		}

		public bool CanExecute()
		{
			return true;
		}
	}
}