﻿#define COMMAND_NAME_UPPER

#if DEBUG
#define WHOAMI2
#endif

#if WHOAMI2

using ApolloInterop.Classes;
using ApolloInterop.Interfaces;
using ApolloInterop.Structs.MythicStructs;

namespace Tasks
{
    public class whoami2 : Tasking
    {
        public whoami2(IAgent agent, ApolloInterop.Structs.MythicStructs.MythicTask data) : base(agent, data)
        {
        }


        public override void Start()
        {
            MythicTaskResponse resp;
            if (_agent.GetIdentityManager().GetCurrentLogonInformation(out var logonInfo))
            {
                resp = CreateTaskResponse("Hello World!", true);
            }
            else
            {
                resp = CreateTaskResponse("Hello World!", true);
            }
            // Your code here..
            // Then add response to queue
            _agent.GetTaskManager().AddTaskResponseToQueue(resp);
        }
    }
}

#endif