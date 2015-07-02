using System;
using Newtonsoft.Json;

namespace Viola
{
    public class CallSlipDeliveryTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var deliveryType = value as CallSlipDeliveryTypes;
            writer.WriteValue(deliveryType != null ? deliveryType.ToString() : string.Empty);

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (typeof(CallSlipDeliveryTypes).IsAssignableFrom(objectType))
                return (CallSlipDeliveryTypes)(reader.Value as string);
            else return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(CallSlipDeliveryTypes).IsAssignableFrom(objectType); //Verkar vara den gängse lössningen
            //return objectType == typeof (CallSlipDeliveryTypes);
        }
    }
}