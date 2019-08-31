using System.Collections.Generic;
using System.Linq;

public abstract class PersonHandler
{
    public List<string> GetQuickReplies(List<string> people)
    {
        return people.Select(GetQuickReplyFormula).ToList();
    }
    public abstract string GetQuickReplyFormula(string personName);
}