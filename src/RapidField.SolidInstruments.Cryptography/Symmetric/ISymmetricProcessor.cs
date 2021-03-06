﻿// =================================================================================================================================
// Copyright (c) RapidField LLC. Licensed under the MIT License. See LICENSE.txt in the project root for license information.
// =================================================================================================================================

using System;
using System.Security;

namespace RapidField.SolidInstruments.Cryptography.Symmetric
{
    /// <summary>
    /// Provides facilities for encrypting and decrypting typed objects.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the object that can be encrypted and decrypted.
    /// </typeparam>
    public interface ISymmetricProcessor<T>
    {
        /// <summary>
        /// Decrypts the specified ciphertext.
        /// </summary>
        /// <param name="ciphertext">
        /// The ciphertext to decrypt.
        /// </param>
        /// <param name="key">
        /// The key derivation bits and algorithm specification used to transform the ciphertext.
        /// </param>
        /// <returns>
        /// The resulting plaintext object.
        /// </returns>
        /// <exception cref="SecurityException">
        /// An exception was raised during decryption or deserialization.
        /// </exception>
        T Decrypt(Byte[] ciphertext, SecureSymmetricKey key);

        /// <summary>
        /// Decrypts the specified ciphertext.
        /// </summary>
        /// <param name="ciphertext">
        /// The ciphertext to decrypt.
        /// </param>
        /// <param name="key">
        /// The cascading key derivation bits and algorithm specifications used to transform the ciphertext.
        /// </param>
        /// <returns>
        /// The resulting plaintext object.
        /// </returns>
        /// <exception cref="SecurityException">
        /// An exception was raised during decryption or deserialization.
        /// </exception>
        T Decrypt(Byte[] ciphertext, CascadingSymmetricKey key);

        /// <summary>
        /// Decrypts the specified ciphertext.
        /// </summary>
        /// <param name="ciphertext">
        /// The ciphertext to decrypt.
        /// </param>
        /// <param name="key">
        /// The key derivation bits used to transform the ciphertext.
        /// </param>
        /// <param name="algorithm">
        /// The algorithm specification used to transform the ciphertext.
        /// </param>
        /// <returns>
        /// The resulting plaintext object.
        /// </returns>
        /// <exception cref="SecurityException">
        /// An exception was raised during decryption or deserialization.
        /// </exception>
        T Decrypt(Byte[] ciphertext, SecureBuffer key, SymmetricAlgorithmSpecification algorithm);

        /// <summary>
        /// Encrypts the specified plaintext object.
        /// </summary>
        /// <param name="plaintextObject">
        /// The plaintext object to encrypt.
        /// </param>
        /// <param name="key">
        /// The key derivation bits and algorithm specification used to transform the object.
        /// </param>
        /// <returns>
        /// The resulting ciphertext.
        /// </returns>
        /// <exception cref="SecurityException">
        /// An exception was raised during encryption or serialization.
        /// </exception>
        Byte[] Encrypt(T plaintextObject, SecureSymmetricKey key);

        /// <summary>
        /// Encrypts the specified plaintext object.
        /// </summary>
        /// <param name="plaintextObject">
        /// The plaintext object to encrypt.
        /// </param>
        /// <param name="key">
        /// The cascading key derivation bits and algorithm specifications used to transform the object.
        /// </param>
        /// <returns>
        /// The resulting ciphertext.
        /// </returns>
        /// <exception cref="SecurityException">
        /// An exception was raised during encryption or serialization.
        /// </exception>
        Byte[] Encrypt(T plaintextObject, CascadingSymmetricKey key);

        /// <summary>
        /// Encrypts the specified plaintext object.
        /// </summary>
        /// <param name="plaintextObject">
        /// The plaintext object to encrypt.
        /// </param>
        /// <param name="key">
        /// The key derivation bits used to transform the object.
        /// </param>
        /// <param name="algorithm">
        /// The algorithm specification used to transform the object.
        /// </param>
        /// <returns>
        /// The resulting ciphertext.
        /// </returns>
        /// <exception cref="SecurityException">
        /// An exception was raised during encryption or serialization.
        /// </exception>
        Byte[] Encrypt(T plaintextObject, SecureBuffer key, SymmetricAlgorithmSpecification algorithm);
    }
}