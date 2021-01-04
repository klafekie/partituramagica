#region usings

using System;
using System.Collections.Generic;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.Playing
{
	/**
     * A user interface element that allows the user to choose from
     * a (small) set of available parts.
     * Parts are represented by strings and the current value
     * is an index
     */
	internal interface IPartNavigator
	{
		int CurrentPartIndex { get; }
		void LoadPartitura(IPlayedPartitura partitura);

		event Action CurrentPartIndexChanged;

		void PartClicked(int index);

		/** going to previous part can be prevented by listeners
         * by setting the cancel parameter to true
         * In this case the same part is "clicked" again
         */
		void PreviousPartClicked();

		void NextPartClicked();
		void LastPartClicked();
		void FirstPartClicked();
		void JumpForwardClicked();
		void JumpBackwardClicked();
		bool IsPartituraFinished();
		IList<Clipping> GetClippings();
	}
}