using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartituraMagica
{
	/** 
	 * Represents a partitura currently open in the application.
	 * Active Partitura knows how to associate image files with 
	 * a partitura. At most one active partitura exists at
	 * a given time.
	 */
	public interface IActivePartitura : IPageFiles
	{			
		string Path {
			get;			
		}		

		Partitura Partitura
		{
			get;			
		}			

		void Open(string path, Partitura partitura);
		void Save(string path);
		bool IsOpen { get; }
		void Close();		
		
		int CurrentPageIndex {
			get;
			set;
		}
	
		string CurrentImagePath {
			get;			
		}		

		Page CurrentPage
		{
			get;
		} 
	}
}
