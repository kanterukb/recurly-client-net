﻿using System;
using System.Collections.Generic;
using System.Xml;

namespace Recurly
{
    public class AddOn : RecurlyEntity
    {
        public enum Type
        {
            Fixed,
            Usage
        }

        public string PlanCode { get; set; }
        public string AddOnCode { get; set; }
        public string Name { get; set; }
        public int DefaultQuantity { get; set; }
        public bool? DisplayQuantityOnHostedPage { get; set; }
        public string TaxCode { get; set; }
        public bool? Optional { get; set; }
        public string AccountingCode { get; set; }
        public long? MeasuredUnitId { get; set; }
        public Type? AddOnType { get; set; }
        public Usage.Type? UsageType { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private Dictionary<string, int> _unitAmountInCents;
        /// <summary>
        /// A dictionary of currencies and values for the add-on amount
        /// </summary>
        public Dictionary<string, int> UnitAmountInCents
        {
            get { return _unitAmountInCents ?? (_unitAmountInCents = new Dictionary<string, int>()); }
        }

        private const string UrlPrefix = "/plans/";
        private const string UrlPostfix = "/add_ons/";

        #region Constructors
        internal AddOn()
        {
        }

        internal AddOn(XmlReader xmlReader)
        {
            ReadXml(xmlReader);
        }

        internal AddOn(string planCode, string addOnCode, string name)
        {
            PlanCode = planCode;
            AddOnCode = addOnCode;
            Name = name;
        }

        #endregion

        /// <summary>
        /// Creates an addon
        /// </summary>
        public void Create()
        {
            Client.Instance.PerformRequest(Client.HttpRequestMethod.Post,
                UrlPrefix + Uri.EscapeUriString(PlanCode) + UrlPostfix,
                WriteXml,
                ReadXml);
        }

        /// <summary>
        /// Update an existing add on in Recurly
        /// </summary>
        public void Update()
        {
            Client.Instance.PerformRequest(Client.HttpRequestMethod.Put,
                UrlPrefix + Uri.EscapeUriString(PlanCode) + UrlPostfix + Uri.EscapeUriString(AddOnCode),
                WriteXml,
                ReadXml);
        }

        /// <summary>
        /// Deletes this add on, making it inactive
        /// </summary>
        public void Delete()
        {
            Client.Instance.PerformRequest(Client.HttpRequestMethod.Delete,
                UrlPrefix + Uri.EscapeUriString(PlanCode) + UrlPostfix + Uri.EscapeUriString(AddOnCode));
        }


        #region Read and Write XML documents

        internal void ReadXmlUnitAmount(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.Name == "unit_amount_in_cents" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    UnitAmountInCents.Remove(reader.Name);
                    UnitAmountInCents.Add(reader.Name, reader.ReadElementContentAsInt());
                }
            }
        }

        internal override void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                // End of account element, get out of here
                if (reader.Name == "add_on" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType != XmlNodeType.Element) continue;

                switch (reader.Name)
                {
                    case "add_on_code":
                        AddOnCode = reader.ReadElementContentAsString();
                        break;

                    case "accounting_code":
                        AccountingCode = reader.ReadElementContentAsString();
                        break;

                    case "name":
                        Name = reader.ReadElementContentAsString();
                        break;

                    case "display_quantity_on_hosted_page":
                        DisplayQuantityOnHostedPage = reader.ReadElementContentAsBoolean();
                        break;

                    case "default_quantity":
                        DefaultQuantity = reader.ReadElementContentAsInt();
                        break;

                    case "optional":
                        Optional = reader.ReadElementContentAsBoolean();
                        break;

                    case "created_at":
                        //CreatedAt = reader.ReadElementContentAsDateTime();
                        CreatedAt = (DateTime)reader.ReadElementContentAs(typeof(DateTime), null);
                        break;

                    case "updated_at":
                        //UpdatedAt = reader.ReadElementContentAsDateTime();
                        UpdatedAt = (DateTime)reader.ReadElementContentAs(typeof(DateTime), null);
                        break;

                    case "unit_amount_in_cents":
                        ReadXmlUnitAmount(reader);
                        break;

                    case "tax_code":
                        TaxCode = reader.ReadElementContentAsString();
                        break;

                    case "add_on_type":
                        AddOnType = reader.ReadElementContentAsString().ParseAsEnum<Type>();
                        break;

                    case "usage_type":
                        UsageType = reader.ReadElementContentAsString().ParseAsEnum<Usage.Type>();
                        break;
                }
            }
        }

        internal override void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("add_on");

            xmlWriter.WriteElementString("add_on_code", AddOnCode);
            xmlWriter.WriteElementString("name", Name);
            xmlWriter.WriteElementString("default_quantity", DefaultQuantity.AsString());
            xmlWriter.WriteElementString("accounting_code", AccountingCode);

            if (AddOnType.HasValue)
                xmlWriter.WriteElementString("add_on_type", AddOnType.Value.ToString().EnumNameToTransportCase());

            if (UsageType.HasValue)
                xmlWriter.WriteElementString("usage_type", UsageType.Value.ToString().EnumNameToTransportCase());

            if (MeasuredUnitId.HasValue)
                xmlWriter.WriteElementString("measured_unit_id", MeasuredUnitId.ToString());

            if (DisplayQuantityOnHostedPage.HasValue)
                xmlWriter.WriteElementString("display_quantity_on_hosted_page", DisplayQuantityOnHostedPage.Value.AsString());

            if (Optional.HasValue)
                xmlWriter.WriteElementString("optional", Optional.Value.AsString());

            xmlWriter.WriteIfCollectionHasAny("unit_amount_in_cents", UnitAmountInCents, pair => pair.Key,
                pair => pair.Value.AsString());

            xmlWriter.WriteEndElement();
        }

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            return "Recurly Plan: " + PlanCode;
        }

        public override bool Equals(object obj)
        {
            var plan = obj as Plan;
            return plan != null && Equals(plan);
        }

        public bool Equals(Plan plan)
        {
            return PlanCode == plan.PlanCode;
        }
        #endregion
    }
}
