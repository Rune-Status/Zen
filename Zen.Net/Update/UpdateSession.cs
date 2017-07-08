using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Zen.Fs;
using Zen.Game;
using Zen.Net.Service;
using Zen.Shared;

namespace Zen.Net.Update
{
    public class UpdateSession : Session
    {
        private readonly LinkedList<FileRequest> _fileRequests = new LinkedList<FileRequest>();
        private readonly UpdateService _service;

        private bool _handshakeComplete;
        private bool _idle = true;

        public UpdateSession(ServiceManager serviceManager, GameServer server, IChannel channel) : base(serviceManager,
            server, channel)
        {
            _service = serviceManager.UpdateService;
        }

        public override void MessageReceived(object message)
        {
            if (_handshakeComplete)
            {
                var request = message as FileRequest;
                if (request != null)
                {
                    lock (_fileRequests)
                    {
                        if (request.Priority)
                            _fileRequests.AddFirst(request);
                        else
                            _fileRequests.AddLast(request);

                        if (!_idle) return;

                        _service.AddPendingSession(this);
                        _idle = false;
                    }
                }
                else if (message is UpdateEncryptionMessage)
                {
                    var encryption = (UpdateEncryptionMessage) message;
                    var encoder = Channel.Pipeline.Get<XorEncoder>();
                    encoder.Key = encryption.Key;
                }
            }
            else
            {
                var version = (UpdateVersionMessage) message;

                var status = UpdateStatusMessage.StatusOk;
                if (version.Version != GameConstants.Version)
                    status = UpdateStatusMessage.StatusOutOfDate;

                var future = Channel.WriteAndFlushAsync(new UpdateStatusMessage(status));
                if (status == UpdateStatusMessage.StatusOk)
                {
                    Channel.Pipeline.Remove<ReadTimeoutHandler>();
                    _handshakeComplete = true;
                }
                else
                {
                    future.ContinueWith(delegate { Channel.CloseAsync(); });
                }
            }
        }

        public void ProcessFileQueue()
        {
            FileRequest request;

            lock (_fileRequests)
            {
                request = _fileRequests.First.Value;
                _fileRequests.RemoveFirst();

                if (_fileRequests.Count == 0)
                {
                    _idle = true;
                }
                else
                {
                    _service.AddPendingSession(this);
                    _idle = false;
                }
            }

            if (request == null) return;

            var type = request.Type;
            var file = request.File;

            var cache = Server.Cache;
            IByteBuffer buffer;

            if (request.Type == 255 && request.File == 255)
            {
                var table = cache.ChecksumTable;
                var container = new Container(Container.CompressionNone, table.Encode());
                buffer = container.Encode();
            }
            else
            {
                buffer = cache.Store.Read(type, file);
                if (type != 255)
                    buffer = buffer.Slice(0, buffer.ReadableBytes - 2);
            }

            Channel.WriteAndFlushAsync(new FileResponse(request.Priority, request.Type, request.File, buffer));
        }

        public override void Unregister()
        {
            _fileRequests.Clear();
        }
    }
}