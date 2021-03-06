﻿// =================================================================================================================================
// Copyright (c) RapidField LLC. Licensed under the MIT License. See LICENSE.txt in the project root for license information.
// =================================================================================================================================

using RapidField.SolidInstruments.Command;
using RapidField.SolidInstruments.Core.Concurrency;
using System;

namespace RapidField.SolidInstruments.Messaging
{
    /// <summary>
    /// Publishes messages to a queue.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of the message that is published by the publisher.
    /// </typeparam>
    public class QueuePublisher<TMessage> : MessagePublisher<TMessage>
        where TMessage : class, IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueuePublisher{TMessage}" /> class.
        /// </summary>
        /// <param name="mediator">
        /// A processing intermediary that is used to process sub-commands.
        /// </param>
        /// <param name="client">
        /// A client that facilitates message publishing operations.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mediator" /> is <see langword="null" /> -or- <paramref name="client" /> is <see langword="null" />.
        /// </exception>
        public QueuePublisher(ICommandMediator mediator, IMessagePublishingClient client)
            : base(mediator, client, MessagingEntityType.Queue)
        {
            return;
        }

        /// <summary>
        /// Releases all resources consumed by the current <see cref="QueuePublisher{TMessage}" />.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether or not managed resources should be released.
        /// </param>
        protected override void Dispose(Boolean disposing) => base.Dispose(disposing);

        /// <summary>
        /// Processes the specified command.
        /// </summary>
        /// <param name="command">
        /// The command to process.
        /// </param>
        /// <param name="mediator">
        /// A processing intermediary that is used to process sub-commands. Do not process <paramref name="command" /> using
        /// <paramref name="mediator" />, as doing so will generally result in infinite-looping.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        protected override void Process(TMessage command, ICommandMediator mediator, ConcurrencyControlToken controlToken) => base.Process(command, mediator, controlToken);
    }
}