using System;
using islaam_db_client;

namespace idb_dialog_flow
{
    public static class Intents
    {
        public static class WHO_IS
        {
            public static string Name = "who-is";
            public static string DefaultResponse(Person p)
            {
                return $"Who is {p.name}?";
            }
            public static string DefaultResponse(string personName)
            {
                return $"Who is {personName}?";
            }
        }
    }
}
