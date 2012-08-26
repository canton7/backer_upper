using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.TaskScheduler;
using System.IO;

namespace BackerUpper
{
    class Scheduler
    {
        public bool Enabled { get; private set; }
        public DateTime ScheduleTime { get; private set; }
        public DaysOfTheWeek DaysOfTheWeek { get; private set; }

        public Scheduler(bool enabled, DateTime scheduleTime, DaysOfTheWeek daysOfTheWeek) {
            this.Enabled = enabled;
            this.ScheduleTime = scheduleTime;
            this.DaysOfTheWeek = daysOfTheWeek;
        }

        private Scheduler(string name) {
            this.loadScheduler(name);
        }

        public static Scheduler Load(string name) {
            Scheduler task = new Scheduler(name);
            return task;
        }

        private static TaskFolder loadTaskFolder() {
            TaskService ts = new TaskService();
            TaskFolder folder;
            try {
                folder = ts.GetFolder(@"\"+Constants.TASK_SCHEDULER_FOLDER);
            }
            catch (FileNotFoundException) {
                folder = ts.RootFolder.CreateFolder(@"\"+Constants.TASK_SCHEDULER_FOLDER);
            }
            return folder;
        }

        private static Task loadTask(string name, TaskFolder folder = null) {
            if (folder == null)
                folder = loadTaskFolder();
            TaskCollection tasks = folder.GetTasks();
            return tasks.FirstOrDefault(x => x.Name == name);
        }

        private void loadScheduler(string name) {
            Task task = loadTask(name);

            if (task == null) {
                this.ScheduleTime = new DateTime(1970, 1, 1, 20, 0, 0);
                this.DaysOfTheWeek = DaysOfTheWeek.Monday | DaysOfTheWeek.Tuesday | DaysOfTheWeek.Wednesday | DaysOfTheWeek.Thursday |
                    DaysOfTheWeek.Friday | DaysOfTheWeek.Saturday | DaysOfTheWeek.Sunday;
                this.Enabled = true;
                return;
            }

            Trigger trigger = task.Definition.Triggers.FirstOrDefault(x => x.TriggerType == TaskTriggerType.Weekly);
            // Don't have an appropriate trigger -- select no days
            DaysOfTheWeek dow;
            if (trigger == null) {
                dow = 0;
                this.ScheduleTime = new DateTime(1970, 1, 1, 20, 0, 0);
            }
            else {
                dow = ((WeeklyTrigger)trigger).DaysOfWeek;
                this.ScheduleTime = new DateTime(1970, 1, 1, trigger.StartBoundary.Hour, trigger.StartBoundary.Minute, 0);
            }

            this.DaysOfTheWeek = dow;
            this.Enabled = task.Definition.Settings.Enabled;
        }

        public static void Delete(string name) {
            TaskFolder folder = loadTaskFolder();
            try {
                folder.DeleteTask(name);
            }
            catch (FileNotFoundException) { }
        }

        public void Save(string name) {
            TaskFolder folder = loadTaskFolder();
            Task task = loadTask(name, folder);
            TaskDefinition definition;

            if (task == null) {
                definition = new TaskService().NewTask();
                string process = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                definition.Actions.Add(new ExecAction(process, " --backup=\""+name+"\""));
                definition.Settings.DisallowStartIfOnBatteries = false;
                definition.Settings.StartWhenAvailable = true;
            }
            else {
                definition = task.Definition;
                definition.Triggers.Clear();
            }

            WeeklyTrigger trigger = new WeeklyTrigger();
            DateTime start = DateTime.Today + new TimeSpan(this.ScheduleTime.Hour, this.ScheduleTime.Minute, 0);
            // If we're starting in the past, move to tomorrow
            if (start < DateTime.Now)
                start += new TimeSpan(1, 0, 0, 0);
            trigger.StartBoundary = start;

            trigger.DaysOfWeek = this.DaysOfTheWeek;

            if (trigger.DaysOfWeek != 0)
                definition.Triggers.Add(trigger);

            definition.Settings.Enabled = this.Enabled;

            folder.RegisterTaskDefinition(name, definition);
        }
    }
}
