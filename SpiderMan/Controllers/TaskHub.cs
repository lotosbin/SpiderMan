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

namespace SpiderMan.Controllers {
    public class TaskHub : Hub {

        private static IList<SpiderTask> tasks = new List<SpiderTask>();
        private static IList<Agent> agents = new List<Agent>();
        Respositorys repos;

        public TaskHub() {
            repos = NinjectWebCommon.Kernel.Get<Respositorys>();

            var models = repos.TaskModelRepo.Collection.Find(Query<TaskModel>.EQ(d => d.Act, (int)eAct.Normal));
            foreach (var model in models) {
                Timer timer = new Timer(1000 * model.Interval);
                //timer.Elapsed += delegate { GenerateTask(model); };
                timer.Enabled = true;
            }
        }

        private SpiderTask GenerateTask(TaskModel model) {
            var newTask = new SpiderTask {
                Id = Guid.NewGuid(),
                Site = model.Site,
                Command = model.Command,
                CommandType = (eCommandType)model.CommandType,
                Encoding = model.Encoding,
                Url = model.Url,
                ArticleType = (eArticleType)model.ArticleType
            };
            tasks.Add(newTask);
            Clients.Group("broad").broadcastAddTask(newTask);
            return newTask;
        }

        public void JoinGroup(string groupName) {
            Groups.Add(Context.ConnectionId, groupName);
        }

        public void RegisterBoard() {
            JoinGroup("broad");
            Clients.Client(Context.ConnectionId).agentList(agents);
        }

        public void RegisterAgent(string name) {
            var offlineAgent = agents.SingleOrDefault(d => d.Name == name);
            if (offlineAgent != null) {
                offlineAgent.ConnectionId = Context.ConnectionId;
                offlineAgent.Online = true;
                foreach (var timer in offlineAgent.Timer) timer.Start();
            } else {
                var agent = new Agent {
                    ConnectionId = Context.ConnectionId,
                    Name = name,
                    Online = true
                };
                agents.Add(agent);
                GenerateAgentProcess(agent);
            }
            JoinGroup("agent");
            Clients.Group("broad").agentList(agents);
        }

        private void GenerateAgentProcess(Agent agent) {
            var sites = repos.SiteRepo.Collection.Find(Query<TaskModel>.EQ(d => d.Act, (int)eAct.Normal));
            agent.Timer = new List<Timer>();
            foreach (var site in sites) {
                Timer timer = new Timer(1000 * site.GrabInterval);
                //timer.Elapsed += delegate { ProcessTesk(site, agent); };
                timer.Enabled = true;
                agent.Timer.Add(timer);
            }
        }

        private void ProcessTesk(Site site, Agent agent) {
            var task = tasks.Where(d => d.Status == eTaskStatus.Standby && d.Site == site.Name).FirstOrDefault();
            if (task != null) {
                task.Status = eTaskStatus.Executing;
                Clients.Client(agent.ConnectionId).castTesk(task);
            }
        }

        private JsonSerializerSettings JsonSetting {
            get {
                return new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore,
                    
                };
            }
        }

        public void PostData(string taskjson, string datajson) {
            SpiderTask task = (SpiderTask)JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask), JsonSetting);
            //tasks = tasks.Where(d => d.Id != task.Id).ToList();
            tasks.Remove(task);
            if (task.Status == eTaskStatus.Fail) {
                ZicLog4Net.ProcessLog(MethodBase.GetCurrentMethod(), task.Error, "Grab", LogType.Warn);
                return;
            }
            //if (task.Encoding == "gbk") {
            //    datajson = datajson.Replace("u", @"\u");
            //    //byte[] unicodeBytes = Encoding.Unicode.GetBytes(datajson);
            //    //byte[] asciiBytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, unicodeBytes);
            //    //char[] asciiChars = new char[Encoding.ASCII.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            //    //Encoding.ASCII.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            //    //datajson = new string(asciiChars);
            //}
            switch (task.ArticleType) {
                case eArticleType.Huanle:
                    if (task.CommandType == eCommandType.List) {
                        var data = (IEnumerable<Huanle>)JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Huanle>), JsonSetting);
                        repos.HuanleRepo.Add(data);
                    } else {
                        var data = (Huanle)JsonConvert.DeserializeObject(datajson, typeof(Huanle));
                        repos.HuanleRepo.Add(data);
                    }
                    break;
                case eArticleType.Shudong:
                    if (task.CommandType == eCommandType.List) {
                        var data = (IEnumerable<Shudong>)JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Shudong>));
                        repos.ShudongRepo.Add(data);
                    } else {
                        var data = (Shudong)JsonConvert.DeserializeObject(datajson, typeof(Shudong));
                        repos.ShudongRepo.Add(data);
                    }
                    break;
                case eArticleType.Dianbo:
                    if (task.CommandType == eCommandType.List) {
                        var data = (IEnumerable<Dianbo>)JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Dianbo>));
                        repos.DianboRepo.Add(data);
                    } else {
                        var data = (Dianbo)JsonConvert.DeserializeObject(datajson, typeof(Dianbo));
                        repos.DianboRepo.Add(data);
                    }
                    break;
                case eArticleType.Finance:
                    if (task.CommandType == eCommandType.List) {
                        var data = (IEnumerable<Finance>)JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Finance>));
                        repos.FinanceRepo.Add(data);
                    } else {
                        var data = (Finance)JsonConvert.DeserializeObject(datajson, typeof(Finance));
                        repos.FinanceRepo.Add(data);
                    }
                    break;
                case eArticleType.Geek:
                    if (task.CommandType == eCommandType.List) {
                        var data = (IEnumerable<Geek>)JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Geek>));
                        repos.GeekRepo.Add(data);
                    } else {
                        var data = (Geek)JsonConvert.DeserializeObject(datajson, typeof(Geek));
                        repos.GeekRepo.Add(data);
                    }
                    break;
                default:
                    break;
            }
            Clients.Group("broad").broadcastDoneTask(task);
        }

        public void ManualTask(string modelid) {
            var model = repos.TaskModelRepo.GetById(modelid);
            SpiderTask task = GenerateTask(model);
            Clients.Client(agents.Where(d => d.Online).Single().ConnectionId).castTesk(task);
        }

        //public override Task OnConnected() {
        //    return base.OnConnected();
        //}

        public override Task OnDisconnected() {
            var agent = agents.SingleOrDefault(d => d.ConnectionId == Context.ConnectionId);
            if (agent != null) {
                agent.Online = false;
                foreach (var timer in agent.Timer) {
                    timer.Stop();
                    timer.Close();
                }
                Clients.Group("broad").agentList(agents);
            }
            return base.OnDisconnected();
        }

        public override Task OnReconnected() {
            var agent = agents.SingleOrDefault(d => d.ConnectionId == Context.ConnectionId);
            if (agent != null) {
                agent.Online = true;
                foreach (var timer in agent.Timer) timer.Start();
                Clients.Group("broad").agentList(agents);
            }
            return base.OnReconnected();
        }
    }

}