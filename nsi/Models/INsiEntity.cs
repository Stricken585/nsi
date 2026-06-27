namespace nsi.Models;

public interface INsiEntity
{
    void Fill(Dictionary<string, string?> dict);
}
