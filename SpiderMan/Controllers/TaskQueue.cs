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
        public static IEnumerable<TaskModel> taskModels;
        public static IEnumerable<Site> sites;
        public static Respositorys repos;
        public static readonly TaskQueue Instance;

        // http://www.yoda.arachsys.com/csharp/singleton.html 线程安全的模式
        // 静态构造函数用于初始化任何静态数据，或用于执行仅需执行一次的特定操作。在创建第一个实例或引用任何静态成员之前将调用静态构造函数。
        static TaskQueue() {
            tasks = new List<SpiderTask>();
            agents = new List<Agent>();
            repos = new Respositorys();
            
            //注意必须使用ToList避免懒惰加载，否则在每次调用taskModels对象时会再次查询，从而清空已被赋值过的Timer属性。
            //这里应该是mongo driver或mongo Respository的特殊模式。
            taskModels = repos.TaskModelRepo.Collection.Find(Query<TaskModel>.EQ(d => d.Act, (int)eAct.Normal)).ToList();
            sites = repos.SiteRepo.Collection.Find(Query<Site>.EQ(d => d.Act, (int)eAct.Normal)).ToList();
            Instance = new TaskQueue();
        }

        TaskQueue() { }

    }
}