﻿// =================================================================================================================================
// Copyright (c) RapidField LLC. Licensed under the MIT License. See LICENSE.txt in the project root for license information.
// =================================================================================================================================

using System;
using System.Collections.Generic;

namespace RapidField.SolidInstruments.Messaging.Service
{
    /// <summary>
    /// Manages the subscriber types that are supported by a
    /// <see cref="MessagingServiceExecutor{TDependencyPackage, TDependencyConfigurator, TDependencyEngine}" />.
    /// </summary>
    public interface IMessageSubscriptionProfile
    {
        /// <summary>
        /// Adds support for the specified queue message type.
        /// </summary>
        /// <typeparam name="TMessage">
        /// The type of the message for which support is added.
        /// </typeparam>
        /// <exception cref="InvalidOperationException">
        /// <typeparamref name="TMessage" /> was already added.
        /// </exception>
        void AddQueueSubscriber<TMessage>()
            where TMessage : class, IMessage;

        /// <summary>
        /// Adds support for the specified request message type.
        /// </summary>
        /// <typeparam name="TRequestMessage">
        /// The type of the request message for which support is added.
        /// </typeparam>
        /// <typeparam name="TResponseMessage">
        /// The type of the response message that is associated with <typeparamref name="TRequestMessage" />.
        /// </typeparam>
        /// <exception cref="InvalidOperationException">
        /// <typeparamref name="TRequestMessage" /> was already added.
        /// </exception>
        void AddRequestSubscriber<TRequestMessage, TResponseMessage>()
            where TRequestMessage : class, IRequestMessage<TResponseMessage>
            where TResponseMessage : class, IResponseMessage;

        /// <summary>
        /// Adds support for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">
        /// The type of the message for which support is added.
        /// </typeparam>
        /// <param name="entityType">
        /// The targeted entity type.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="entityType" /> is equal to <see cref="MessagingEntityType.Unspecified" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <typeparamref name="TMessage" /> was already added.
        /// </exception>
        void AddSubscriber<TMessage>(MessagingEntityType entityType)
            where TMessage : class, IMessage;

        /// <summary>
        /// Adds support for the specified topic message type.
        /// </summary>
        /// <typeparam name="TMessage">
        /// The type of the message for which support is added.
        /// </typeparam>
        /// <exception cref="InvalidOperationException">
        /// <typeparamref name="TMessage" /> was already added.
        /// </exception>
        void AddTopicSubscriber<TMessage>()
            where TMessage : class, IMessage;

        /// <summary>
        /// Gets a collection of message types that are supported by the associated
        /// <see cref="MessagingServiceExecutor{TDependencyPackage, TDependencyConfigurator, TDependencyEngine}" />.
        /// </summary>
        IEnumerable<Type> SupportedMessageTypes
        {
            get;
        }
    }
}