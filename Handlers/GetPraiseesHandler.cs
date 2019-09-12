using System.Collections.Generic;
using System.Linq;
using idb_dialog_flow;
using Microsoft.EntityFrameworkCore;

namespace IslaamDatabase
{
    public class GetPraiseesHandler : SinglePersonHandler
    {
        private readonly List<string> praiseeNames;

        public GetPraiseesHandler(Islaam.Database idb, IDictionary<string, object> entities)
            : base(idb, entities)
        {
            this.idb = idb;
            if (Person != null)
            {
                Person = idb
                    .People
                    .Include(p => p.PraisesGiven)
                    .Where(p => p.Id == Person.Id)
                    .First();

                praiseeNames = Person
                    .PraisesGiven
                    .Select(x => x.Praisee.Name)
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

                if (praiseeNames.Count == 0)
                    return $"Sorry. I currently don't have any information on who {Person.FriendlyName} praised.";

                if (praiseeNames.Count == 1)
                    return $"{Person.FriendlyName} praised {praiseeNames[0]}. {TAIKATM}";

                return $"{Person.FriendlyName} praised {FriendlyStringJoin(praiseeNames)}.";
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
                    $"Who praised {Person.FriendlyName}?",
                };
                var qrs = praiseeNames
                    .Concat(SearchResults.Select(x => x.Person.FriendlyName))
                    .Take(5)
                    .Select(Formula);
                return defaultQRs.Concat(qrs).ToList();
            }
        }

        protected override string Formula(string personName) => $"Who praised {personName}?";
    }
}