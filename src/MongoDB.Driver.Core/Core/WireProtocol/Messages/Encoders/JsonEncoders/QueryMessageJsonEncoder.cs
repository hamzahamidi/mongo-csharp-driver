﻿/* Copyright 2013-2014 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Driver.Core.WireProtocol.Messages.Encoders.JsonEncoders
{
    public class QueryMessageJsonEncoder : MessageJsonEncoderBase, IMessageEncoder<QueryMessage>
    {
        // constructors
        public QueryMessageJsonEncoder(TextReader textReader, TextWriter textWriter, MessageEncoderSettings encoderSettings)
            : base(textReader, textWriter, encoderSettings)
        {
        }

        // methods
        public QueryMessage ReadMessage()
        {
            var jsonReader = CreateJsonReader();
            var messageContext = BsonDeserializationContext.CreateRoot<BsonDocument>(jsonReader);
            var messageDocument = BsonDocumentSerializer.Instance.Deserialize(messageContext);

            var opcode = messageDocument["opcode"].AsString;
            if (opcode != "query")
            {
                throw new FormatException("Opcode is not query.");
            }

            var requestId = messageDocument["requestId"].ToInt32();
            var databaseName = messageDocument["database"].AsString;
            var collectionName = messageDocument["collection"].AsString;
            var query = messageDocument["query"].AsBsonDocument;
            var fields = (BsonDocument)messageDocument.GetValue("fields", null);
            var skip = messageDocument.GetValue("skip", 0).ToInt32();
            var batchSize = messageDocument.GetValue("batchSize", 0).ToInt32();
            var slaveOk = messageDocument.GetValue("slaveOk", false).ToBoolean();
            var partialOk = messageDocument.GetValue("partialOk", false).ToBoolean();
            var noCursorTimeout = messageDocument.GetValue("noCursorTimeout", false).ToBoolean();
            var tailableCursor = messageDocument.GetValue("tailableCursor", false).ToBoolean();
            var awaitData = messageDocument.GetValue("awaitData", false).ToBoolean();

            return new QueryMessage(
                requestId,
                new CollectionNamespace(databaseName, collectionName),
                query,
                fields,
                NoOpElementNameValidator.Instance,
                skip,
                batchSize,
                slaveOk,
                partialOk,
                noCursorTimeout,
                tailableCursor,
                awaitData);
        }

        public void WriteMessage(QueryMessage message)
        {
            Ensure.IsNotNull(message, "message");

            var messageDocument = new BsonDocument
            {
                { "opcode", "query" },
                { "requestId", message.RequestId },
                { "database", message.CollectionNamespace.DatabaseNamespace.DatabaseName },
                { "collection", message.CollectionNamespace.CollectionName },
                { "fields", message.Fields, message.Fields != null },
                { "skip", message.Skip, message.Skip != 0 },
                { "batchSize", message.BatchSize, message.BatchSize != 0 },
                { "slaveOk", true, message.SlaveOk },
                { "partialOk", true, message.PartialOk },
                { "noCursorTimeout", true, message.NoCursorTimeout },
                { "tailableCursor", true, message.TailableCursor },
                { "awaitData", true, message.AwaitData },
                { "query", message.Query }
            };

            var jsonWriter = CreateJsonWriter();
            var messageContext = BsonSerializationContext.CreateRoot<BsonDocument>(jsonWriter);
            BsonDocumentSerializer.Instance.Serialize(messageContext, messageDocument);
        }

        // explicit interface implementations
        MongoDBMessage IMessageEncoder.ReadMessage()
        {
            return ReadMessage();
        }

        void IMessageEncoder.WriteMessage(MongoDBMessage message)
        {
            WriteMessage((QueryMessage)message);
        }
    }
}
