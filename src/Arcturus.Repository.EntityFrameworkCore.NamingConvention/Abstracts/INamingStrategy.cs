namespace Arcturus.Repository.EntityFrameworkCore.NamingConvention.Abstracts;

public interface INamingStrategy
{
    string ApplyNaming(string name);
}