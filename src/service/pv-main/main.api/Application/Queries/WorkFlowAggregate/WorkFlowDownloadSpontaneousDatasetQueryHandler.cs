using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;

namespace OpenRIMS.PV.Main.API.Application.Queries.WorkFlowAggregate
{
    public class WorkFlowDownloadSpontaneousDatasetQueryHandler
        : IRequestHandler<WorkFlowDownloadSpontaneousDatasetQuery, ArtifactDto>
    {
        private readonly IRepositoryInt<Dataset> _datasetRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<ReportInstanceMedication> _reportInstanceMedicationRepository;
        private readonly IExcelDocumentService _excelDocumentService;
        private readonly ILogger<WorkFlowDownloadSpontaneousDatasetQueryHandler> _logger;

        public WorkFlowDownloadSpontaneousDatasetQueryHandler(
            IRepositoryInt<Dataset> datasetRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<ReportInstanceMedication> reportInstanceMedicationRepository,
            IExcelDocumentService excelDocumentService,
            ILogger<WorkFlowDownloadSpontaneousDatasetQueryHandler> logger)
        {
            _datasetRepository = datasetRepository ?? throw new ArgumentNullException(nameof(datasetRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _reportInstanceMedicationRepository = reportInstanceMedicationRepository ?? throw new ArgumentNullException(nameof(reportInstanceMedicationRepository));
            _excelDocumentService = excelDocumentService ?? throw new ArgumentNullException(nameof(excelDocumentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ArtifactDto> Handle(WorkFlowDownloadSpontaneousDatasetQuery message, CancellationToken cancellationToken)
        {
            var model = PrepareFileModel();

            _excelDocumentService.CreateDocument(model);

            await PrepareDataForReportSheetAsync();
            await PrepareSubDataForReportSheetAsync();

            return model;
        }

        private async Task PrepareDataForReportSheetAsync()
        {
            var data = new List<List<string>>();

            var orderby = Extensions.GetOrderBy<ReportInstance>("Id", "asc");
            var reportsFromRepo = await _reportInstanceRepository.ListAsync(ri => ri.WorkFlow.WorkFlowGuid.ToString() == "4096D0A3-45F7-4702-BDA1-76AEDE41B986" && ri.Activities.Any(a => a.QualifiedName == "Confirm Report Data" && a.CurrentStatus.Description != "DELETED"), orderby, new string[] {
                "Medications"
            });
            if (reportsFromRepo != null)
            {
                var dataset = await _datasetRepository.GetAsync(d => d.DatasetName == "ICSR Spontaneous Report",
                    new string[] {
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType"
                    });

                var headers = PrepareReportHeader(dataset);
                data.Add(headers);

                foreach (var reportInstance in reportsFromRepo)
                {
                    var datasetInstance = await _datasetInstanceRepository.GetAsync(di => di.DatasetInstanceGuid == reportInstance.ContextGuid,
                        new string[] {
                            "DatasetInstanceValues",
                        });

                    var row = PrepareReportRow(dataset, datasetInstance);
                    data.Add(row);
                }
            }
            _excelDocumentService.AddSheet("Reports", data);
        }

        private async Task PrepareSubDataForReportSheetAsync()
        {
            var data = new List<List<string>>();

            var orderby = Extensions.GetOrderBy<ReportInstance>("Id", "asc");
            var reportsFromRepo = await _reportInstanceRepository.ListAsync(ri => ri.WorkFlow.WorkFlowGuid.ToString() == "4096D0A3-45F7-4702-BDA1-76AEDE41B986" && ri.Activities.Any(a => a.QualifiedName == "Confirm Report Data" && a.CurrentStatus.Description != "DELETED"), orderby, new string[] {
                "Medications"
            });
            if (reportsFromRepo != null)
            {
                var dataset = await _datasetRepository.GetAsync(d => d.DatasetName == "ICSR Spontaneous Report",
                    new string[] {
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType",
                    "DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType"
                    });

                foreach (var category in dataset.DatasetCategories)
                {
                    foreach (var element in category.DatasetCategoryElements.Where(dce => dce.DatasetElement.System == false && dce.DatasetElement.Field.FieldType.Description == "Table"))
                    {
                        data.Clear();

                        var headers = PrepareReportSubHeader(element);
                        data.Add(headers);

                        foreach (var reportInstance in reportsFromRepo)
                        {
                            var datasetInstance = await _datasetInstanceRepository.GetAsync(di => di.DatasetInstanceGuid == reportInstance.ContextGuid,
                                new string[] { "DatasetInstanceValues.DatasetElement.Field.FieldType"
                                    , "DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType"
                                });

                            var value = datasetInstance.DatasetInstanceValues.SingleOrDefault(div1 => div1.DatasetElement.Id == element.DatasetElement.Id);

                            if(value != null)
                            {
                                if (value.DatasetInstanceSubValues.Count > 0)
                                {
                                    var rows = PrepareReportSubRow(datasetInstance, element, value);
                                    foreach(var row in rows)
                                    {
                                        data.Add(row);
                                    }
                                }
                            }
                        }
                        _excelDocumentService.AddSheet(element.DatasetElement.ElementName, data);
                    }
                }
            }
        }

        private ArtifactDto PrepareFileModel()
        {
            var model = new ArtifactDto();
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmss");

            model.Path = Path.GetTempPath();
            model.FileName = $"{Path.GetRandomFileName()}_{generatedDate}.xlsx";
            return model;
        }

        private List<string> PrepareReportHeader(Dataset dataset)
        {
            var headers = new List<string>
            {
                "Report Unique Id",
            };

            foreach (var category in dataset.DatasetCategories)
            {
                foreach (var element in category.DatasetCategoryElements.Where(dce => dce.DatasetElement.System == false && dce.DatasetElement.Field.FieldType.Description != "Table"))
                {
                    headers.Add(element.DatasetElement.ElementName);
                }
            }

            return headers;
        }

        private List<string> PrepareReportSubHeader(DatasetCategoryElement element)
        {
            var headers = new List<string>
            {
                "Report Unique Id",
            };

            foreach (var subElement in element.DatasetElement.DatasetElementSubs.Where(des1 => des1.System == false).OrderBy(des2 => des2.Id))
            {
                headers.Add(subElement.ElementName);
            }

            return headers;
        }

        private List<string> PrepareReportRow(Dataset dataset, DatasetInstance datasetInstance)
        {
            var row = new List<string>
            {
                datasetInstance.DatasetInstanceGuid.ToString()
            };

            foreach (var category in dataset.DatasetCategories)
            {
                foreach (var element in category.DatasetCategoryElements.Where(dce => dce.DatasetElement.System == false && dce.DatasetElement.Field.FieldType.Description != "Table"))
                {
                    var value = datasetInstance.DatasetInstanceValues.SingleOrDefault(div1 => div1.DatasetElement.Id == element.DatasetElement.Id);
                    row.Add(value == null ? string.Empty : value.InstanceValue);
                }
            }

            return row;
        }

        private List<List<string>> PrepareReportSubRow(DatasetInstance datasetInstance, DatasetCategoryElement element, DatasetInstanceValue value)
        {
            var rows = new List<List<string>> { };

            var contexts = value.DatasetInstance.GetInstanceSubValuesContext(value.DatasetElement.ElementName);

            foreach (var context in contexts)
            {
                var subValues = value.DatasetInstance.GetInstanceSubValues(value.DatasetElement.ElementName, context);

                var row = new List<string>
                    {
                        datasetInstance.DatasetInstanceGuid.ToString()
                    };

                foreach (var subElement in element.DatasetElement.DatasetElementSubs.Where(des1 => des1.System == false).OrderBy(des2 => des2.Id))
                {
                    var subValue = subValues.SingleOrDefault(disv => disv.DatasetElementSub.Id == subElement.Id);
                    row.Add(subValue == null ? string.Empty : subValue.InstanceValue);
                }

                rows.Add(row);
            }

            return rows;
        }

        private List<string> PrepareTerminologyReportHeader()
        {
            var headers = new List<string>
            {
                "Report Unique Id",
                "MedDra Term",
                "Terminology Type",
                "MedDra Code"
            };

            return headers;
        }

        private List<string> PrepareCausalityReportHeader()
        {
            var headers = new List<string>
            {
                "Report Unique Id",
                "MedDra Term",
                "Medication",
                "Naranjo Causality",
                "WHO Causality"
            };

            return headers;
        }
    }
}
