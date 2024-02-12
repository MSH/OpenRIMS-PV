using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Models;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenRIMS.PV.Main.API.Infrastructure.Services
{
    public class XmlDocumentService : IXmlDocumentService
    {
        private readonly IRepositoryInt<DatasetXmlNode> _datasetXmlNodeRepository;

        DatasetInstance _e2bInstance;
        XmlDocument _xmlDocument;

        public XmlDocumentService(IRepositoryInt<DatasetXmlNode> datasetXmlNodeRepository)
        {
            _datasetXmlNodeRepository = datasetXmlNodeRepository ?? throw new ArgumentNullException(nameof(datasetXmlNodeRepository));
        }

        public async Task<ArtefactInfoModel> CreateE2BDocumentAsync(DatasetInstance e2bInstance)
        {
            if (e2bInstance.Dataset?.DatasetXml == null)
            {
                throw new ArgumentException("Unable to locate dataset XML structure", nameof(e2bInstance));
            }

            _e2bInstance = e2bInstance;
            _xmlDocument = new XmlDocument();

            XmlDeclaration xmlDeclaration = _xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            _xmlDocument.AppendChild(xmlDeclaration);

            XmlDocumentType doctype;
            doctype = _xmlDocument.CreateDocumentType("ichicsr", "-//ICHM2//DTD ICH ICSR Vers. 2.1//EN", "ich-icsr-v2.1.dtd", null);
            _xmlDocument.AppendChild(doctype);

            _xmlDocument.AppendChild(await PrepareRootNodeAsync(e2bInstance.Dataset.DatasetXml));

            var artefactModel = PrepareArtefactModel(e2bInstance.Id);
            SaveFormattedXML(artefactModel);

            return artefactModel;
        }

        private ArtefactInfoModel PrepareArtefactModel(long identifier = 0)
        {
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmsss");

            return new ArtefactInfoModel()
            {
                Path = Path.GetTempPath(),
                FileName = $"E2B{identifier}_{generatedDate}.xml"
            };
        }

        private void SaveFormattedXML(ArtefactInfoModel artefactModel)
        {
            WriteXMLContentToFile(artefactModel.FullPath, ConvertXMLToFormattedText());
        }

        private async Task<XmlNode> PrepareRootNodeAsync(DatasetXml xmlStructureForDataset)
        {
            var rootStructure = xmlStructureForDataset.ChildrenNodes.SingleOrDefault(c => c.NodeType == NodeType.RootNode);
            if (rootStructure == null)
            {
                throw new DomainException($"{nameof(xmlStructureForDataset)} Unable to locate root structure");
            }

            var rootNode = CreateXmlNodeWithAttributes(rootStructure);
            await ProcessNodeStructureChildrenAsync(rootStructure.Id, rootNode);
            return rootNode;
        }

        private async Task<XmlNode[]> PrepareXmlNodeAsync(DatasetXmlNode datasetXmlNode, DatasetInstanceSubValue[] subItemValues = null)
        {
            if (datasetXmlNode == null)
            {
                throw new ArgumentNullException(nameof(datasetXmlNode));
            }

            List<XmlNode> xmlNodes = new List<XmlNode>();

            switch (datasetXmlNode.NodeType)
            {
                case NodeType.RootNode:
                case NodeType.StandardNode:
                    var xmlStandardNode = CreateXmlNodeWithAttributes(datasetXmlNode);
                    if (datasetXmlNode.DatasetElement != null)
                    {
                        SetXmlNodeValueWithDatasetElement(xmlStandardNode, datasetXmlNode);
                    }

                    if (datasetXmlNode.DatasetElementSub != null)
                    {
                        SetXmlNodeValueWithSubValues(xmlStandardNode, datasetXmlNode, subItemValues);
                    }

                    await ProcessNodeStructureChildrenAsync(datasetXmlNode.Id, xmlStandardNode, subItemValues);

                    xmlNodes.Add(xmlStandardNode);
                    break;

                case NodeType.RepeatingNode:
                    if (datasetXmlNode.DatasetElement != null)
                    {
                        var sourceContexts = _e2bInstance.GetInstanceSubValuesContext(datasetXmlNode.DatasetElement.ElementName);
                        foreach (Guid sourceContext in sourceContexts)
                        {
                            var xmlRepeatingNode = CreateXmlNodeWithAttributes(datasetXmlNode);
                            var values = _e2bInstance.GetInstanceSubValues(datasetXmlNode.DatasetElement.ElementName, sourceContext);

                            await ProcessNodeStructureChildrenAsync(datasetXmlNode.Id, xmlRepeatingNode, values);

                            xmlNodes.Add(xmlRepeatingNode);
                        }
                    }
                    break;

                default:
                    break;
            }
            return xmlNodes.ToArray();
        }

        private async Task ProcessNodeStructureChildrenAsync(int datasetXmlNodeId, XmlNode parentNode, DatasetInstanceSubValue[] subItemValues = null)
        {
            var datasetXmlNode = await _datasetXmlNodeRepository.GetAsync(dx => dx.Id == datasetXmlNodeId,
                new string[] {
                    "ChildrenNodes.NodeAttributes.DatasetElement",
                    "ChildrenNodes.DatasetElement", "ChildrenNodes.DatasetElementSub",
                    "NodeAttributes.DatasetElement",
                    "DatasetElement", "DatasetElementSub"
                });

            if (datasetXmlNode.ChildrenNodes.Count > 0)
            {
                foreach (var datasetChildXmlNode in datasetXmlNode.ChildrenNodes)
                {
                    var childNodes = await PrepareXmlNodeAsync(datasetChildXmlNode, subItemValues);
                    foreach (var childNode in childNodes)
                    {
                        parentNode.AppendChild(childNode);
                    }
                }
            }
        }

        private XmlNode CreateXmlNodeWithAttributes(DatasetXmlNode datasetXmlNode)
        {
            XmlNode xmlNode = _xmlDocument.CreateElement(datasetXmlNode.NodeName, "");
            if (datasetXmlNode.NodeAttributes.Count == 0) return xmlNode;

            foreach (DatasetXmlAttribute datasetXmlAttribute in datasetXmlNode.NodeAttributes)
            {
                XmlAttribute xmlAttribute = _xmlDocument.CreateAttribute(datasetXmlAttribute.AttributeName);

                if (datasetXmlAttribute.DatasetElement != null)
                {
                    var value = _e2bInstance.GetInstanceValue(datasetXmlAttribute.DatasetElement.ElementName);
                    if (value.IndexOf("=") > -1)
                    {
                        value = value.Substring(0, value.IndexOf("="));
                    }
                    xmlAttribute.InnerText = value;
                }
                else
                {
                    xmlAttribute.InnerText = datasetXmlAttribute.AttributeValue;
                }
                xmlNode.Attributes.Append(xmlAttribute);
            }

            return xmlNode;
        }

        private void SetXmlNodeValueWithDatasetElement(XmlNode xmlNode, DatasetXmlNode datasetXmlNode)
        {
            var value = _e2bInstance.GetInstanceValue(datasetXmlNode.DatasetElement.ElementName);
            if (value?.IndexOf("=") > -1)
            {
                value = value.Substring(0, value.IndexOf("="));
            }
            xmlNode.InnerText = value;
        }

        private void SetXmlNodeValueWithSubValues(XmlNode xmlNode, DatasetXmlNode datasetXmlNode, DatasetInstanceSubValue[] subItemValues)
        {
            var subvalue = subItemValues?.SingleOrDefault(siv => siv.DatasetElementSub.Id == datasetXmlNode.DatasetElementSub.Id);
            if (subvalue != null)
            {
                var value = subvalue.InstanceValue;
                if (value.IndexOf("=") > -1)
                {
                    value = value.Substring(0, value.IndexOf("="));
                }
                xmlNode.InnerText = value;
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(datasetXmlNode.DatasetElementSub.DefaultValue))
                {
                    xmlNode.InnerText = datasetXmlNode.DatasetElementSub.DefaultValue;
                }
                else
                {
                    xmlNode.InnerText = string.Empty;
                }
            }
        }

        private void WriteXMLContentToFile(string fileName, string xmlText)
        {
            using (TextWriter streamWriter = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                _xmlDocument.Save(streamWriter);
            }
        }

        private string ConvertXMLToFormattedText()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                Encoding = Encoding.UTF8
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                _xmlDocument.Save(writer);
            }

            return sb.ToString();
        }
    }
}