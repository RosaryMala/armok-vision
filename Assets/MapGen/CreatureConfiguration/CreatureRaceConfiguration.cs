using RemoteFortressReader;
using System;
using System.Xml.Linq;

public class CreatureRaceConfiguration<T> : CreatureConfiguration<T> where T : IContent, new()
{
    CreatureRaceMatcher<Content> raceStorage = new CreatureRaceMatcher<Content>();
    public override object SecondaryDictionary
    {
        set
        {
        }
    }

    public override bool GetValue(UnitDefinition unit, out T value)
    {
        throw new NotImplementedException();
    }

    protected override void ParseElementConditions(XElement elemtype, Content content)
    {
        throw new NotImplementedException();
    }
}
