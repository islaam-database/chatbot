using System;
using System.Linq;
using System.Collections.Generic;
using Islaam;

namespace idb_dialog_flow
{
    public abstract class SinglePersonHandler : Handler
    {
        protected string query;
        protected Database idb;
        protected Person Person;
        protected List<PersonSearchResult> SearchResults;

        protected SinglePersonHandler(Database idb, IDictionary<string, object> entities)
        {
            this.idb = idb;
            query = entities["person"].ToString();
            SearchResults = idb.SearchForPerson(query);
            Person = SearchResults.FirstOrDefault()?.Person;
        }

        /// <summary>
        /// Person not found handler
        /// </summary>
        protected HandlerLite PnfHandler => new HandlerLite
        {
            TextResponse = $"Sorry. I don't know anyone named \"{query}.\"",
            QuickReplies = SearchResults
                    .Select(x => Formula(x.Person.FriendlyName))
                    .ToList()
        };

        protected abstract string Formula(string personName);
    }
}
