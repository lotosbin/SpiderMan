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
    public sealed class TaskQueue {
        public static IList<SpiderTask> tasks;
        public static IList<Agent> agents;
        private static Respositorys repos;

        static readonly TaskQueue instance = new TaskQueue();

        // http://www.yoda.arachsys.com/csharp/singleton.html 线程安全的模式
        // 静态构造函数用于初始化任何静态数据，或用于执行仅需执行一次的特定操作。在创建第一个实例或引用任何静态成员之前将调用静态构造函数。
        static TaskQueue() {
            tasks = new List<SpiderTask>();
            agents = new List<Agent>();
            repos = new Respositorys();
            var models = repos.TaskModelRepo.Collection.Find(Query<TaskModel>.EQ(d => d.Act, (int)eAct.Normal));
            foreach (var model in models) {
                Timer timer = new Timer(1000 * model.Interval);
                //timer.Elapsed += delegate { GenerateTask(model); };
                timer.Enabled = true;
            }
        }

        TaskQueue() { }

        public static TaskQueue Instance {
            get {
                return instance;
            }
        }

        public SpiderTask GenerateTask(TaskModel model) {
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
            return newTask;
        }

        public void GenerateAgentProcess(Agent agent) {
            var sites = repos.SiteRepo.Collection.Find(Query<TaskModel>.EQ(d => d.Act, (int)eAct.Normal));
            agent.Timer = new List<Timer>();
            foreach (var site in sites) {
                Timer timer = new Timer(1000 * site.GrabInterval);
                //timer.Elapsed += delegate { ProcessTesk(site, agent); };
                timer.Enabled = true;
                agent.Timer.Add(timer);
            }
        }

    }
}