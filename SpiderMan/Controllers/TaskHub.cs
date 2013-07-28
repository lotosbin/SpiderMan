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
                TaskQueue.Instance.ModelTimerBuild();
            }
        }

        public void RegisterBoard() {
            Groups.Add(Context.ConnectionId, "broad");
            Clients.Client(Context.ConnectionId).agentList(TaskQueue.agents);
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
                AgentsTimerBuild(newagent);
            }
            Groups.Add(Context.ConnectionId, "agent");
            Clients.Group("broad").agentList(TaskQueue.agents);
        }

        private void AgentsTimerBuild(Agent agent) {
            //复写agent.Timer并不会中止其Timer，必须手动中止
            foreach (var model in agent.Timer) model.Close();
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
                task.Status = eTaskStatus.Executing;
                Clients.Group("broad").broadcastExeTask(task);
                Clients.Client(agent.ConnectionId).castTesk(task);
            }
        }

        public void DoneTask(string taskjson) {
            SpiderTask task = (SpiderTask)JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask));
            TaskQueue.tasks.Remove(TaskQueue.tasks.SingleOrDefault(x => x.Id == task.Id));
            Clients.Group("broad").broadcastDoneTask(task);
            if (task.Status == eTaskStatus.Fail)
                ZicLog4Net.ProcessLog(MethodBase.GetCurrentMethod(), task.Error, "Grab", LogType.Warn);
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
                Clients.Client(TaskQueue.agents.Where(d => d.Online).Single().ConnectionId).castTesk(task);
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