﻿// =================================================================================================================================
// Copyright (c) RapidField LLC. Licensed under the MIT License. See LICENSE.txt in the project root for license information.
// =================================================================================================================================

using RapidField.SolidInstruments.Core;
using RapidField.SolidInstruments.Core.ArgumentValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace RapidField.SolidInstruments.ObjectComposition
{
    /// <summary>
    /// Manages object creation, storage, resolution and disposal for a related group of factory-produced instances.
    /// </summary>
    /// <remarks>
    /// <see cref="FactoryProducedInstanceGroup" /> is the default implementation of <see cref="IFactoryProducedInstanceGroup" />.
    /// </remarks>
    public class FactoryProducedInstanceGroup : Instrument, IFactoryProducedInstanceGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryProducedInstanceGroup" /> class.
        /// </summary>
        /// <param name="factory">
        /// A factory that creates new object instances for the group.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" /> is <see langword="null" />.
        /// </exception>
        public FactoryProducedInstanceGroup(IObjectFactory factory)
            : base()
        {
            Factory = factory.RejectIf().IsNull(nameof(factory)).TargetArgument;
            Instances = new Dictionary<Type, Object>();
        }

        /// <summary>
        /// Returns the instance of specified type that is managed by the current <see cref="IFactoryProducedInstanceGroup" />.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the instance to return.
        /// </typeparam>
        /// <returns>
        /// The instance of specified type that is managed by the current <see cref="IFactoryProducedInstanceGroup" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <typeparamref name="T" /> is not a supported type for the group.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The object is disposed.
        /// </exception>
        /// <exception cref="ObjectProductionException">
        /// An exception was raised during object production.
        /// </exception>
        public T Get<T>()
            where T : class
        {
            var instanceType = typeof(T);

            using (var controlToken = StateControl.Enter())
            {
                RejectIfDisposed();

                if (Instances.TryGetValue(instanceType, out var extantInstance))
                {
                    return (extantInstance as T);
                }

                var newInstance = Factory.Produce(instanceType) as T;
                Instances.Add(instanceType, newInstance);
                ReferenceManager.AddObject(newInstance);
                return newInstance;
            }
        }

        /// <summary>
        /// Returns the lazily-initialized instance of specified type that is managed by the current
        /// <see cref="IFactoryProducedInstanceGroup" />.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the lazily-initialized instance to return.
        /// </typeparam>
        /// <returns>
        /// The lazily-initialized instance of specified type that is managed by the current
        /// <see cref="IFactoryProducedInstanceGroup" />.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The object is disposed.
        /// </exception>
        public Lazy<T> GetLazy<T>()
            where T : class
        {
            RejectIfDisposed();
            return new Lazy<T>(() => Get<T>(), LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Releases all resources consumed by the current <see cref="FactoryProducedInstanceGroup" />.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether or not managed resources should be released.
        /// </param>
        protected override void Dispose(Boolean disposing)
        {
            try
            {
                if (disposing)
                {
                    ReferenceManager.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Gets the types of the instances that are managed by the current <see cref="FactoryProducedInstanceGroup" />.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The object is disposed.
        /// </exception>
        public IEnumerable<Type> InstanceTypes
        {
            get
            {
                RejectIfDisposed();
                return Factory.SupportedProductTypes;
            }
        }

        /// <summary>
        /// Represents a factory that creates new object instances for the group.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IObjectFactory Factory;

        /// <summary>
        /// Represents the instances that are managed by the current <see cref="FactoryProducedInstanceGroup" />.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IDictionary<Type, Object> Instances;

        /// <summary>
        /// Represents a utility that disposes of the object references that are managed by the current
        /// <see cref="FactoryProducedInstanceGroup" />.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IReferenceManager ReferenceManager = new ReferenceManager();
    }
}