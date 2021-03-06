﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Reciprocal License (Ms-RL) license. See LICENSE file in the project root for full license information.

namespace IronPigeon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using PCLCrypto;
    using Validation;

    /// <summary>
    /// A self-signed description of an endpoint including public signing and encryption keys.
    /// </summary>
    [DataContract]
    public class AddressBookEntry
    {
        /// <summary>
        /// The Content-Type that identifies a blob containing a serialized instance of this type.
        /// </summary>
        public const string ContentType = "ironpigeon/addressbookentry";

        /// <summary>
        /// Gets or sets the serialized <see cref="Endpoint"/>.
        /// </summary>
        [DataMember]
        public byte[] SerializedEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the hash algorithm used to sign the serialized endpoint.
        /// </summary>
        [DataMember]
        public string HashAlgorithmName { get; set; }

        /// <summary>
        /// Gets or sets the signature of the <see cref="SerializedEndpoint"/> bytes,
        /// as signed by the private counterpart to the
        /// public key stored in <see cref="Endpoint.SigningKeyPublicMaterial"/>.
        /// </summary>
        /// <remarks>
        /// The point of this signature is to prove that the owner (the signer)
        /// has approved of the encryption key that is also included in the endpoint
        /// metadata.  This mitigates a rogue address book entry that claims to
        /// be someone (a victim) by using their public signing key, but with an
        /// encryption key that the attacker controls the private key to.
        /// </remarks>
        [DataMember]
        public byte[] Signature { get; set; }

        /// <summary>
        /// Deserializes an endpoint from an address book entry and validates that the signatures are correct.
        /// </summary>
        /// <returns>The deserialized endpoint.</returns>
        /// <exception cref="BadAddressBookEntryException">Thrown if the signatures are invalid.</exception>
        public Endpoint ExtractEndpoint()
        {
            var reader = new BinaryReader(new MemoryStream(this.SerializedEndpoint));
            Endpoint endpoint;
            try
            {
                endpoint = reader.DeserializeDataContract<Endpoint>();
            }
            catch (SerializationException ex)
            {
                throw new BadAddressBookEntryException(ex.Message, ex);
            }

            try
            {
                if (!CryptoProviderExtensions.VerifySignatureWithTolerantHashAlgorithm(endpoint.SigningKeyPublicMaterial, this.SerializedEndpoint, this.Signature, this.HashAlgorithmName != null ? (AsymmetricAlgorithm?)CryptoProviderExtensions.GetSignatureProvider(this.HashAlgorithmName) : null))
                {
                    throw new BadAddressBookEntryException(Strings.AddressBookEntrySignatureDoesNotMatch);
                }
            }
            catch (Exception ex)
            { // all those platform-specific exceptions that aren't available to portable libraries.
                throw new BadAddressBookEntryException(Strings.AddressBookEntrySignatureDoesNotMatch, ex);
            }

            return endpoint;
        }
    }
}
