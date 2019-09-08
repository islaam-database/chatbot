using System;
using System.Collections.Generic;
using System.Linq;
using idb_dialog_flow;
using islaam_db_client;

namespace IslaamDatabase
{
    internal class GetDeathYearHandler : SinglePersonHandler
    {
        public GetDeathYearHandler(IslaamDBClient idb, IDictionary<string, object> entities)
            : base(idb, entities)
        {
        }

        public override string TextResponse
        {
            get
            {
                if (personHelper.person == null)
                    return PnfHandler.TextResponse;

                var deathYear = personHelper.person.deathYear;

                if (deathYear == null)
                    return $"Sorry. I don't what year {friendlyName} passed away.";

                return
                    $"{friendlyName} passed away in the year {deathYear} AH.";
            }
        }

        public override List<string> QuickReplies
        {
            get
            {
                if (personHelper.person == null)
                    return PnfHandler.QuickReplies;

                var allPraises = idb.PraisesAPI.GetData();

                var qr = personHelper
                    .SearchResults
                    .Take(5)
                    .Select(x => $"When did {x.person.friendlyName} die?").ToList();
                qr.Add($"Who praised {friendlyName}");
                qr = qr.Concat(
                        personHelper
                            .GetPraiserNames(allPraises)
                            .Select(x => $"When did {x} die?")
                        )
                    .ToList();
                return qr;
            }
        }

        protected override Func<string, string> Formula => x => $"When did {x} die?";
    }
}