using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Utils
{
    /// <summary>
    /// This class defines different constants that are used accros the library
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// This String contains all possible characters of the tryte alphabet
        /// </summary>                              
        public static readonly string TryteAlphabet = "9ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// The maximum seed length
        /// </summary>
        public static readonly int SeedLengthMax = 81;


        public static int TAG_LENGTH = 27;
        
        /**
        * The length of an address without checksum
        */
        public static int ADDRESS_LENGTH_WITHOUT_CHECKSUM = 81;

        /**
         * The length of an address with checksum
         */
        public static int ADDRESS_LENGTH_WITH_CHECKSUM = 90;

        /**
         * The length of an message
         */
        public static int MESSAGE_LENGTH = 2187;

        public static readonly string EmptyTag = "999999999999999999999999999";

        /// <summary>
        /// This String represents the empty hash consisting of '9'
        /// </summary>
        public static readonly string EmptyHash = "999999999999999999999999999999999999999999999999999999999999999999999999999999999";

        /// <summary>
        /// The length of an address without checksum
        /// </summary>
        public static readonly int AddressLengthWithoutChecksum = 81;

        /// <summary>
        /// The address length with checksum
        /// </summary>
        public static readonly int AddressLengthWithChecksum = 90;

        public static readonly string INVALID_TRYTES_INPUT_ERROR = "Invalid trytes provided.";
        public static readonly string INVALID_HASHES_INPUT_ERROR = "Invalid hashes provided.";
        public static readonly string INVALID_TAIL_HASH_INPUT_ERROR = "Invalid tail hash provided.";
        public static readonly string INVALID_SEED_INPUT_ERROR = "Invalid seed provided.";
        public static readonly string INVALID_SECURITY_LEVEL_INPUT_ERROR = "Invalid security level provided.";
        public static readonly string INVALID_ATTACHED_TRYTES_INPUT_ERROR = "Invalid attached trytes provided.";
        public static readonly string INVALID_TRANSFERS_INPUT_ERROR = "Invalid transfers provided.";
        public static readonly string INVALID_ADDRESSES_INPUT_ERROR = "Invalid addresses provided.";
        public static readonly string INVALID_INPUT_ERROR = "Invalid input provided.";
                               
        public static readonly string INVALID_BUNDLE_ERROR = "Invalid bundle.";
        public static readonly string INVALID_BUNDLE_SUM_ERROR = "Invalid bundle sum.";
        public static readonly string INVALID_BUNDLE_HASH_ERROR = "Invalid bundle hash.";
        public static readonly string INVALID_SIGNATURES_ERROR = "Invalid signatures.";
        public static readonly string INVALID_VALUE_TRANSFER_ERROR = "Invalid value transfer: the transfer does not require a signature.";
                               
        public static readonly string NOT_ENOUGH_BALANCE_ERROR = "Not enough balance.";
        public static readonly string NO_REMAINDER_ADDRESS_ERROR = "No remainder address defined.";
                               
        public static readonly string GET_TRYTES_RESPONSE_ERROR = "Get trytes response was null.";
        public static readonly string GET_BUNDLE_RESPONSE_ERROR = "Get bundle response was null.";
        public static readonly string GET_INCLUSION_STATE_RESPONSE_ERROR = "Get inclusion state response was null.";
                               
        public static readonly string SENDING_TO_USED_ADDRESS_ERROR = "Sending to a used address.";
        public static readonly string PRIVATE_KEY_REUSE_ERROR = "Private key reuse detect!";
        public static readonly string SEND_TO_INPUTS_ERROR = "Send to inputs!";

    }
}
