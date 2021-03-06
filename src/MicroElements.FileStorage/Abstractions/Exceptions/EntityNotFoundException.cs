﻿// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace MicroElements.FileStorage.Abstractions.Exceptions
{
    /// <summary>
    /// Indicates when entity not found cases.
    /// </summary>
    [Serializable]
    public class EntityNotFoundException : FileStorageException
    {
        public EntityNotFoundException() { }

        public EntityNotFoundException(string message) : base(message) { }

        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
