#define COMMAND_NAME_UPPER

#if DEBUG
#define HELLOWORLD
#endif

#if HELLOWORLD

using ApolloInterop.Classes;
using ApolloInterop.Interfaces;
using ApolloInterop.Structs.MythicStructs;

namespace Tasks
{
    public class helloworld : Tasking
    {
        public helloworld(IAgent agent, ApolloInterop.Structs.MythicStructs.MythicTask data) : base(agent, data)
        {
        }


        public override void Start()
        {
            MythicTaskResponse resp;
            if (_agent.GetIdentityManager().GetCurrentLogonInformation(out var logonInfo))
            {
                resp = CreateTaskResponse(
                    $"Hello" +
                    $"World!", true);
            }
            else
            {
                resp = CreateTaskResponse(
                    $"Hello" +
                    $"World!", true);
            }
            // Your code here..
            // Then add response to queue
            _agent.GetTaskManager().AddTaskResponseToQueue(resp);
        }
    }
}

#endif