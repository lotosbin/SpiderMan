using Microsoft.AspNet.SignalR;
using MongoRepository;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using sharp_net;
using SpiderMan.Models;
using SpiderMan.Respository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Web;

namespace SpiderMan.Controllers {
    public class TaskHub : Hub {

        private static IList<SpiderTask> tasks = new List<SpiderTask>();
        private static IList<Agent> agents = new List<Agent>();
        MongoRepository<TaskModel> taskModeRepo;
        MongoRepository<Site> siteRepo;
        ArticleRespository articleRepo;

        public TaskHub(MongoRepository<TaskModel> _taskModeRepo, MongoRepository<Site> _siteRepo, ArticleRespository _articleRepo) {
            taskModeRepo = _taskModeRepo;
            siteRepo = _siteRepo;
            articleRepo = _articleRepo;

            var models = taskModeRepo.All();
            foreach (var model in models) {
                Timer timer = new Timer(1000 * model.Interval);
                timer.Elapsed += delegate { GenerateTask(model); };
                timer.AutoReset = true;
                timer.Enabled = true;
            }

            var sites = siteRepo.All();
            foreach (var site in sites) {
                foreach (var agent in agents) {
                    Timer timer = new Timer(1000 * site.GrabInterval);
                    timer.Elapsed += delegate { ProcessTesk(site, agent); };
                    timer.AutoReset = true;
                    timer.Enabled = true;
                }
            }
        }

        public void ProcessTesk(Site site, Agent agent) {
            var task = tasks.Where(d => d.Site == site.Name).FirstOrDefault();
            if (task != null)
                Clients.Client(agent.ConnectionId).processTesk(task);
        }

        public void GenerateTask(TaskModel model) {
            var newTask = new SpiderTask {
                Id = Guid.NewGuid(),
                Site = model.Site,
                Command = model.Command,
                Url = model.Url,
                ArticleType = (eArticleType)model.ArticleType
            };
            tasks.Add(newTask);
            Clients.Group("broad").broadcast_AddTask(newTask);
        }

        public void PostData(SpiderTask task, string datajson) {
            if (task.Status == eTaskStatus.Fail) {
                ZicLog4Net.ProcessLog(MethodBase.GetCurrentMethod(), task.Error, "Grab", LogType.Warn);
                return;
            }
            switch (task.ArticleType) {
                case eArticleType.Huanle:
                    var huanle = (Huanle)JsonConvert.DeserializeObject(datajson, typeof(Huanle));
                    articleRepo.HuanleRepo.Add(huanle);
                    break;
                case eArticleType.Shudong:
                    var shudong = (Shudong)JsonConvert.DeserializeObject(datajson, typeof(Shudong));
                    articleRepo.ShudongRepo.Add(shudong);
                    break;
                case eArticleType.Dianbo:
                    var dianbo = (Dianbo)JsonConvert.DeserializeObject(datajson, typeof(Dianbo));
                    articleRepo.DianboRepo.Add(dianbo);
                    break;
                case eArticleType.Finance:
                    var finance = (Finance)JsonConvert.DeserializeObject(datajson, typeof(Finance));
                    articleRepo.FinanceRepo.Add(finance);
                    break;
                case eArticleType.Geek:
                    var geek = (Geek)JsonConvert.DeserializeObject(datajson, typeof(Geek));
                    articleRepo.GeekRepo.Add(geek);
                    break;
                default:
                    break;
            }
        }

        public void registerAgent(string name) {
            if (!agents.Any(d => d.ConnectionId == Context.ConnectionId)) return;
            agents.Add(new Agent {
                ConnectionId = Context.ConnectionId,
                LastActionTimes = new Dictionary<string, DateTime>(),
                Name = name
            });
            Clients.Group("broad").AgentList(agents);
        }

        public void ManualTask(string modelid) {
            var model = taskModeRepo.GetById(modelid);
            GenerateTask(model);
        }



        public override Task OnConnected() {
            return base.OnConnected();
        }

        public override Task OnDisconnected() {
            var agent = agents.SingleOrDefault(d => d.ConnectionId == Context.ConnectionId);
            if (agent != null) {
                agent.Online = false;
                Clients.Group("broad").AgentList(agents);
            }
            return base.OnDisconnected();
        }

        public override Task OnReconnected() {
            var agent = agents.SingleOrDefault(d => d.ConnectionId == Context.ConnectionId);
            if (agent != null) {
                agent.Online = true;
                Clients.Group("broad").AgentList(agents);
            }
            return base.OnReconnected();
        }
    }

    public class SpiderTask {
        public Guid Id { get; set; }
        [JsonProperty("site")]
        public string Site { get; set; }
        [JsonProperty("command")]
        public string Command { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("status")]
        public eTaskStatus Status { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("handler")]
        public string Handler { get; set; }
        [JsonProperty("spend")]
        public string Spend { get; set; }
        [JsonProperty("articletype")]
        public eArticleType ArticleType { get; set; }
    }

    public class Agent {
        [JsonProperty("connectionid")]
        public string ConnectionId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonIgnore]
        public IDictionary<string, DateTime> LastActionTimes { get; set; }
        [JsonProperty("online")]
        public bool Online { get; set; }
    }
}