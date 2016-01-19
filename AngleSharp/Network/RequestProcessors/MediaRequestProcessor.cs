﻿namespace AngleSharp.Network.RequestProcessors
{
    using AngleSharp.Dom;
    using AngleSharp.Dom.Media;
    using AngleSharp.Services.Media;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// See the following link for more infos:
    /// https://html.spec.whatwg.org/multipage/embedded-content.html#dom-media-load
    /// </summary>
    class MediaRequestProcessor<TMediaInfo> :ResourceRequestProcessor<TMediaInfo>
        where TMediaInfo : IMediaInfo
    {
        #region Fields

        TMediaInfo _media;

        #endregion

        #region ctor

        private MediaRequestProcessor(IConfiguration options, IResourceLoader loader)
            : base(options, loader)
        {
        }

        internal static MediaRequestProcessor<TMediaInfo> Create(Document document)
        {
            var options = document.Options;
            var loader = document.Loader;

            return options != null && loader != null ?
                new MediaRequestProcessor<TMediaInfo>(options, loader) : null;
        }

        #endregion

        #region Properties

        public MediaNetworkState NetworkState
        {
            get
            {
                var download = Download;

                if (download != null)
                {
                    if (download.IsRunning)
                    {
                        return MediaNetworkState.Loading;
                    }
                    else if (Resource == null)
                    {
                        return MediaNetworkState.NoSource;
                    }
                }

                return MediaNetworkState.Idle; 
            }
        }

        #endregion

        #region Methods

        protected override async Task ProcessResponse(IResponse response)
        {
            var service = GetService(response);

            if (service != null)
            {
                var cancel = CancellationToken.None;
                _media = await service.CreateAsync(response, cancel).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
