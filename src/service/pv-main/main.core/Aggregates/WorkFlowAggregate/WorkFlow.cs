using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using System;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
	public class WorkFlow : EntityBase
	{
		protected WorkFlow()
		{
			_activities = new List<Activity>();
			_reportInstances = new List<ReportInstance>();
		}

		public WorkFlow(string description, Guid workFlowGuid)
		{
			WorkFlowGuid = workFlowGuid;
			Description = description;

			_activities = new List<Activity>();
			_reportInstances = new List<ReportInstance>();
		}

		public string Description { get; private set; }
		public Guid WorkFlowGuid { get; private set; }

		private List<Activity> _activities;
		public IEnumerable<Activity> Activities => _activities.AsReadOnly();

		private List<ReportInstance> _reportInstances;
		public IEnumerable<ReportInstance> ReportInstances => _reportInstances.AsReadOnly();
	}
}