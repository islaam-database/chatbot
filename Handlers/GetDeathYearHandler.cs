using System;
using System.Collections.Generic;
using System.Linq;
using idb_dialog_flow;

namespace IslaamDatabase
{
    internal class GetDeathYearHandler : SinglePersonHandler
    {
        public GetDeathYearHandler(Islaam.Database idb, IDictionary<string, object> entities)
            : base(idb, entities)
        {
        }

        public override string TextResponse
        {
            get
            {
                if (Person == null)
                    return PnfHandler.TextResponse;

                var deathYear = Person.DeathYear;

                if (deathYear == null)
                    return $"Sorry. I don't what year {Person.FriendlyName} passed away.";

                return
                    $"{Person.FriendlyName} passed away in the year {deathYear} AH.";
            }
        }

        public override List<string> QuickReplies
        {
            get
            {
                if (Person == null)
                    return PnfHandler.QuickReplies;

                var qr = SearchResults
                    .Take(5)
                    .Select(x => $"When did {x.Person.FriendlyName} die?").ToList();

                var praiserNames = Person
                    .PraisesReceived
                    .Select(x => x.Praiser.Name)
                    .Select(x => $"When did {x} die?")
                    .ToList();

                qr.Add($"Who praised {Person.FriendlyName}");
                qr = qr.Concat(praiserNames).ToList();
                return qr;
            }
        }

        protected override string Formula(string personName) => $"When did {personName} die?";
    }
}