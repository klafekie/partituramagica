using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace PartituraMagica
{
	public class OpenCommand : PartituraCommandBase, ICommand
	{
		private string path;

		public OpenCommand(ICommandParameters p)
			: base(p) {
			this.path = null;
		}

		//for testing purposes
		public OpenCommand(string path, ICommandParameters p)
			: base(p) {
			this.path = path;
		}


		public void Execute() {
			
			string chosenFile = this.path != null ?
				this.path : p.FileSystem.ShowOpenDialog(FileType.Partitura);
			if (chosenFile != null) {
				new ClosePartituraCommand(p).TryExecute();
				if (!p.Active.IsOpen) {

					using (Stream str = p.FileSystem.OpenRead(chosenFile)) {
						BinaryFormatter sf = new BinaryFormatter();
						PartituraMemento mem = (PartituraMemento)sf.Deserialize(str);
						Partitura partitura = new Partitura(mem);
						p.Active.Open(chosenFile, partitura);
					}
					new ShowPageCommand(0, p).TryExecute();
					p.MainWindow.PartPattern = p.Active.Partitura.PartPattern;
				}
			}
		}

		public bool CanExecute() {
			return true;
		}
	}
}
