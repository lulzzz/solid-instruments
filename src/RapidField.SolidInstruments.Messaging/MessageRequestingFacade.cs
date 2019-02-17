﻿// =================================================================================================================================
// Copyright (c) RapidField LLC. Licensed under the MIT License. See LICENSE.txt in the project root for license information.
// =================================================================================================================================

using RapidField.SolidInstruments.Core.ArgumentValidation;
using RapidField.SolidInstruments.Core.Concurrency;
using RapidField.SolidInstruments.Core.Extensions;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RapidField.SolidInstruments.Messaging
{
    /// <summary>
    /// Facilitates implementation-specific requesting operations for a message bus.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of the implementation-specific send client.
    /// </typeparam>
    /// <typeparam name="TReceiver">
    /// The type of the implementation-specific receive client.
    /// </typeparam>
    /// <typeparam name="TAdaptedMessage">
    /// The type of implementation-specific adapted messages.
    /// </typeparam>
    /// <typeparam name="TPublishingFacade">
    /// The type of the implementation-specific messaging facade that is used to publish request and response messages.
    /// </typeparam>
    /// <typeparam name="TSubscriptionFacade">
    /// The type of the implementation-specific messaging facade that subscribes to request messages.
    /// </typeparam>
    /// <remarks>
    /// <see cref="MessageRequestingFacade{TSender, TReceiver, TAdaptedMessage, TPublishingFacade, TSubscriptionFacade}" /> is the
    /// default implementation of
    /// <see cref="IMessageRequestingFacade{TSender, TReceiver, TAdaptedMessage, TPublishingFacade, TSubscriptionFacade}" />.
    /// </remarks>
    public abstract class MessageRequestingFacade<TSender, TReceiver, TAdaptedMessage, TPublishingFacade, TSubscriptionFacade> : MessageRequestingFacade<TSender, TReceiver, TAdaptedMessage>, IMessageRequestingFacade<TSender, TReceiver, TAdaptedMessage, TPublishingFacade, TSubscriptionFacade>
        where TAdaptedMessage : class
        where TPublishingFacade : MessagePublishingFacade<TSender, TReceiver, TAdaptedMessage>
        where TSubscriptionFacade : MessageSubscriptionFacade<TSender, TReceiver, TAdaptedMessage, TPublishingFacade>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRequestingFacade{TSender, TReceiver, TAdaptedMessage}" /> class.
        /// </summary>
        /// <param name="subscriptionFacade">
        /// An implementation-specific messaging facade that subscribes to request messages.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="subscriptionFacade" /> is <see langword="null" />.
        /// </exception>
        protected MessageRequestingFacade(TSubscriptionFacade subscriptionFacade)
            : base(subscriptionFacade.RejectIf().IsNull(nameof(subscriptionFacade)).TargetArgument.ClientFactory, subscriptionFacade.MessageAdapter)
        {
            PublishingFacade = subscriptionFacade.PublishingFacade;
            SubscriptionFacade = subscriptionFacade;
        }

        /// <summary>
        /// Releases all resources consumed by the current
        /// <see cref="MessageRequestingFacade{TSender, TReceiver, TAdaptedMessage, TPublishingFacade, TSubscriptionFacade}" />.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether or not managed resources should be released.
        /// </param>
        protected override void Dispose(Boolean disposing) => base.Dispose(disposing);

        /// <summary>
        /// Asynchronously publishes the specified request message to a bus.
        /// </summary>
        /// <typeparam name="TRequestMessage">
        /// The type of the request message.
        /// </typeparam>
        /// <typeparam name="TResponseMessage">
        /// The type of the response message.
        /// </typeparam>
        /// <param name="requestMessage">
        /// The request message to publish.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        protected sealed override Task PublishRequestMessageAsync<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage, ConcurrencyControlToken controlToken) => PublishingFacade.PublishAsync(requestMessage, Message.RequestEntityType);

        /// <summary>
        /// Registers the specified response handler with a bus.
        /// </summary>
        /// <typeparam name="TResponseMessage">
        /// The type of the response message.
        /// </typeparam>
        /// <param name="responseHandler">
        /// An action that handles the response.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        protected sealed override void RegisterResponseHandler<TResponseMessage>(Action<TResponseMessage> responseHandler, ConcurrencyControlToken controlToken) => SubscriptionFacade.RegisterHandler(responseHandler, Message.ResponseEntityType);

        /// <summary>
        /// Represents an implementation-specific messaging facade that is used to publish request and response messages.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TPublishingFacade PublishingFacade;

        /// <summary>
        /// Represents an implementation-specific messaging facade that subscribes to request messages.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TSubscriptionFacade SubscriptionFacade;
    }

    /// <summary>
    /// Facilitates implementation-specific requesting operations for a message bus.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of the implementation-specific send client.
    /// </typeparam>
    /// <typeparam name="TReceiver">
    /// The type of the implementation-specific receive client.
    /// </typeparam>
    /// <typeparam name="TAdaptedMessage">
    /// The type of implementation-specific adapted messages.
    /// </typeparam>
    /// <remarks>
    /// <see cref="MessageRequestingFacade{TSender, TReceiver, TAdaptedMessage}" /> is the default implementation of
    /// <see cref="IMessageRequestingFacade{TSender, TReceiver, TAdaptedMessage}" />.
    /// </remarks>
    public abstract class MessageRequestingFacade<TSender, TReceiver, TAdaptedMessage> : MessagingFacade<TSender, TReceiver, TAdaptedMessage>, IMessageRequestingFacade<TSender, TReceiver, TAdaptedMessage>
        where TAdaptedMessage : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRequestingFacade{TSender, TReceiver, TAdaptedMessage}" /> class.
        /// </summary>
        /// <param name="clientFactory">
        /// An appliance that creates manages implementation-specific messaging clients.
        /// </param>
        /// <param name="messageAdapter">
        /// An appliance that facilitates implementation-specific message conversion.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="clientFactory" /> is <see langword="null" /> -or- <paramref name="messageAdapter" /> is
        /// <see langword="null" />.
        /// </exception>
        protected MessageRequestingFacade(IMessagingClientFactory<TSender, TReceiver, TAdaptedMessage> clientFactory, IMessageAdapter<TAdaptedMessage> messageAdapter)
            : base(clientFactory, messageAdapter)
        {
            return;
        }

        /// <summary>
        /// Asynchronously publishes the specified request message to a bus and waits for the correlated response message.
        /// </summary>
        /// <typeparam name="TRequestMessage">
        /// The type of the request message.
        /// </typeparam>
        /// <typeparam name="TResponseMessage">
        /// The type of the response message.
        /// </typeparam>
        /// <param name="requestMessage">
        /// The request message to publish.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation and containing the correlated response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="requestMessage" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="MessagePublishingException">
        /// An exception was raised while attempting to publish <paramref name="requestMessage" />.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The object is disposed.
        /// </exception>
        public Task<TResponseMessage> RequestAsync<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage)
            where TRequestMessage : class, IRequestMessage<TResponseMessage>
            where TResponseMessage : class, IResponseMessage
        {
            var requestMessageIdentifier = requestMessage.RejectIf().IsNull(nameof(requestMessage)).TargetArgument.Identifier;

            using (var controlToken = StateControl.Enter())
            {
                if (TryAddOutstandingRequest<TRequestMessage, TResponseMessage>(requestMessage, requestMessageIdentifier))
                {
                    try
                    {
                        RegisterResponseHandler<TResponseMessage>(HandleResponseMessage, controlToken);
                    }
                    catch (Exception exception)
                    {
                        throw new MessagePublishingException(typeof(TRequestMessage), exception);
                    }

                    return PublishRequestMessageAsync<TRequestMessage, TResponseMessage>(requestMessage, controlToken).ContinueWith((publishTask) =>
                    {
                        RejectIfDisposed();

                        try
                        {
                            var responseMessage = WaitForResponse(requestMessageIdentifier) as TResponseMessage;

                            if (responseMessage is null)
                            {
                                throw new MessageSubscriptionException("The response message is invalid.");
                            }

                            return responseMessage;
                        }
                        catch (MessagePublishingException)
                        {
                            throw;
                        }
                        catch (MessageSubscriptionException exception)
                        {
                            throw new MessagePublishingException(typeof(TRequestMessage), exception);
                        }
                        catch (TimeoutException exception)
                        {
                            throw new MessagePublishingException(typeof(TRequestMessage), exception);
                        }
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);
                }
            }

            throw new InvalidOperationException("The request was not processed because it is a duplicate.");
        }

        /// <summary>
        /// Releases all resources consumed by the current
        /// <see cref="MessageRequestingFacade{TSender, TReceiver, TAdaptedMessage}" />.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether or not managed resources should be released.
        /// </param>
        protected override void Dispose(Boolean disposing) => base.Dispose(disposing);

        /// <summary>
        /// Asynchronously publishes the specified request message to a bus.
        /// </summary>
        /// <typeparam name="TRequestMessage">
        /// The type of the request message.
        /// </typeparam>
        /// <typeparam name="TResponseMessage">
        /// The type of the response message.
        /// </typeparam>
        /// <param name="requestMessage">
        /// The request message to publish.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        protected abstract Task PublishRequestMessageAsync<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage, ConcurrencyControlToken controlToken)
            where TRequestMessage : class, IRequestMessage<TResponseMessage>
            where TResponseMessage : class, IResponseMessage;

        /// <summary>
        /// Registers the specified response handler with a bus.
        /// </summary>
        /// <typeparam name="TResponseMessage">
        /// The type of the response message.
        /// </typeparam>
        /// <param name="responseHandler">
        /// An action that handles the response.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        protected abstract void RegisterResponseHandler<TResponseMessage>(Action<TResponseMessage> responseHandler, ConcurrencyControlToken controlToken)
            where TResponseMessage : class, IResponseMessage;

        /// <summary>
        /// Handles the specified response message.
        /// </summary>
        /// <typeparam name="TResponseMessage">
        /// The type of the response message.
        /// </typeparam>
        /// <param name="responseMessage">
        /// The response message to handle.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="responseMessage" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The internal state of the requesting facade is corrupt.
        /// </exception>
        [DebuggerHidden]
        private void HandleResponseMessage<TResponseMessage>(TResponseMessage responseMessage)
            where TResponseMessage : class, IResponseMessage
        {
            if (TryAddUnprocessedResponse(responseMessage.RejectIf().IsNull(nameof(responseMessage)).TargetArgument, responseMessage.RequestMessageIdentifier))
            {
                return;
            }

            throw new InvalidOperationException("The response message could not be processed because the internal state of the requesting facade is corrupt.");
        }

        /// <summary>
        /// Attempts to add the specified request to a list of outstanding requests as a thread safe operation.
        /// </summary>
        /// <typeparam name="TRequestMessage">
        /// The type of the request message.
        /// </typeparam>
        /// <typeparam name="TResponseMessage">
        /// The type of the response message.
        /// </typeparam>
        /// <param name="requestMessage">
        /// The outstanding request message.
        /// </param>
        /// <param name="requestMessageIdentifier">
        /// An identifier for the outstanding request message.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the request was added, otherwise <see langword="false" />.
        /// </returns>
        [DebuggerHidden]
        private Boolean TryAddOutstandingRequest<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage, Guid requestMessageIdentifier)
            where TRequestMessage : class, IRequestMessage<TResponseMessage>
            where TResponseMessage : class, IResponseMessage => OutstandingRequests.TryAdd(requestMessageIdentifier, requestMessage);

        /// <summary>
        /// Attempts to add the specified response to a list of unprocessed responses as a thread safe operation.
        /// </summary>
        /// <param name="responseMessage">
        /// The unprocessed response message.
        /// </param>
        /// <param name="requestMessageIdentifier">
        /// An identifier for the associated request message.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the response was added, otherwise <see langword="false" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The internal state of the requesting facade is corrupt.
        /// </exception>
        [DebuggerHidden]
        private Boolean TryAddUnprocessedResponse(IResponseMessage responseMessage, Guid requestMessageIdentifier)
        {
            if (OutstandingRequests.ContainsKey(requestMessageIdentifier))
            {
                if (UnprocessedResponses.TryAdd(requestMessageIdentifier, responseMessage))
                {
                    if (OutstandingRequests.TryRemove(requestMessageIdentifier, out var requestMessage))
                    {
                        return true;
                    }

                    throw new InvalidOperationException("A request operation caused the message requesting facade to enter a corrupt state.");
                }
            }

            return false;
        }

        /// <summary>
        /// Waits for a response message matching the specified request to populate the client manager's response dictionary.
        /// </summary>
        /// <param name="requestMessageIdentifier">
        /// An identifier for the associated request message.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation and containing the response message.
        /// </returns>
        /// <exception cref="TimeoutException">
        /// The timeout threshold duration was exceeded while waiting for a response.
        /// </exception>
        [DebuggerHidden]
        private IResponseMessage WaitForResponse(Guid requestMessageIdentifier) => WaitForResponse(requestMessageIdentifier, DefaultWaitForResponseTimeoutThreshold);

        /// <summary>
        /// Waits for a response message matching the specified request to populate the client manager's response dictionary.
        /// </summary>
        /// <param name="requestMessageIdentifier">
        /// An identifier for the associated request message.
        /// </param>
        /// <param name="timeoutThreshold">
        /// The maximum length of time to wait for the response before raising an exception, or
        /// <see cref="Timeout.InfiniteTimeSpan" /> to specify an infinite duration. The default value is one minute.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation and containing the response message.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeoutThreshold" /> is less than or equal to <see cref="TimeSpan.Zero" /> -and-
        /// <paramref name="timeoutThreshold" /> is not equal to <see cref="Timeout.InfiniteTimeSpan" />.
        /// </exception>
        /// <exception cref="TimeoutException">
        /// The timeout threshold duration was exceeded while waiting for a response.
        /// </exception>
        [DebuggerHidden]
        private IResponseMessage WaitForResponse(Guid requestMessageIdentifier, TimeSpan timeoutThreshold)
        {
            var stopwatch = new Stopwatch();

            if (timeoutThreshold.RejectIf(argument => argument <= TimeSpan.Zero && argument != Timeout.InfiniteTimeSpan, nameof(timeoutThreshold)) != Timeout.InfiniteTimeSpan)
            {
                stopwatch.Start();
            }

            while (stopwatch.Elapsed < timeoutThreshold)
            {
                Thread.Sleep(ResponseMessagePollingInterval);

                if (UnprocessedResponses.TryRemove(requestMessageIdentifier, out var responseMessage))
                {
                    return responseMessage;
                }
            }

            throw new TimeoutException($"The timeout threshold duration was exceeded while waiting for a response to request message {requestMessageIdentifier.ToSerializedString()}.");
        }

        /// <summary>
        /// Represents a collection of outstanding requests that key pending response messages.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ConcurrentDictionary<Guid, IMessageBase> OutstandingRequests => LazyOutstandingRequests.Value;

        /// <summary>
        /// Gets a collection of unprocessed response messages that are keyed by request message identifier.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ConcurrentDictionary<Guid, IResponseMessage> UnprocessedResponses => LazyUnprocessedResponses.Value;

        /// <summary>
        /// Represents the default maximum length of time to wait for the response before raising an exception.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly TimeSpan DefaultWaitForResponseTimeoutThreshold = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Represents the polling interval that is used when waiting for a response message.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly TimeSpan ResponseMessagePollingInterval = TimeSpan.FromMilliseconds(2);

        /// <summary>
        /// Represents a lazily-initialized collection of outstanding requests that key pending response messages.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<ConcurrentDictionary<Guid, IMessageBase>> LazyOutstandingRequests = new Lazy<ConcurrentDictionary<Guid, IMessageBase>>(() => new ConcurrentDictionary<Guid, IMessageBase>(), LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Represents a lazily-initialized collection of unprocessed response messages that are keyed by request message identifier.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<ConcurrentDictionary<Guid, IResponseMessage>> LazyUnprocessedResponses = new Lazy<ConcurrentDictionary<Guid, IResponseMessage>>(() => new ConcurrentDictionary<Guid, IResponseMessage>(), LazyThreadSafetyMode.ExecutionAndPublication);
    }
}