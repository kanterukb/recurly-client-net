﻿using System.Xml;

namespace Recurly
{
    public class SubscriptionAddOn : RecurlyEntity
    {
        public string AddOnCode { get; set; }
        public int UnitAmountInCents { get; set; }
        public int Quantity { get; set; }

        public SubscriptionAddOn(XmlReader reader)
        {
            ReadXml(reader);
        }

        public SubscriptionAddOn(string addOnCode, int unitAmountInCents, int quantity = 1)
        {
            AddOnCode = addOnCode;
            UnitAmountInCents = unitAmountInCents;
            Quantity = quantity;
        }

        internal override void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.Name == "subscription_add_on" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType != XmlNodeType.Element) continue;

                switch (reader.Name)
                {
                    case "add_on_code":
                        AddOnCode = reader.ReadElementContentAsString();
                        break;

                    case "quantity":
                        Quantity = reader.ReadElementContentAsInt();
                        break;

                    case "unit_amount_in_cents":
                        UnitAmountInCents = reader.ReadElementContentAsInt();
                        break;
                }
            }
        }

        internal override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("subscription_add_on");

            writer.WriteElementString("add_on_code", AddOnCode);
            writer.WriteElementString("quantity", Quantity.AsString());
            writer.WriteElementString("unit_amount_in_cents", UnitAmountInCents.AsString());

            writer.WriteEndElement();
        }
    }
}
