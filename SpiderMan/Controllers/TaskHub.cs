using Microsoft.AspNet.SignalR;
using MongoRepository;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using sharp_net;
using SpiderMan.App_Start;
using SpiderMan.Models;
using SpiderMan.Respository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using Ninject;
using System.Text;
using MongoDB.Driver.Builders;
using sharp_net.Repositories;
using Newtonsoft.Json.Linq;
using SpiderMan.Help;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SpiderMan.Controllers {

    public class TaskHub : Hub {
        private Respositorys repos;
        public TaskHub() {
            repos = NinjectWebCommon.Kernel.Get<Respositorys>();
        }

        private SpiderTask GenerateTask(TaskModel model) {
            var newTask = TaskQueue.Instance.GenerateTask(model);
            Clients.Group("broad").broadcastAddTask(newTask);
            return newTask;
        }

        public void JoinGroup(string groupName) {
            Groups.Add(Context.ConnectionId, groupName);
        }

        public void RegisterBoard() {
            JoinGroup("broad");
            Clients.Client(Context.ConnectionId).agentList(TaskQueue.agents);
        }

        public void RegisterAgent(string name) {
            var offlineAgent = TaskQueue.agents.SingleOrDefault(d => d.Name == name);
            if (offlineAgent != null) {
                offlineAgent.ConnectionId = Context.ConnectionId;
                offlineAgent.Online = true;
                foreach (var timer in offlineAgent.Timer) timer.Start();
            } else {
                var newagent = new Agent {
                    ConnectionId = Context.ConnectionId,
                    Name = name,
                    Online = true
                };
                TaskQueue.agents.Add(newagent);
                TaskQueue.Instance.GenerateAgentProcess(newagent);
            }
            JoinGroup("agent");
            Clients.Group("broad").agentList(TaskQueue.agents);
        }

        private void ProcessTesk(Site site, Agent agent) {
            var task = TaskQueue.tasks.Where(d => d.Status == eTaskStatus.Standby && d.Site == site.Name).FirstOrDefault();
            if (task != null) {
                task.Status = eTaskStatus.Executing;
                Clients.Client(agent.ConnectionId).castTesk(task);
            }
        }

        public void DoneTask(string taskjson) {
            SpiderTask task = (SpiderTask)JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask));
            TaskQueue.tasks.Remove(TaskQueue.tasks.SingleOrDefault(x => x.Id == task.Id));
            Clients.Group("broad").broadcastRemoveTask(task);
            if (task.Status == eTaskStatus.Fail) {
                ZicLog4Net.ProcessLog(MethodBase.GetCurrentMethod(), task.Error, "Grab", LogType.Warn);
                return;
            }
        }

        public void ManualTask(string modelid) {
            var model = repos.TaskModelRepo.GetById(modelid);
            if (model != null) {
                SpiderTask task = GenerateTask(model);
                Clients.Client(TaskQueue.agents.Where(d => d.Online).Single().ConnectionId).castTesk(task);
            }
        }

        public override Task OnDisconnected() {
            var agent = TaskQueue.agents.SingleOrDefault(d => d.ConnectionId == Context.ConnectionId);
            if (agent != null) {
                agent.Online = false;
                foreach (var timer in agent.Timer) {
                    timer.Stop();
                    timer.Close();
                }
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
    }

}