namespace PartituraMagica
{
    /** 
     * represents an instance that knows how to associate page indices 
     * with file names and how to count the pages
     */

    public interface IPageFiles
    {
        string GetImage(int index);
        int GetPageCount();
    }
}