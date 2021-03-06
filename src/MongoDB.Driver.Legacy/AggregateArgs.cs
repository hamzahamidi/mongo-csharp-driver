/* Copyright 2010-present MongoDB Inc.
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
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Driver
{
    /// <summary>
    /// Represents the output mode for an aggregate operation.
    /// </summary>
    [Obsolete("Server versions 3.6 and newer always use a cursor.")]
    public enum AggregateOutputMode
    {
        /// <summary>
        /// The output of the aggregate operation is returned inline.
        /// </summary>
        Inline,
        /// <summary>
        /// The output of the aggregate operation is returned using a cursor.
        /// </summary>
        Cursor
    }

    /// <summary>
    /// Represents options for the Aggregate command.
    /// </summary>
    public class AggregateArgs
    {
        // private fields
        private bool? _allowDiskUse;
        private int? _batchSize;
        private bool? _bypassDocumentValidation;
        private Collation _collation;
        private TimeSpan? _maxTime;
#pragma warning disable 618
        private AggregateOutputMode _outputMode = AggregateOutputMode.Inline;
#pragma warning restore 618
        private IEnumerable<BsonDocument> _pipeline;

        // public properties
        /// <summary>
        /// Gets or sets a value indicating whether disk use is allowed.
        /// </summary>
        public bool? AllowDiskUse
        {
            get { return _allowDiskUse; }
            set { _allowDiskUse = value; }
        }

        /// <summary>
        /// Gets or sets the size of a batch when using a cursor.
        /// </summary>
        public int? BatchSize
        {
            get { return _batchSize; }
            set
            {
                if (value.HasValue && value.Value < 0)
                {
                    throw new ArgumentException("BatchSize cannot be negative.", "value");
                }
                _batchSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to bypass document validation.
        /// </summary>
        /// <value>
        /// A value indicating whether to bypass document validation.
        /// </value>
        public bool? BypassDocumentValidation
        {
            get { return _bypassDocumentValidation; }
            set { _bypassDocumentValidation = value; }
        }

        /// <summary>
        /// Gets or sets the collation.
        /// </summary>
        /// <value>
        /// The collation.
        /// </value>
        public Collation Collation
        {
            get { return _collation; }
            set { _collation = value; }
        }

        /// <summary>
        /// Gets or sets the max time the server should spend on the aggregation command.
        /// </summary>
        public TimeSpan? MaxTime
        {
            get { return _maxTime; }
            set { _maxTime = Ensure.IsNullOrInfiniteOrGreaterThanOrEqualToZero(value, nameof(value)); }
        }

        /// <summary>
        /// Gets or sets the output mode.
        /// </summary>
        [Obsolete("Server versions 3.6 and newer always use a cursor.")]
        public AggregateOutputMode OutputMode
        {
            get { return _outputMode; }
            set { _outputMode = value; }
        }

        /// <summary>
        /// Gets or sets the pipeline.
        /// </summary>
        public IEnumerable<BsonDocument> Pipeline
        {
            get { return _pipeline; }
            set { _pipeline = value.ToList(); }
        }
    }
}
