namespace IronPigeon {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using PCLCrypto;
	using Validation;

	/// <summary>
	/// Wraps another crypto transform so that when transformation has completed
	/// a verification can be run.
	/// </summary>
	internal class VerifyingCryptoTransform : ICryptoTransform {
		private readonly ICryptoTransform inner;
		private Action verification;

		/// <summary>
		/// Initializes a new instance of the <see cref="VerifyingCryptoTransform"/> class.
		/// </summary>
		/// <param name="inner">The inner.</param>
		/// <param name="verification">The method to invoke when the final transform result has been processed. This may throw if the verification fails.</param>
		internal VerifyingCryptoTransform(ICryptoTransform inner, Action verification) {
			Requires.NotNull(inner, "inner");
			Requires.NotNull(verification, "verification");

			this.inner = inner;
			this.verification = verification;
		}

		public bool CanReuseTransform {
			get { return false; }
		}

		public bool CanTransformMultipleBlocks {
			get { return this.inner.CanTransformMultipleBlocks; }
		}

		public int InputBlockSize {
			get { return this.inner.InputBlockSize; }
		}

		public int OutputBlockSize {
			get { return this.inner.OutputBlockSize; }
		}

		public void Dispose() {
			this.inner.Dispose();
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset) {
			return this.inner.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount) {
			byte[] result = this.inner.TransformFinalBlock(inputBuffer, inputOffset, inputCount);
			if (this.verification != null) {
				this.verification();
			}

			return result;
		}
	}
}
