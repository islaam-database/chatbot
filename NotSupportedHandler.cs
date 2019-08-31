using idb_dialog_flow;
using System.Collections.Generic;

namespace IslaamDatabase
{
    public class NotSupportedHandler : Handler
    {
        public override string Id
        {
            get
            {
                return "unsupported-intent";
            }
        }

        public override string TextResponse
        {
            get
            {
                return "Subhaanallaah. I wish I can answer that question but I'm not that smart yet.";
            }
        }

        public override List<string> QuickReplies
        {
            get
            {
                return new List<string> {
                    "Who is Shaykh Rabee?",
                    "Who praised Shaykh Rabee?",
                    "Who is Shayhk Muhammad ibn 'Abdel-Wahhaab?",
                    "Who is Shaykh Ahmad an-Najmee?",
                };
            }
        }
    }
}