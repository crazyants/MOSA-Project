﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representations the x86 rotate right instruction.
	/// </summary>
	public sealed class Rcr : X86Instruction
	{
		#region Data Members

		private static readonly LegacyOpCode C = new LegacyOpCode(new byte[] { 0xC1 }, 3);
		private static readonly LegacyOpCode C1 = new LegacyOpCode(new byte[] { 0xD1 }, 3);
		private static readonly LegacyOpCode RM = new LegacyOpCode(new byte[] { 0xD3 }, 3);

		#endregion Data Members

		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Rcr"/>.
		/// </summary>
		public Rcr() :
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
