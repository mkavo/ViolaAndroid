using System;
using System.Collections.Generic;

namespace Viola
{
    public sealed class CallSlipDeliveryTypes
    {
        private readonly String _name;
        private static readonly Dictionary<string, CallSlipDeliveryTypes> Instance = new Dictionary<string, CallSlipDeliveryTypes>();


        public CallSlipDeliveryTypes(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                Instance[string.Empty] = this;
            else
            {
                _name = name;
                Instance[name] = this;
            }
        }

        public static explicit operator CallSlipDeliveryTypes(string str)
        {
            CallSlipDeliveryTypes result;
            if (Instance.TryGetValue(str, out result))
                return result;
            else
                throw new InvalidCastException();
        }

        /// <summary>
        /// FJ‰rrut
        /// </summary>
        public static readonly CallSlipDeliveryTypes Unknown = new CallSlipDeliveryTypes(string.Empty);

        /// <summary>
        /// FJ‰rrut
        /// </summary>
        public static readonly CallSlipDeliveryTypes ILLOUT = new CallSlipDeliveryTypes("FjÅ‰rrut fysiskt material");

        /// <summary>
        /// Kopia
        /// </summary>
        public static readonly CallSlipDeliveryTypes COPY = new CallSlipDeliveryTypes("FjÅ‰rrut kopia");

        /// <summary>
        /// Magasinsbest‰llning
        /// </summary>
        public static readonly CallSlipDeliveryTypes STACKCALL = new CallSlipDeliveryTypes("Magasin");

        /// <summary>
        /// Direktleverans
        /// </summary>
        public static readonly CallSlipDeliveryTypes DIRECTDELIVERY = new CallSlipDeliveryTypes("Direktleverans");

        /// <summary>
        /// Sakanade
        /// </summary>
        public static readonly CallSlipDeliveryTypes MISSING = new CallSlipDeliveryTypes("Saknade");

        /// <summary>
        /// Arkiverade poster
        /// </summary>
        public static readonly CallSlipDeliveryTypes ARCHIVE = new CallSlipDeliveryTypes("Arkiverade");


        public override String ToString()
        {
            return _name;
        }

    }
}
