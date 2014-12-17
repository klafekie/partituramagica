using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartituraMagica
{
	/** 
	 * Everything that can be done by the user within the 
	 * main application window results in a corresponding
	 * command object being created and executed.
	 * 
	 * The command pattern offers a clean separation of user interface
	 * and application logic, simplifies unit testing and allows
	 * to implement an "Undo" feature (later)
	 */
	public interface ICommand
	{
    bool CanExecute();
	void Execute();		
	}
}
