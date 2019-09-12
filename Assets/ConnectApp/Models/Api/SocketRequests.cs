using System;
using System.Collections.Generic;

namespace ConnectApp.Models.Api {
        [Serializable]
        public class SocketRequestPayload {
            //opCode
            public int op;
            //dataBody
            public SocketRequestData d;
        }

        [Serializable]
        public class SocketRequestData {
        }

        [Serializable]
        public class SocketIdentifyRequest : SocketRequestData {
            public string ls;
            public string commitId;
            public Dictionary<string, string> properties;
        }

        [Serializable]
        public class SocketResumeRequest : SocketRequestData {
            public string sessionId;
            public int seq;
        }
}