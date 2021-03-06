﻿// =================================================================================================================================
// Copyright (c) RapidField LLC. Licensed under the MIT License. See LICENSE.txt in the project root for license information.
// =================================================================================================================================

using System;

namespace RapidField.SolidInstruments.Serialization
{
    /// <summary>
    /// Performs binary, JSON or XML serialization and deserialization for a given type.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the serializable object.
    /// </typeparam>
    public interface ISerializer<T>
        where T : class
    {
        /// <summary>
        /// Converts the specified binary, JSON or XML buffer to its typed equivalent.
        /// </summary>
        /// <param name="buffer">
        /// A serialized object.
        /// </param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer" /> is <see langword="null" />.
        /// </exception>
        T Deserialize(Byte[] buffer);

        /// <summary>
        /// Converts the specified object to a serialized binary, JSON or XML buffer.
        /// </summary>
        /// <param name="target">
        /// An object to be serialized.
        /// </param>
        /// <returns>
        /// The serialized buffer.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="target" /> is <see langword="null" />.
        /// </exception>
        Byte[] Serialize(T target);

        /// <summary>
        /// Gets the format to use for serialization and deserialization.
        /// </summary>
        SerializationFormat Format
        {
            get;
        }
    }
}