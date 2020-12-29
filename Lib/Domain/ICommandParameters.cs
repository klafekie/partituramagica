#region usings

using System.Windows;

#endregion

namespace PartituraMagica.Domain
{
	/**
     * A collection of parameters needed for all editing and partitura commands
     */
	public interface ICommandParameters
	{
		/**
		 * gets the part of the mainwindow where the user can draw boxes
		 */

		IDrawingPanel Panel { get; set; }

		/**
		 * gets the one and only currently active partitura
		 */

		IActivePartitura Active { get; }

		/**
		 * offers access to standard filesystem operations and dialogs
		 */

		IFileSystem FileSystem { get; }

		//@todo p2 this seems not a good place for this method, maybe move playerView instance
		//elsewhere?
		void DisposePlayerView();
		Shape GetShapeAt(Point currentPos);
	}
}