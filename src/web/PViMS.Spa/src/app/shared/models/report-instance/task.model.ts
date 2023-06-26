import { TaskCommentModel } from "./task-comment.model";

export interface TaskModel {
  id: number;
  source: string;
  description: string;
  taskType: string;
  taskStatus: string;
  taskAge: string;
  createdDetail: string;
  updatedDetail: string;
  comments: TaskCommentModel[];
}