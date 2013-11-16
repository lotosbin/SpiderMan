using Microsoft.AspNet.SignalR;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using sharp_net;
using sharp_net.Repositories;
using SpiderMan.Entity;
using SpiderMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.Mvc;

namespace SpiderMan.Controllers {

    public class TaskHub : Hub {

        //TaskHub在每次有客户端与服务端建立链接时都会新建一个实例。所以Timer的存在导致第一个TaskHub实例永远不被销毁，直至app中止。
        public TaskHub() {
            if (TaskQueue.masterhub == null)
                TaskQueue.masterhub = this;
        }

        public void RegisterBoard() {
            Groups.Add(Context.ConnectionId, "broad");
            Clients.Client(Context.ConnectionId).agentList(TaskQueue.agents);
            BroadcastRanderTask();
        }

        public void RegisterAgent(string name) {
            Groups.Add(Context.ConnectionId, "agent");
            var agent = TaskQueue.agents.SingleOrDefault(d => d.Name == name);
            if (agent != null) {
                agent.ConnectionId = Context.ConnectionId;
                agent.Online = true;
                if (agent.Timers != null)
                    foreach (var timer in agent.Timers) timer.Start();
                Clients.Group("broad").agentList(TaskQueue.agents);
            } else {
                var newagent = new Agent {
                    ConnectionId = Context.ConnectionId,
                    Name = name,
                    Online = true
                };
                TaskQueue.agents.Add(newagent);
                Clients.Group("broad").agentList(TaskQueue.agents);
                AgentTimerBuild(newagent);
            }
        }

        public void AgentTimerBuild(Agent agent) {
            agent.Timers = new List<Timer>();
            foreach (var site in TaskQueue.sites) {
                System.Threading.Thread.Sleep(2000);
                ProcessTesk(site, agent);
                Timer timer = new Timer(1000 * site.GrabInterval);
                timer.Elapsed += delegate { ProcessTesk(site, agent); };
                timer.Enabled = true;
                agent.Timers.Add(timer);
            }
        }

        private void ProcessTesk(Site site, Agent agent) {
            var task = TaskQueue.tasks.Where(d => d.Status == eTaskStatus.Standby && d.Site == site.Name).FirstOrDefault();
            if (task != null) {
                task.Status = eTaskStatus.Executing;
                task.HandlerAgent = agent.Name;
                task.HandlerTime = DateTime.Now;
                Clients.Client(agent.ConnectionId).castTesk(task);
                BroadcastRanderTask();
            } else {
                var executingTask = TaskQueue.tasks.Where(d => d.Status == eTaskStatus.Executing && d.Site == site.Name).OrderBy(d => d.HandlerTime).FirstOrDefault();
                if (executingTask != null && (DateTime.Now - executingTask.HandlerTime).TotalMinutes > 2) {
                    executingTask.HandlerAgent = agent.Name;
                    executingTask.HandlerTime = DateTime.Now;
                    Clients.Client(agent.ConnectionId).castTesk(executingTask);
                    BroadcastRanderTask();
                }
            }
        }

        public void DoneTask(SpiderTask task) {
            if (task.Status == eTaskStatus.Fail)
                ZicLog4Net.ProcessLog(MethodBase.GetCurrentMethod(), "SpiderTask Fail: " + task.Error + " Url:" + task.Url, "Grab", LogType.Warn);
            int index = TaskQueue.tasks.FindIndex(d => d.Id == task.Id);
            TaskQueue.tasks[index] = task;
            //完整替换List成员必须使用index赋值方法。不能直接赋值引用对象成员。
            BroadcastRanderTask();
        }

        public void DeleteTask(string taskId) {
            var task = TaskQueue.tasks.SingleOrDefault(d => d.Id == Guid.Parse(taskId));
            TaskQueue.tasks.Remove(task);
            BroadcastRanderTask();
        }

        public void DeleteAllTask() {
            TaskQueue.tasks.RemoveAll(d => true);
            BroadcastRanderTask();
        }

        //注意：终止agent进程并不能马上触发OnDisconnected。所以需要在ProcessTesk中检查超时Executing任务
        public override Task OnDisconnected() {
            var agent = TaskQueue.agents.SingleOrDefault(d => d.ConnectionId == Context.ConnectionId);
            if (agent != null) {
                agent.Online = false;
                if (agent.Timers != null)
                    foreach (var timer in agent.Timers) timer.Stop();
                Clients.Group("broad").agentList(TaskQueue.agents);
            }
            return base.OnDisconnected();
        }

        public override Task OnReconnected() {
            var agent = TaskQueue.agents.SingleOrDefault(d => d.ConnectionId == Context.ConnectionId);
            if (agent != null) {
                ZicLog4Net.ProcessLog(MethodBase.GetCurrentMethod(), "OnReconnected be used!", "Grab", LogType.Debug);
                agent.Online = true;
                if (agent.Timers != null)
                    foreach (var timer in agent.Timers) timer.Start();
                Clients.Group("broad").agentList(TaskQueue.agents);
            }
            return base.OnReconnected();
        }

        public void ManualModel(string modelid) {
            var model = TaskQueue.taskModels.SingleOrDefault(d => d.Id == modelid);
            if (model != null) {
                TaskQueue.Instance.GenerateTask(model);
                BroadcastRanderTask();
            }
        }

        public void StopModel(string modelid) {
            var model = TaskQueue.taskModels.SingleOrDefault(d => d.Id == modelid);
            if (model != null) model.Timer.Stop();
        }

        public void StartModel(string modelid) {
            var model = TaskQueue.taskModels.SingleOrDefault(d => d.Id == modelid);
            if (model != null) model.Timer.Start();
        }

        public void BroadcastRanderTask() {
            try {
                //在TaskModel生产task时会于这里的Json.net序列化同时进行，从而造成异常。
                Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
            } catch (Exception ex) {
                ZicLog4Net.ProcessLog(MethodBase.GetCurrentMethod(), "BroadcastRanderTask: " + ex.Message, "Grab", LogType.Warn);
            }
        }

    }
}