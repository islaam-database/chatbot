using System;
using System.Collections.Generic;
using System.Linq;
using idb_dialog_flow;
using Microsoft.EntityFrameworkCore;

namespace IslaamDatabase
{
    public class GetPraisersHandler : SinglePersonHandler
    {
        private readonly List<string> praiserNames;

        public GetPraisersHandler(Islaam.Database idb, IDictionary<string, object> entities)
            : base(idb, entities)
        {
            this.idb = idb;
            if (Person != null)
            {
                Person = idb
                    .People
                    .Include(p => p.PraisesReceived)
                        .ThenInclude(x => x.Praiser)
                            .ThenInclude(x => x.PraisesReceived)
                                .ThenInclude(x => x.Praiser)
                    .Include(p => p.PraisesReceived)
                        .ThenInclude(x => x.Title)
                            .ThenInclude(x => x.Status)
                    .Where(p => p.Id == Person.Id)
                    .FirstOrDefault();

                praiserNames = Person
                    .GetPraisesReceivedConsideringStatus()
                    .Select(x => x.Praiser.Name)
                    .Distinct()
                    .ToList();
            }
        }

        public override string TextResponse
        {
            get
            {
                if (Person == null)
                    return PnfHandler.TextResponse;

                if (praiserNames.Count == 0)
                    return $"Sorry. I currently don't have any information on who praised {Person.FriendlyName}.";

                if (praiserNames.Count == 1)
                    return $"{Person.FriendlyName} was praised by {praiserNames[0]}. {TAIKATM}";

                return $"{Person.FriendlyName} was praised by {FriendlyStringJoin(praiserNames)}.";
            }
        }

        public override List<string> QuickReplies
        {
            get
            {
                if (Person == null)
                    return PnfHandler.QuickReplies;

                var defaultQRs = new List<string>()
                {
                    $"{Person.FriendlyName}'s students",
                    $"{Person.FriendlyName}'s teachers",
                    $"Who did {Person.FriendlyName} praise?",
                };
                var qrs = praiserNames
                    .Concat(SearchResults.Select(x => x.Person.FriendlyName))
                    .Take(5)
                    .Select(Formula);
                return defaultQRs.Concat(qrs).ToList();
            }
        }


        protected override string Formula(string personName) => $"Who praised {personName}?";
    }
}