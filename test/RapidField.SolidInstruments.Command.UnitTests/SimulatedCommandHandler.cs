﻿// =================================================================================================================================
// Copyright (c) RapidField LLC. Licensed under the MIT License. See LICENSE.txt in the project root for license information.
// =================================================================================================================================

using RapidField.SolidInstruments.Core.Concurrency;
using System;

namespace RapidField.SolidInstruments.Command.UnitTests
{
    /// <summary>
    /// Represents a <see cref="CommandHandler{TCommand}" /> derivative that is used for testing.
    /// </summary>
    internal sealed class SimulatedCommandHandler : CommandHandler<SimulatedCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatedCommandHandler" /> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mediator" /> is <see langword="null" />.
        /// </exception>
        public SimulatedCommandHandler(ICommandMediator mediator)
            : base(mediator)
        {
            return;
        }

        /// <summary>
        /// Releases all resources consumed by the current <see cref="SimulatedCommandHandler" />.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether or not managed resources should be released.
        /// </param>
        protected sealed override void Dispose(Boolean disposing) => base.Dispose(disposing);

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
        protected sealed override void Process(SimulatedCommand command, ICommandMediator mediator, ConcurrencyControlToken controlToken) => command.IsProcessed = true;
    }
}