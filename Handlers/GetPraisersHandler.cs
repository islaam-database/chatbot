using System;
using System.Collections.Generic;
using System.Linq;
using idb_dialog_flow;
using islaam_db_client;

namespace IslaamDatabase
{
    public class GetPraisersHandler : SinglePersonHandler
    {
        private readonly List<string> praiserNames;

        public GetPraisersHandler(IslaamDBClient idb, IDictionary<string, object> entities)
            : base(idb, entities)
        {
            this.idb = idb;
            var allPraisers = idb.PraisesAPI.GetData();
            praiserNames = personHelper.GetPraiserNames(allPraisers);
        }

        public override string TextResponse
        {
            get
            {
                if (personHelper.person == null)
                    return PnfHandler.TextResponse;

                if (praiserNames.Count == 0)
                    return $"Sorry. I currently don't have any information on who praised {friendlyName}.";

                if (praiserNames.Count == 1)
                    return $"{friendlyName} was praised by {praiserNames[0]}. {TAIKATM}";

                return $"{friendlyName} was praised by {FriendlyStringJoin(praiserNames)}.";
            }
        }

        public override List<string> QuickReplies
        {
            get
            {
                if (personHelper.person == null)
                    return PnfHandler.QuickReplies;

                var defaultQRs = new List<string>()
                {
                    $"{friendlyName}'s students",
                    $"{friendlyName}'s teachers",
                    $"Who did {friendlyName} praised?",
                };
                var qrs = praiserNames
                    .Concat(
                        personHelper
                            .SearchResults
                            .Select(x => x.person.friendlyName)
                    )
                    .Take(5)
                    .Select(Formula);
                return defaultQRs.Concat(qrs).ToList();
            }
        }

        protected override Func<string, string> Formula => x => $"Who praised {x}?";
    }
}