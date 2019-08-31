using System;
using Google.Apis.Dialogflow.v2.Data;
using islaam_db_client;

namespace idb_dialog_flow
{
    public interface Handler
    {
        /// <summary>
        /// The id of the intent. Same as in Dialogflow.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Returns a default web messeage response
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="idb"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        GoogleCloudDialogflowV2IntentMessage GetDefaultResponse(
           string intent,
           IslaamDBClient idb,
           GoogleCloudDialogflowV2WebhookRequest req
       );

        /// <summary>
        /// Returns a Facebook Messenger response.
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="idb"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        GoogleCloudDialogflowV2IntentMessage GetFacebookResponse(
           string intent,
           IslaamDBClient idb,
           GoogleCloudDialogflowV2WebhookRequest req
       );
    }
}
