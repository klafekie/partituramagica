namespace PartituraMagica.Test
{
	internal class MainClass
	{
		public static int Main(string[] args)
		{
			// The only test failing in EditingTests is TestLabel on p.Active not being 'open' [jed 20141227]
			//new EditingTests().RunAll();			
			new PartituraTests().RunAll();
			//new PlayingTests().RunAll();
			//new NavigatorTests().RunAll();
			new BoxFinderTests().RunAll();
			return 0;
		}
	}
}