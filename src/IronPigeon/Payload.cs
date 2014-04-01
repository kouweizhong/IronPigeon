namespace IronPigeon {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net.Http.Headers;
	using System.Runtime.Serialization;
	using System.Text;
	using Validation;

	/// <summary>
	/// The payload in a securely transmitted message, before encryptions and signatures are applied
	/// or after they are removed.
	/// </summary>
	public class Payload {
		/// <summary>
		/// Initializes a new instance of the <see cref="Payload" /> class.
		/// </summary>
		/// <param name="content">The content to upload.</param>
		/// <param name="contentType">Type of the content.</param>
		public Payload(Stream content, MediaTypeHeaderValue contentType) {
			Requires.NotNull(content, "content");
			Requires.NotNull(contentType, "contentType");

			this.Content = content;
			this.ContentType = contentType;
		}

		/// <summary>
		/// Gets or sets the stream that constitutes the payload.
		/// </summary>
		public Stream Content { get; set; }

		/// <summary>
		/// Gets or sets the content-type that describes the type of data that is
		/// serialized in the <see cref="Content"/> property.
		/// </summary>
		public MediaTypeHeaderValue ContentType { get; set; }

		/// <summary>
		/// Gets or sets the location of the payload reference that led to the discovery of this payload.
		/// </summary>
		internal Uri PayloadReferenceUri { get; set; }
	}
}
