using System;
using Google.Apis.Dialogflow.v2.Data;
using islaam_db_client;

namespace idb_dialog_flow
{
    public interface Handler
    {
        string Id { get; set; }

        GoogleCloudDialogflowV2IntentMessage GetDefaultResponse(
           string intent,
           IslaamDBClient idb,
           GoogleCloudDialogflowV2WebhookRequest req
       );

        GoogleCloudDialogflowV2IntentMessage GetFacebookResponse(
           string intent,
           IslaamDBClient idb,
           GoogleCloudDialogflowV2WebhookRequest req
       );
    }
}
