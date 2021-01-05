#region usings

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using PartituraMagica;

#endregion

namespace PartituraFileConverter
{
	//convert all .partitura and .runtime files in given folder to new json format.
	// this is required to get rid of BinaryFormatter which has a lot of drawbacks.
	// e.g. it prevents changing type names for used types.
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("Please provide the root folder as first parameter.");
				return;
			}

			foreach (var file in Directory.EnumerateFiles(args[0], "*.runtime", SearchOption.AllDirectories))
			{
				Console.WriteLine(file);
				using (Stream str = File.OpenRead(file))
				{
					BinaryFormatter sf = new BinaryFormatter();
					PlayerMemento memento = (PlayerMemento) sf.Deserialize(str);

					string asString = JsonSerializer.Serialize(memento);
					File.WriteAllText(file + ".json", asString);
				}
			}

			foreach (var file in Directory.EnumerateFiles(args[0], "*.partitura", SearchOption.AllDirectories))
			{
				Console.WriteLine(file);
				using (Stream str = File.OpenRead(file))
				{
					BinaryFormatter sf = new BinaryFormatter();
#pragma warning disable 618
					var memento = (PartituraMemento) sf.Deserialize(str);
#pragma warning restore 618

					string asString = JsonSerializer.Serialize(memento);
					File.WriteAllText(file + ".json", asString);
				}
			}

			Console.ReadKey();
		}
	}
}