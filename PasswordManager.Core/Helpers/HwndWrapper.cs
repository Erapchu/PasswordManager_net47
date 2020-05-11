using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Core.Helpers
{
	public class HwndWrapper : System.Windows.Forms.IWin32Window
	{
		#region Properties

		private readonly IntPtr _handle;
		/// <summary>
		/// Gets the handle to the window represented by the implementer.
		/// </summary>
		public IntPtr Handle { get { return _handle; } }

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a new instance of the <see cref="HwndWrapper"/> class with the specified handle.
		/// </summary>
		/// <param name="handle">A wrapped window HWND handle.</param>
		public HwndWrapper(IntPtr handle)
		{
			_handle = handle;
		}

		#endregion
	}
}
