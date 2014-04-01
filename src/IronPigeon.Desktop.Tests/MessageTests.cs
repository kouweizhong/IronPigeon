namespace IronPigeon.Tests {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
	public class MessageTests {
		[Test]
		public void CtorInvalidArgs() {
			Assert.Throws<ArgumentNullException>(() => new Payload(null, Valid.ContentType));
			Assert.Throws<ArgumentNullException>(() => new Payload(new MemoryStream(Valid.MessageContent), null));
		}

		[Test]
		public void Ctor() {
			var content = new MemoryStream(Valid.MessageContent);
			var message = new Payload(content, Valid.ContentType);
			Assert.That(message.Content, Is.SameAs(content));
			Assert.That(message.ContentType, Is.EqualTo(Valid.ContentType));
		}
	}
}
