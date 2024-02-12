using OpenRIMS.PV.Main.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate
{
    public class ReportInstanceTask 
        : AuditedEntityBase
	{
        public int ReportInstanceId { get; private set; }

        public TaskDetail TaskDetail { get; private set; }

        public int TaskTypeId { get; private set; }
        public int TaskStatusId { get; private set; }

        private readonly List<ReportInstanceTaskComment> _comments;
        public IReadOnlyCollection<ReportInstanceTaskComment> Comments => _comments;

        public virtual ReportInstance ReportInstance { get; private set; }

        protected ReportInstanceTask()
        {
            _comments = new List<ReportInstanceTaskComment>();
        }

        public ReportInstanceTask(TaskDetail taskDetail, TaskType taskType, TaskStatus taskStatus)
        {
            TaskDetail = taskDetail;
            TaskTypeId = taskType.Id;
            TaskStatusId = taskStatus.Id;
        }

        public ReportInstanceTaskComment AddComment(string comment)
        {
            var newTaskComment = new ReportInstanceTaskComment(comment);
            _comments.Add(newTaskComment);
            return newTaskComment;
        }

        public void ChangeDetails(TaskDetail taskDetail)
        {
            TaskDetail = taskDetail;
        }

        public void ChangeTaskStatusToAcknowledged()
        {
            TaskStatusId = TaskStatus.Acknowledged.Id;
        }

        public void ChangeTaskStatusToOnHold()
        {
            TaskStatusId = TaskStatus.OnHold.Id;
        }

        public void ChangeTaskStatusToDone()
        {
            TaskStatusId = TaskStatus.Done.Id;
        }

        public void ChangeTaskStatusToCompleted()
        {
            TaskStatusId = TaskStatus.Completed.Id;
        }

        public void ChangeTaskStatusToCancelled()
        {
            TaskStatusId = TaskStatus.Cancelled.Id;
        }

        public void DeleteComment(int taskCommentId)
        {
            var taskComment = _comments.SingleOrDefault(t => t.Id == taskCommentId);
            if (taskComment == null)
            {
                throw new KeyNotFoundException(nameof(taskCommentId));
            }

            _comments.Remove(taskComment);
        }
    }
}