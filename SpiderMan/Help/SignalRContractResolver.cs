using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace SpiderMan.Help {
    // SignalR CamelCase命名 http://goo.gl/9ViRwJ
    public class SignalRContractResolver : IContractResolver {
        private readonly Assembly _assembly;
        private readonly IContractResolver _camelCaseContractResolver;
        private readonly IContractResolver _defaultContractSerializer;

        public SignalRContractResolver() {
            _defaultContractSerializer = new DefaultContractResolver();
            _camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
            _assembly = typeof(Connection).Assembly;
        }

        public JsonContract ResolveContract(Type type) {
            if (type.Assembly.Equals(_assembly))
                return _defaultContractSerializer.ResolveContract(type);

            return _camelCaseContractResolver.ResolveContract(type);
        }
    }

}
