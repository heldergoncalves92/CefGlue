using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;
using Xilium.CefGlue.Common.Serialization;

namespace Xilium.CefGlue.Common.JavascriptExecution
{
    internal class JavascriptExecutionEngine
    {
        private static volatile int lastTaskId;

        private readonly CefBrowser _browser;
        private readonly ConcurrentDictionary<int, TaskCompletionSource<object>> _pendingTasks = new ConcurrentDictionary<int, TaskCompletionSource<object>>();

        public JavascriptExecutionEngine(CefBrowser browser, MessageDispatcher dispatcher)
        {
            _browser = browser;

            dispatcher.RegisterMessageHandler(Messages.JsEvaluationResult.Name, HandleScriptEvaluationResultMessage);
            dispatcher.RegisterMessageHandler(Messages.JsContextCreated.Name, HandleContextCreatedMessage);
            dispatcher.RegisterMessageHandler(Messages.JsContextReleased.Name, HandleContextReleasedMessage);
        }

        public bool IsMainFrameContextInitialized { get; private set; }

        public event Action<CefFrame> ContextCreated;
        public event Action<CefFrame> ContextReleased;

        private void HandleScriptEvaluationResultMessage(MessageReceivedEventArgs args)
        {
            var message = Messages.JsEvaluationResult.FromCefMessage(args.Message);

            if (_pendingTasks.TryRemove(message.TaskId, out var pendingTask))
            {
                if (message.Success)
                {
                    pendingTask.SetResult(CefValueSerialization.DeserializeCefValue(message.Result));
                }
                else
                {
                    pendingTask.SetException(new Exception(message.Exception));
                }
            }
        }

        private void HandleContextCreatedMessage(MessageReceivedEventArgs args)
        {
            var message = Messages.JsContextCreated.FromCefMessage(args.Message);
            if (message.FrameId == null)
            {
                IsMainFrameContextInitialized = true;
            }
            ContextCreated?.Invoke(_browser.GetFrame(message.FrameId ?? ""));
        }

        private void HandleContextReleasedMessage(MessageReceivedEventArgs args)
        {
            var message = Messages.JsContextReleased.FromCefMessage(args.Message);
            if (message.FrameId == null)
            {
                IsMainFrameContextInitialized = false;
            }
            ContextReleased?.Invoke(_browser.GetFrame(message.FrameId ?? ""));
        }

        public async Task<T> Evaluate<T>(string script, string url, int line, CefFrame frame)
        {
            var taskId = lastTaskId++;
            var message = new Messages.JsEvaluationRequest()
            {
                TaskId = taskId,
                FrameId = frame.Name,
                Script = script,
                Url = url,
                Line = line
            };

            var messageReceiveCompletionSource = new TaskCompletionSource<object>();

            _pendingTasks.TryAdd(taskId, messageReceiveCompletionSource);

            try
            {
                _browser.SendProcessMessage(CefProcessId.Renderer, message.ToCefProcessMessage());

                // TODO should we add any timeout param and remove the task after that ?
                await messageReceiveCompletionSource.Task;
            }
            catch
            {
                _pendingTasks.TryRemove(taskId, out var dummy);
                throw;
            }

            var result = messageReceiveCompletionSource.Task.Result;
            var typedResult = JavascriptToNativeTypeConverter.ConvertToNative(result, typeof(T));

            return (T)typedResult;
        }
    }
}
