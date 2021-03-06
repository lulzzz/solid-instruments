﻿// =================================================================================================================================
// Copyright (c) RapidField LLC. Licensed under the MIT License. See LICENSE.txt in the project root for license information.
// =================================================================================================================================

using System;

namespace RapidField.SolidInstruments.Prototype.Contracts.Models
{
    /// <summary>
    /// Represents an integer number belonging to a specific number series.
    /// </summary>
    public interface INumberSeriesNumber
    {
        /// <summary>
        /// Gets a unique identifier for the entity.
        /// </summary>
        Guid Identifier
        {
            get;
        }

        /// <summary>
        /// Gets a unique identifier for the associated number.
        /// </summary>
        Guid NumberIdentifier
        {
            get;
        }

        /// <summary>
        /// Gets a unique identifier for the associated number series.
        /// </summary>
        Guid NumberSeriesIdentifier
        {
            get;
        }
    }
}