using System;
using System.Collections.Generic;
using System.Linq;
using idb_dialog_flow;
using islaam_db_client;

namespace IslaamDatabase
{
    public class GetPraiseesHandler : SinglePersonHandler
    {
        private readonly List<string> praiseeNames;

        public GetPraiseesHandler(IslaamDBClient idb, IDictionary<string, object> entities)
            : base(idb, entities)
        {
            this.idb = idb;
            var allPraises = idb.PraisesAPI.GetData();
            praiseeNames = personHelper.GetPraiseeNames(allPraises);
        }

        public override string TextResponse
        {
            get
            {
                if (personHelper.person == null)
                    return PnfHandler.TextResponse;

                if (praiseeNames.Count == 0)
                    return $"Sorry. I currently don't have any information on who {friendlyName} praised.";

                if (praiseeNames.Count == 1)
                    return $"{friendlyName} praised {praiseeNames[0]}. {TAIKATM}";

                return $"{friendlyName} praised {FriendlyStringJoin(praiseeNames)}.";
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
                    $"Who praised {friendlyName}?",
                };
                var qrs = praiseeNames
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