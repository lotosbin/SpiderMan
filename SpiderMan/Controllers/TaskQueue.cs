using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using sharp_net;
using SpiderMan.App_Start;
using SpiderMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using Ninject;
using System.Text;
using sharp_net.Repositories;
using sharp_net.Mongo;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System.Web.Mvc;

namespace SpiderMan.Controllers {
    public sealed class TaskQueue {
        public static IList<SpiderTask> tasks;
        public static IList<Agent> agents;
        public static IEnumerable<TaskModel> taskModels;
        public static IEnumerable<Site> sites;
        public static MongoCollection<Site> siteCollection;
        public static MongoCollection<TaskModel> taskModelCollection;
        public static TaskHub firsthub;
        public static readonly TaskQueue Instance;

        // http://www.yoda.arachsys.com/csharp/singleton.html 线程安全的模式
        // 静态构造函数用于初始化任何静态数据，或用于执行仅需执行一次的特定操作。在创建第一个实例或引用任何静态成员之前将调用静态构造函数。
        static TaskQueue() {
            tasks = new List<SpiderTask>();
            agents = new List<Agent>();
            var siteRepo = DependencyResolver.Current.GetService(typeof(IMongoRepo<Site>)) as IMongoRepo<Site>;
            siteCollection = siteRepo.Collection;
            var taskModelRepo = DependencyResolver.Current.GetService(typeof(IMongoRepo<TaskModel>)) as IMongoRepo<TaskModel>;
            taskModelCollection = taskModelRepo.Collection;

            //注意必须使用ToList避免懒惰加载，否则在每次调用taskModels对象时会再次查询，从而清空已被赋值过的Timer属性。
            //这里应该是mongo driver或mongo Respository的特殊模式。
            taskModels = taskModelCollection.Find(Query<TaskModel>.EQ(d => d.Act, (int)eAct.Normal)).ToList();
            sites = siteCollection.Find(Query<Site>.EQ(d => d.Act, (int)eAct.Normal)).ToList();
            Instance = new TaskQueue();
        }

        TaskQueue() { }

        public SpiderTask GenerateTask(TaskModel model) {
            var newTask = new SpiderTask {
                Id = Guid.NewGuid(),
                Site = model.Site,
                Command = model.Command,
                CommandType = (eCommandType)model.CommandType,
                Url = model.Url,
                ArticleType = (eArticleType)model.ArticleType
            };
            tasks.Add(newTask);
            firsthub.Clients.Group("broad").broadcastAddTask(newTask);
            return newTask;
        }

        public void ModelTimerBuild() {
            foreach (var model in taskModels) {
                model.Timer = new Timer(1000 * model.Interval);
                model.Timer.Elapsed += delegate { GenerateTask(model); };
                model.Timer.Enabled = true;
            }
        }

        public void ModelTimerReBuild() {
            if (TaskQueue.firsthub == null) return;
            //复写taskModels并不会中止其Timer，必须手动中止
            foreach (var model in TaskQueue.taskModels) model.Timer.Close();
            taskModels = taskModelCollection.Find(Query<TaskModel>.EQ(d => d.Act, (int)eAct.Normal)).ToList();
            ModelTimerBuild();
        }

    }
}