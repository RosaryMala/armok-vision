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
        Content cont;
        if(raceStorage.TryGetValue(unit.race, out cont))
        {
            value = cont.GetValue(unit);
            return true;
        }
        else
        {
            value = default(T);
            return false;
        }
    }

    protected override void ParseElementConditions(XElement elemtype, Content content)
    {
        var elemCreatures = elemtype.Elements("creature");
        foreach (XElement elemCreature in elemCreatures)
        {
            XAttribute elemGameID = elemCreature.Attribute("gameID");
            if (elemGameID != null)
            {
                raceStorage[elemGameID.Value] = content;
            }
        }

    }
}
