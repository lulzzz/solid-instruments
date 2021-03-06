﻿// =================================================================================================================================
// Copyright (c) RapidField LLC. Licensed under the MIT License. See LICENSE.txt in the project root for license information.
// =================================================================================================================================

using Microsoft.Extensions.Configuration;
using RapidField.SolidInstruments.Collections;
using RapidField.SolidInstruments.Core;
using RapidField.SolidInstruments.Core.Concurrency;
using System;

namespace RapidField.SolidInstruments.ObjectComposition.UnitTests
{
    /// <summary>
    /// Represents an <see cref="ObjectFactory" /> derivative that is used for testing.
    /// </summary>
    internal sealed class SimulatedInstrumentFactory : ObjectFactory<Instrument>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatedInstrumentFactory" /> class.
        /// </summary>
        /// <param name="applicationConfiguration">
        /// Configuration information for the application.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="applicationConfiguration" /> is <see langword="null" />.
        /// </exception>
        public SimulatedInstrumentFactory(IConfiguration applicationConfiguration)
            : base(applicationConfiguration)
        {
            return;
        }

        /// <summary>
        /// Configures the current <see cref="SimulatedInstrumentFactory" />.
        /// </summary>
        /// <param name="configuration">
        /// Configuration information for the current <see cref="SimulatedInstrumentFactory" />.
        /// </param>
        protected override void Configure(ObjectFactoryConfiguration<Instrument> configuration)
        {
            configuration.StateControlMode = ConcurrencyControlMode.SingleThreadSpinLock;
            configuration.ProductionFunctions
                .Define(() => new SimulatedInstrument(configuration.StateControlMode))
                .Define(() => new PinnedStructureArray<Int16>(3))
                .Define(() => new CircularBuffer<Int32>(5));
        }

        /// <summary>
        /// Releases all resources consumed by the current <see cref="SimulatedInstrumentFactory" />.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether or not managed resources should be released.
        /// </param>
        protected override void Dispose(Boolean disposing) => base.Dispose(disposing);
    }
}