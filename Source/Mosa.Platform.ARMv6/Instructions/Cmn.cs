// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common;
using Mosa.Compiler.Framework;

namespace Mosa.Platform.ARMv6.Instructions
{
	/// <summary>
	/// Cmn instruction: Compare Negative
	/// Sets flags. Like ADD but with no destination register.
	/// </summary>
	public class Cmn : ARMv6Instruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Cmn"/>.
		/// </summary>
		public Cmn() :
			base(1, 3)
		{
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Emits the specified platform instruction.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="emitter">The emitter.</param>
		protected override void Emit(InstructionNode node, ARMv6CodeEmitter emitter)
		{
			EmitDataProcessingInstruction(node, emitter, Bits.b1011);
		}

		#endregion Methods
	}
}
