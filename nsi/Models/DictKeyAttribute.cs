namespace nsi.Models;

public class DictKeyAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
