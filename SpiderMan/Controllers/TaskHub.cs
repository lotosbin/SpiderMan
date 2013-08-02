using Microsoft.AspNet.SignalR;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using sharp_net;
using sharp_net.Repositories;
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
            if (TaskQueue.firsthub == null) {
                TaskQueue.firsthub = this;
            }
        }

        public void RegisterBoard() {
            Groups.Add(Context.ConnectionId, "broad");
            Clients.Client(Context.ConnectionId).agentList(TaskQueue.agents);
            Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        public void RegisterAgent(string name) {
            var agent = TaskQueue.agents.SingleOrDefault(d => d.Name == name);
            if (agent != null) {
                agent.ConnectionId = Context.ConnectionId;
                agent.Online = true;
                foreach (var timer in agent.Timer) timer.Start();
            } else {
                var newagent = new Agent {
                    ConnectionId = Context.ConnectionId,
                    Name = name,
                    Online = true
                };
                TaskQueue.agents.Add(newagent);
                AgentTimerBuild(newagent);
            }
            Groups.Add(Context.ConnectionId, "agent");
            Clients.Group("broad").agentList(TaskQueue.agents);
        }

        public void AgentTimerBuild(Agent agent) {
            agent.Timer = new List<Timer>();
            foreach (var site in TaskQueue.sites) {
                System.Threading.Thread.Sleep(2000);
                ProcessTesk(site, agent);
                Timer timer = new Timer(1000 * site.GrabInterval);
                timer.Elapsed += delegate { ProcessTesk(site, agent); };
                timer.Enabled = true;
                agent.Timer.Add(timer);
            }
        }

        private void ProcessTesk(Site site, Agent agent) {
            var task = TaskQueue.tasks.Where(d => d.Status == eTaskStatus.Standby && d.Site == site.Name).FirstOrDefault();
            if (task != null) {
                Clients.Client(agent.ConnectionId).castTesk(task);
                task.Status = eTaskStatus.Executing;
                task.HandlerAgent = agent.Name;
                task.HandlerTime = DateTime.Now;
                Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
            } else {
                var executingTask = TaskQueue.tasks.Where(d => d.Status == eTaskStatus.Executing && d.Site == site.Name).OrderBy(d => d.HandlerTime).FirstOrDefault();
                if (executingTask != null) {
                    if ((DateTime.Now - executingTask.HandlerTime).TotalMinutes > 2) {
                        Clients.Client(agent.ConnectionId).castTesk(executingTask);
                        executingTask.HandlerAgent = agent.Name;
                        executingTask.HandlerTime = DateTime.Now;
                        Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
                    }
                }
            }
        }

        public void DoneTask(string taskjson) {
            SpiderTask task = (SpiderTask)JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask));
            if (task.Status == eTaskStatus.Fail)
                ZicLog4Net.ProcessLog(MethodBase.GetCurrentMethod(), task.Error, "Grab", LogType.Warn);
            //TaskQueue.tasks.Remove(TaskQueue.tasks.SingleOrDefault(x => x.Id == task.Id));
            task = TaskQueue.tasks.SingleOrDefault(d => d.Id == task.Id);
            task.Status = eTaskStatus.Done;
            Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        public override Task OnDisconnected() {
            var agent = TaskQueue.agents.SingleOrDefault(d => d.ConnectionId == Context.ConnectionId);
            if (agent != null) {
                agent.Online = false;
                foreach (var timer in agent.Timer) timer.Stop();
                Clients.Group("broad").agentList(TaskQueue.agents);
            }
            return base.OnDisconnected();
        }

        public override Task OnReconnected() {
            var agent = TaskQueue.agents.SingleOrDefault(d => d.ConnectionId == Context.ConnectionId);
            if (agent != null) {
                agent.Online = true;
                foreach (var timer in agent.Timer) timer.Start();
                Clients.Group("broad").agentList(TaskQueue.agents);
            }
            return base.OnReconnected();
        }

        public void ManualModel(string modelid) {
            var model = TaskQueue.taskModels.SingleOrDefault(d => d.Id == modelid);
            if (model != null) {
                var task = TaskQueue.Instance.GenerateTask(model);
                var agent = TaskQueue.agents.Where(d => d.Online).Single();
                Clients.Client(agent.ConnectionId).castTesk(task);
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


    }
}