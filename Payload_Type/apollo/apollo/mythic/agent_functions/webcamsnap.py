from mythic_container.MythicCommandBase import *
from uuid import uuid4
import json
from os import path
from mythic_container.MythicRPC import *
import base64

class WebcamSnapArguments(TaskArguments):

    def __init__(self, command_line, **kwargs):
        super().__init__(command_line, **kwargs)
        self.args = []

    async def parse_arguments(self):
        pass


class WebcamSnapCommand(CommandBase):
    cmd = "webcamsnap"
    needs_admin = False
    help_cmd = "webcamsnap"
    description = "Take a webcam snap of the computer."
    version = 2
    author = "@reznok, @djhohnstein"
    argument_class = ScreenshotArguments
    browser_script = BrowserScript(script_name="webcamsnap", author="@djhohnstein", for_new_ui=True)
    attackmapping = ["T1113"]

    async def create_go_tasking(self, taskData: PTTaskMessageAllData) -> PTTaskCreateTaskingMessageResponse:
        response = PTTaskCreateTaskingMessageResponse(
            TaskID=taskData.Task.ID,
            Success=True,
        )
        return response

    async def process_response(self, task: PTTaskMessageAllData, response: any) -> PTTaskProcessResponseMessageResponse:
        resp = PTTaskProcessResponseMessageResponse(TaskID=task.Task.ID, Success=True)
        return resp
