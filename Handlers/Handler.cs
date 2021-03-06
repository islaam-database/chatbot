﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace idb_dialog_flow
{
    public abstract class Handler
    {
        public abstract string TextResponse { get; }
        public abstract List<string> QuickReplies { get; }
        public string TAIKATM = "That's all I know at the moment.";
        public class HandlerLite
        {
            public string TextResponse;
            public List<string> QuickReplies;
        }
        public string FriendlyStringJoin(List<string> strings)
        {
            var firstPart = String.Join(", ", strings.SkipLast(1));
            var lastPart = strings.Last();
            return $"{firstPart} and {lastPart}";
        }
    }


}