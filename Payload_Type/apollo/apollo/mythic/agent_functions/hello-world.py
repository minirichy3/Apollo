from mythic_container.MythicCommandBase import *
import json


class HelloworldArguments(TaskArguments):

    def __init__(self, command_line, **kwargs):
        super().__init__(command_line, **kwargs)
        self.args = []

    async def parse_arguments(self):
        if len(self.command_line) > 0:
            raise Exception("hello-world takes no command line arguments.")
        pass


class HelloworldCommand(CommandBase):
    cmd = "hello-world"
    needs_admin = False
    help_cmd = "hello-world"
    description = "Get the username associated with your current thread token."
    version = 2
    author = "@djhohnstein"
    argument_class = HelloworldArguments
    attackmapping = ["T1033"]

    async def create_go_tasking(self, taskData: PTTaskMessageAllData) -> PTTaskCreateTaskingMessageResponse:
        response = PTTaskCreateTaskingMessageResponse(
            TaskID=taskData.Task.ID,
            Success=True,
        )
        return response

    async def process_response(self, task: PTTaskMessageAllData, response: any) -> PTTaskProcessResponseMessageResponse:
        resp = PTTaskProcessResponseMessageResponse(TaskID=task.Task.ID, Success=True)
        return resp