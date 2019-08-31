using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using idb_dialog_flow;
using islaam_db_client;

namespace idb_dialog_flow
{
    public abstract class Handler
    {
        public abstract string Id { get; }
        public abstract string TextResponse { get; }
        public abstract List<string> QuickReplies { get; }
        public HandlerLite PersonNotFoundHandler(string query, Func<string, string> formula, List<PersonSearchResult> searchResults)
        {
            return new HandlerLite
            {
                TextResponse = $"Sorry. I don't know anyone named \"{query}\"",
                QuickReplies = searchResults
                    .Select(x => formula(x.person.name))
                    .ToList()
            };
        }
        public class HandlerLite
        {
            public string TextResponse;
            public List<string> QuickReplies;
        }
    }


}