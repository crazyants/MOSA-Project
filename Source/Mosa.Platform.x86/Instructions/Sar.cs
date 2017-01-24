﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Intermediate representation of the arithmetic shift right instruction.
	/// </summary>
	public sealed class Sar : X86Instruction
	{
		#region Data Members

		private static readonly LegacyOpCode C = new LegacyOpCode(new byte[] { 0xC1 }, 7);
		private static readonly LegacyOpCode C1 = new LegacyOpCode(new byte[] { 0xD1 }, 7);
		private static readonly LegacyOpCode RM = new LegacyOpCode(new byte[] { 0xD3 }, 7);

		#endregion Data Members

		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Shr"/>.
		/// </summary>
		public Sar() :
			base(1, 2)
		{
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Emits the specified platform instruction.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="emitter">The emitter.</param>
		internal override void EmitLegacy(InstructionNode node, X86CodeEmitter emitter)
		{
			if (node.Operand2.IsConstant)
			{
				if (node.Operand2.IsConstantOne)
				{
					emitter.Emit(C1, node.Result);
				}
				else
				{
					emitter.Emit(C, node.Result, node.Operand2);
				}
			}
			else
			{
				emitter.Emit(RM, node.Operand1);
			}
		}

		#endregion Methods
	}
}
