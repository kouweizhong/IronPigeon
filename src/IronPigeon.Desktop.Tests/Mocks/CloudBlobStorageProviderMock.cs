namespace IronPigeon.Tests.Mocks {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using NUnit.Framework;
	using Validation;

	internal class CloudBlobStorageProviderMock : ICloudBlobStorageProvider {
		internal static readonly string BaseUploadUri = "http://localhost/blob/";

		private readonly Dictionary<Uri, Tuple<MediaTypeHeaderValue, byte[]>> blobs = new Dictionary<Uri, Tuple<MediaTypeHeaderValue, byte[]>>();

		internal CloudBlobStorageProviderMock() {
		}

		internal Dictionary<Uri, Tuple<MediaTypeHeaderValue, byte[]>> Blobs {
			get { return this.blobs; }
		}

		#region ICloudBlobStorageProvider Members

		public async Task<Uri> UploadMessageAsync(Stream encryptedMessageContent, DateTime expiration, MediaTypeHeaderValue contentType, string contentEncoding, IProgress<int> bytesCopiedProgress, CancellationToken cancellationToken) {
			var ms = new MemoryStream();
			await encryptedMessageContent.CopyToAsync(ms, 4096, cancellationToken);
			var buffer = ms.ToArray();
			Assert.That(buffer.Length, Is.GreaterThan(0));
			lock (this.blobs)
			{
				var contentUri = new Uri(BaseUploadUri + (this.blobs.Count + 1));
				this.blobs[contentUri] = Tuple.Create(contentType, buffer);
				return contentUri;
			}
		}

		#endregion

		internal void AddHttpHandler(HttpMessageHandlerMock httpMock) {
			Requires.NotNull(httpMock, "httpMock");
			httpMock.RegisterHandler(this.HandleRequest);
		}

		private Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request) {
			Tuple<MediaTypeHeaderValue, byte[]> contentTypeAndBuffer;
			if (this.blobs.TryGetValue(request.RequestUri, out contentTypeAndBuffer)) {
				var content = new StreamContent(new MemoryStream(contentTypeAndBuffer.Item2));
				content.Headers.ContentType = contentTypeAndBuffer.Item1;
				return Task.FromResult(new HttpResponseMessage() { Content = content });
			}

			return Task.FromResult<HttpResponseMessage>(null);
		}
	}
}
