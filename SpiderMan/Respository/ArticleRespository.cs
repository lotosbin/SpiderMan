using MongoRepository;
using sharp_net;
using SpiderMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Respository {
    public class ArticleRespository {
        private MongoRepository<Huanle> huanleRepo;
        private MongoRepository<Dianbo> dianboRepo;
        private MongoRepository<Finance> financeRepo;
        private MongoRepository<Geek> geekRepo;
        private MongoRepository<Shudong> shudongeRepo;

        public MongoRepository<Huanle> HuanleRepo {
            get {
                if (huanleRepo == null) huanleRepo = new MongoRepository<Huanle>();
                return huanleRepo;
            }
        }
        public MongoRepository<Dianbo> DianboRepo {
            get {
                if (dianboRepo == null) dianboRepo = new MongoRepository<Dianbo>();
                return dianboRepo;
            }
        }
        public MongoRepository<Finance> FinanceRepo {
            get {
                if (financeRepo == null) financeRepo = new MongoRepository<Finance>();
                return financeRepo;
            }
        }
        public MongoRepository<Geek> GeekRepo {
            get {
                if (geekRepo == null) geekRepo = new MongoRepository<Geek>();
                return geekRepo;
            }
        }
        public MongoRepository<Shudong> ShudongRepo {
            get {
                if (shudongeRepo == null) shudongeRepo = new MongoRepository<Shudong>();
                return shudongeRepo;
            }
        }
    }
}