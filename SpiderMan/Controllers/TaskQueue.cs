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
using MongoDB.Driver.Linq;
using System.Web.Mvc;
using SpiderMan.Entity;

namespace SpiderMan.Controllers {
    public sealed class TaskQueue {
        public static List<SpiderTask> tasks;
        public static List<Agent> agents;
        public static IEnumerable<TaskModel> taskModels;
        public static IEnumerable<Site> sites;
        public static MongoCollection<Site> siteCollection;
        public static MongoCollection<TaskModel> taskModelCollection;
        public static TaskHub masterhub;
        public static readonly TaskQueue Instance;

        // http://www.yoda.arachsys.com/csharp/singleton.html 线程安全的模式
        // 静态构造函数用于初始化任何静态数据，或用于执行仅需执行一次的特定操作。在创建第一个实例或引用任何静态成员之前将调用静态构造函数。
        static TaskQueue() {
            tasks = new List<SpiderTask>();
            agents = new List<Agent>();
            siteCollection = (DependencyResolver.Current.GetService(typeof(IMongoRepo<Site>)) as IMongoRepo<Site>).Collection;
            taskModelCollection = (DependencyResolver.Current.GetService(typeof(IMongoRepo<TaskModel>)) as IMongoRepo<TaskModel>).Collection;

            //注意必须使用ToList避免懒惰加载，否则在每次调用taskModels对象时会再次查询，从而清空已被赋值过的Timer属性。
            //这里应该是mongo driver或mongo Respository的特殊模式。
            taskModels = taskModelCollection.AsQueryable<TaskModel>().Where(d => d.Act == (int)eAct.Normal && d.Interval > 0).ToList();
            sites = siteCollection.Find(Query<Site>.EQ(d => d.Act, (int)eAct.Normal)).ToList();
            Instance = new TaskQueue();
            Instance.ModelTimerBuild();
            Instance.Maintenance();
        }

        TaskQueue() { }

        public SpiderTask GenerateTask(TaskModel model) {
            var newTask = new SpiderTask {
                Id = Guid.NewGuid(),
                BirthTime = DateTime.Now,
                TaskModelId = model.Id,
                Site = model.Site,
                Source = model.SourceCode,
                CommandType = ((eCommandType)model.CommandType).ToString(),
                Url = model.Url,
                ArticleType = ((eArticleType)model.ArticleType).ToString()
            };
            tasks.Add(newTask);
            if (masterhub != null) {
                masterhub.BroadcastRanderTask();
            }
            return newTask;
        }

        private void ModelTimerBuild() {
            foreach (var model in taskModels) {
                GenerateTask(model);
                model.Timer = new Timer(1000 * model.Interval);
                model.Timer.Elapsed += delegate { GenerateTask(model); };
                model.Timer.Enabled = true;
            }
        }

        public void ModelTimerReBuild() {
            //复写taskModels并不会中止其Timer，必须手动中止
            foreach (var model in TaskQueue.taskModels)
                if (model.Timer != null) model.Timer.Close();
            taskModels = taskModelCollection.AsQueryable<TaskModel>().Where(d => d.Act == (int)eAct.Normal && d.Interval > 0).ToList();
            ModelTimerBuild();
        }

        public void SiteTimerReBuild() {
            sites = siteCollection.Find(Query<Site>.EQ(d => d.Act, (int)eAct.Normal)).ToList();
            if (masterhub != null) {
                foreach (var agent in agents) {
                    if (agent.Timers != null)
                        foreach (var timer in agent.Timers) timer.Close();
                    masterhub.AgentTimerBuild(agent);
                }
            }
        }

        private void Maintenance() {
            Timer _1min = new Timer(1000 * 60);
            _1min.Elapsed += delegate { ClearDoneTask(); };
            _1min.Enabled = true;

            Timer _5min = new Timer(1000 * 300);
            _5min.Elapsed += delegate { ClearExecutingTask(); };
            _5min.Enabled = true;
        }

        private void ClearDoneTask() {
            tasks.RemoveAll(x => x.Status == eTaskStatus.Done && (DateTime.Now - x.HandlerTime).TotalMinutes > 5);
            if (masterhub != null) masterhub.BroadcastRanderTask();
        }

        private void ClearExecutingTask() {
            var executerTask = tasks.Where(x => x.Status == eTaskStatus.Executing && (DateTime.Now - x.BirthTime).TotalMinutes > 15);
            if (executerTask.Count() > 0) {
                var str = new StringBuilder();
                foreach (var task in executerTask) str.AppendLine(task.Url);
                ZicLog4Net.ProcessLog(MethodBase.GetCurrentMethod(), "SpiderTask ExecutingOver15min: " + str.ToString(), "Grab", LogType.Warn);

                //tasks = tasks.Except(executerTask).ToList(); //这种写法会产生意外的null成员。原因未知。
                tasks.RemoveAll(x => x.Status == eTaskStatus.Executing && (DateTime.Now - x.BirthTime).TotalMinutes > 15);
                if (masterhub != null) masterhub.BroadcastRanderTask();
            }
        }

    }
}