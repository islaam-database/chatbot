using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using islaam_db_client;
using static idb_dialog_flow.Handler;

namespace idb_dialog_flow
{
    public abstract class SinglePersonHandler : Handler
    {
        protected string query;
        protected PersonHelper personHelper;
        protected HandlerLite pnfHandler;
        protected IslaamDBClient idb;
        protected string friendlyName;

        protected SinglePersonHandler(IslaamDBClient idb, IDictionary<string, object> entities)
        {
            this.idb = idb;
            query = entities["person"].ToString();
            personHelper = new PersonHelper(query, idb);
            pnfHandler = PersonNotFoundHandler(query, x => $"Who is {x}", personHelper.SearchResults);
            if (personHelper.person != null)
            {
                friendlyName = personHelper.person.friendlyName;
            }
        }

        protected HandlerLite PersonNotFoundHandler(string query, Func<string, string> formula, List<PersonSearchResult> searchResults)
        {
            return new HandlerLite
            {
                TextResponse = $"Sorry. I don't know anyone named \"{query}\"",
                QuickReplies = searchResults
                    .Select(x => formula(x.person.name))
                    .ToList()
            };
        }
    }
}
