/*
 * (c) 2012 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using Mosa.Compiler.Framework;

namespace Mosa.Platform.AVR32
{
	/// <summary>
	/// Base class for AVR32 registers.
	/// </summary>
	public abstract class GenericAVR32Register : Register
	{
		#region Data members

		#endregion // Data members

		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="GenericAVR32Register"/>.
		/// </summary>
		/// <param name="index"></param>
		protected GenericAVR32Register(int index) :
			base(index)
		{
		}

		#endregion // Construction

		#region Properties

		#endregion // Properties
	}
}