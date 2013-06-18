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

namespace SpiderMan.Controllers {
    public class TaskHub : Hub {

        private static IList<SpiderTask> tasks = new List<SpiderTask>();
        private static IList<Agent> agents = new List<Agent>();
        MongoRepository<TaskModel> taskModeRepo;
        MongoRepository<Site> siteRepo;
        ArticleRespository articleRepo;

        public TaskHub() {
            taskModeRepo = NinjectWebCommon.Kernel.Get<MongoRepository<TaskModel>>();
            siteRepo = NinjectWebCommon.Kernel.Get<MongoRepository<Site>>();
            articleRepo = NinjectWebCommon.Kernel.Get<ArticleRespository>();

            var models = taskModeRepo.All();
            foreach (var model in models) {
                Timer timer = new Timer(1000 * model.Interval);
                timer.Elapsed += delegate { GenerateTask(model); };
                timer.Enabled = false;
            }
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
            Clients.Group("broad").agentList(agents);
        }

        private void GenerateAgentProcess(Agent agent) {
            var sites = siteRepo.All();
            agent.Timer = new List<Timer>();
            foreach (var site in sites) {
                Timer timer = new Timer(1000 * site.GrabInterval);
                timer.Elapsed += delegate { ProcessTesk(site, agent); };
                timer.Enabled = true;
                agent.Timer.Add(timer);
            }
        }

        private void ProcessTesk(Site site, Agent agent) {
            var task = tasks.Where(d => d.Site == site.Name).FirstOrDefault();
            if (task != null)
                Clients.Client(agent.ConnectionId).castTesk(task);
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

        private JsonSerializerSettings JsonSetting {
            get {
                return new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                };
            }
        }

        public void PostData(string taskjson, string datajson) {
            SpiderTask task = (SpiderTask)JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask), JsonSetting);
            if (task.Status == eTaskStatus.Fail) {
                ZicLog4Net.ProcessLog(MethodBase.GetCurrentMethod(), task.Error, "Grab", LogType.Warn);
                return;
            }
            if (task.Encoding == "gbk")
                datajson = datajson.Replace("u", @"\u");
            switch (task.ArticleType) {
                case eArticleType.Huanle:
                    if (task.CommandType == eCommandType.List) {
                        var data = (IEnumerable<Huanle>)JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Huanle>));
                        articleRepo.HuanleRepo.Add(data);
                    } else {
                        var data = (Huanle)JsonConvert.DeserializeObject(datajson, typeof(Huanle));
                        articleRepo.HuanleRepo.Add(data);
                    }
                    break;
                case eArticleType.Shudong:
                    if (task.CommandType == eCommandType.List) {
                        var data = (IEnumerable<Shudong>)JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Shudong>));
                        articleRepo.ShudongRepo.Add(data);
                    } else {
                        var data = (Shudong)JsonConvert.DeserializeObject(datajson, typeof(Shudong));
                        articleRepo.ShudongRepo.Add(data);
                    }
                    break;
                case eArticleType.Dianbo:
                    if (task.CommandType == eCommandType.List) {
                        var data = (IEnumerable<Dianbo>)JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Dianbo>));
                        articleRepo.DianboRepo.Add(data);
                    } else {
                        var data = (Dianbo)JsonConvert.DeserializeObject(datajson, typeof(Dianbo));
                        articleRepo.DianboRepo.Add(data);
                    }
                    break;
                case eArticleType.Finance:
                    if (task.CommandType == eCommandType.List) {
                        var data = (IEnumerable<Finance>)JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Finance>));
                        articleRepo.FinanceRepo.Add(data);
                    } else {
                        var data = (Finance)JsonConvert.DeserializeObject(datajson, typeof(Finance));
                        articleRepo.FinanceRepo.Add(data);
                    }
                    break;
                case eArticleType.Geek:
                    if (task.CommandType == eCommandType.List) {
                        var data = (IEnumerable<Geek>)JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Geek>));
                        articleRepo.GeekRepo.Add(data);
                    } else {
                        var data = (Geek)JsonConvert.DeserializeObject(datajson, typeof(Geek));
                        articleRepo.GeekRepo.Add(data);
                    }
                    break;
                default:
                    break;
            }
            Clients.Group("broad").broadcastDoneTask(task);
        }

        public void JoinGroup(string groupName) {
            Groups.Add(Context.ConnectionId, groupName);
        }

        public void ManualTask(string modelid) {
            var model = taskModeRepo.GetById(modelid);
            Clients.Client(agents.Where(d => d.Online).Single().ConnectionId).castTesk(GenerateTask(model));
        }

        public override Task OnConnected() {
            return base.OnConnected();
        }

        public override Task OnDisconnected() {
            var agent = agents.SingleOrDefault(d => d.ConnectionId == Context.ConnectionId);
            if (agent != null) {
                agent.Online = false;
                foreach (var timer in agent.Timer) timer.Stop();
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

    public class SpiderTask {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("status")]
        public eTaskStatus Status { get; set; }
        [JsonProperty("articletype")]
        public eArticleType ArticleType { get; set; }
        [JsonProperty("site")]
        public string Site { get; set; }
        [JsonProperty("command")]
        public string Command { get; set; }
        [JsonProperty("commandtype")]
        public eCommandType CommandType { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("handler")]
        public string Handler { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("spend")]
        public int Spend { get; set; }
        [JsonProperty("grabdate")]
        public DateTime GrabDate { get; set; }
        [JsonProperty("encoding")]
        public string Encoding { get; set; }
    }

    public class Agent {
        [JsonProperty("connectionid")]
        public string ConnectionId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("online")]
        public bool Online { get; set; }
        [JsonIgnore]
        public IList<Timer> Timer { get; set; }
    }
}