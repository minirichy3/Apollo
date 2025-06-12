from mythic_container.MythicRPC import *
from mythic_container.MythicCommandBase import *

class HelloworldArguments(TaskArguments):
    def __init__(self, command_line, **kwargs):
        super().__init__(command_line, **kwargs)
        self.args = []

    async def parse_arguments(self):
        pass

class HelloworldCommand(CommandBase):
    cmd = "hello-world"
    needs_admin = False
    help_cmd = "hello-world"
    description = "Prints a Simple Hello World."
    version = 1
    author = "@minirichy"
    attackmapping = ["T1033"]
    argument_class = HelloworldArguments
    
    async def create_go_tasking(self, taskData: PTTaskMessageAllData) -> PTTaskCreateTaskingMessageResponse:
        response = PTTaskCreateTaskingMessageResponse(
            TaskID=taskData.Task.ID,
            Success=True,
        )
        return response

    async def process_response(self, task: PTTaskMessageAllData, response: any) -> PTTaskProcessResponseMessageResponse:
        resp = PTTaskProcessResponseMessageResponse(TaskID=task.Task.ID, Success=True)
        return response

